import socket

import stat
from os import listdir, chmod

from subprocess import call

# splitlines removes trailing \n
# because python adds them when reading files
reader = open("HOST", 'r')
HOST = reader.read().splitlines()[0]
reader.close()

reader = open("PORT", 'r')
PORT = int(reader.read())
reader.close()


reader = open("CMD_PATH", 'r')
CMD_PATH = reader.read().splitlines()[0]
reader.close()


all_commands = None


def load_commands():
    global all_commands
    all_commands = listdir(CMD_PATH)


def send_big_packet(conn, size, packet):
    conn.sendall(int.to_bytes(size))
    conn.sendall(packet)


def create_command_file(data: str, name: str):
    name = name.rstrip('\x00')
    path = CMD_PATH + "/" + name + ".sh"
    file = open(path, 'w')

    file.write(data)

    file.close()

    chmod(path, stat.S_IRWXU)


def call_command(cmd: str) -> str:
    cmd = cmd.rstrip('\x00')
    path = CMD_PATH + "/" + cmd + ".sh"

    return call(["sh", path])


def create_command_request(conn):
    with conn:
        cmd_file_size = int.from_bytes(conn.recv(4), "little")

        cmd_file = conn.recv(cmd_file_size)

        cmd_file_name = conn.recv(32)

        conn.sendall(b'Done')

    create_command_file(cmd_file.decode(), cmd_file_name.decode())


def get_commands_request(conn):
    with conn:
        packet = bytearray('\n'.join(all_commands), "utf-8")
        packet_size = len(bytearray('\n'.join(all_commands), "utf-8")) * 8

        send_big_packet(conn, packet_size, packet)


def call_command_request(conn):
    with conn:
        cmd = conn.recv(32)
        call_command(cmd.decode())


print("Starting server on: " + HOST + ":" + str(PORT))


with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
    load_commands()
    sock.bind((HOST, PORT))
    sock.listen()
    while True:
        conn, addr = sock.accept()
        print("Connected by: ", addr)

        req = conn.recv(1).decode()

        if req == 'g':
            get_commands_request(conn)
        elif req == 'n':
            create_command_request(conn)
        elif req == 'x':
            call_command_request(conn)

        conn.sendall(b'K')
