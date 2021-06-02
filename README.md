## Server
#### Configuration:

**PORT** fille - the target port the server will listen on.
Note: In the case that the specified port is not available, the server will wait 10 seconds and attempt to start again.

**CMD_PATH** file - a path to the folder containing the commands available on the server
Note: Use absolute paths., a relative path will consider the current working directory to be the one from which the server was initaly executed.

## Client
#### Configuration:
The `settings.json` file can be edited to configure the client.
Note: If the file is deleted or not present, a new one will be generated.

#### Usage:
`help` - will display a consise overview of the available commands, flags and their syntax.

`scan` - pings every ip on the subnet, within the range specified in the `settings.json`, saves all the active connections in `connections.json` and attempts to get their hostname and available commands.

`newcmd` - takes a `<path>` argument that specifies a file, it turns the file into a bytearray an sends it to every active connection as a new command script;
optionally a `[name]` argument can be passed to specify the name of the command, if no such name is provided, the file name will be used instead.

`callcmd` - takes a `<name>` argument that specifies the name of the command which should be called, it sends a request to every active connection to run the command script;
upon succsessful execution the server returns 'K' to the client.

**Important:** Make sure to `scan` the network before you attempt to run any other commands, an empty or non-existent connections.json results in a void command!
