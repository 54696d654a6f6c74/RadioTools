import socket
from time import sleep

import stat
from os import listdir, chmod

from subprocess import run


logfile = open("LOG", 'w')


def log(message):
    print(message)
    logfile.write(message + "\n")


# if not path.isfile("HOST"):
writer = open("HOST", 'w')
local_ip = socket.gethostbyname(socket.gethostname())
writer.write(local_ip)
writer.close()

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

    result = run(["sh", path], capture_output=True)

    if result.stderr != b'':
        return (result.stdout + result.stderr).decode()
    else:
        return result.stdout.decode()


def create_command_request(conn):
    with conn:
        cmd_file_size = int.from_bytes(conn.recv(4), "little")

        cmd_file = conn.recv(cmd_file_size)

        cmd_file_name = conn.recv(32)

        conn.sendall(b'K')

    create_command_file(cmd_file.decode(), cmd_file_name.decode())


def get_commands_request(conn):
    with conn:
        packet = bytearray('\n'.join(all_commands), "utf-8")
        packet_size = len(bytearray('\n'.join(all_commands), "utf-8")) * 8

        send_big_packet(conn, packet_size, packet)


def call_command_request(conn):
    with conn:
        cmd = conn.recv(32)
        out = call_command(cmd.decode())
        log(out)

        conn.sendall(b'K')


log("Starting server on: " + HOST + ":" + str(PORT))
delay = 10


while True:
    try:
        with socket.socket(socket.AF_INET, socket.SOCK_STREAM) as sock:
            load_commands()
            sock.bind((HOST, PORT))
            sock.listen()
            print("Succsess, now listening...")
            while True:
                conn, addr = sock.accept()
                log("Connected by: " + str(addr[0]))

                req = conn.recv(1).decode()
                log("Recieved: " + req)

                if req == 'g':
                    get_commands_request(conn)
                elif req == 'n':
                    create_command_request(conn)
                elif req == 'x':
                    call_command_request(conn)
    except OSError:
        log("OS error on host, will reinit in " + str(delay) + " sec...")
        sleep(delay)
        continue
