using System.Collections.Generic;
using System.IO;

namespace RadioTools
{
    public static class Commands
    {
        private delegate void ExecuteCommand();

        private static Dictionary<string, ExecuteCommand> mapper;
        private static List<char> flags;
        private static string[] values;

        public static void Parse(string[] args)
        {
            if(args.Length == 0 || args[0] == "")
            {
                Run();
                return;
            }

            ParseFlags(args);
            CollectValues(args);

            mapper = new Dictionary<string, ExecuteCommand>();

            mapper.Add("scan", Scan);
            mapper.Add("newcmd", NewCMD);
            mapper.Add("callcmd", CallCMD);
            mapper.Add("help", Help);

            if(!mapper.ContainsKey(args[0]))
            {
                Logger.Println("Invalid command, please try again");
                return;
            }

            if(flags.Contains('s'))
                DisableOutput();
            if(flags.Contains('l'))
                DisableLogging();

            mapper[args[0]]();
        }

        private static void ParseFlags(string[] args)
        {
            flags = new List<char>();

            for(int i = 1; i < args.Length; i++)
            {
                if(args[i].ToString()[0] == '-')
                {
                    foreach(char c in args[i].Split('-')[1].ToCharArray())
                    {
                        if(!flags.Contains(c))
                            flags.Add(c);
                        else {
                            Logger.Println("Please don't repeat flags");
                        }
                    }
                }
            }
        }
        private static void CollectValues(string[] args)
        {
            int count = args.Length - (flags.Count + 1);

            if(count <= 0)
                return;
            try
            {
                values = new string[count];

                for(int i = 0; i < count; i++)
                    values[i] = args[args.Length - count + i];
            }
            catch
            {
                Logger.Println("Something went wrong when collecting values!");
                return;
            }
        }

        private static void Scan()
        {
            List<ConnectionDetails> connections = new List<ConnectionDetails>();
            connections = NetworkTools.Scan();

            if(!flags.Contains('n'))
                Serializer.SaveJSON(connections, "connections");
        }

        private static void NewCMD()
        {
            if(values == null || values[0] == string.Empty)
            {
                Logger.Println("Invalid command, please try again");
                return;
            }
            
            string name;
            if(values.Length < 2)
            {
                Logger.Println("No command name specified, using file name as command name...");
                name = ExtractName(values[0]);
            }
            else name = values[1];


            NewCMD(values[0], name);

            string ExtractName(string path)
            {
                string[] parts = path.Split(System.IO.Path.DirectorySeparatorChar);

                string file = parts[parts.Length - 1];
                parts = file.Split('.');

                string name = parts[0];

                if(parts.Length > 2)
                    Logger.Println("WARNING: Please use '.' only as a speparator for file name and file extension! Using " + name + "as command name...");

                return name;
            }
        }

        private static void NewCMD(string path, string name)
        {
            string script = File.ReadAllText(path);

            NetworkTools.CreateCommand(script, name);
        }

        private static void CallCMD()
        {
            if(values[0] == null || values[0] == string.Empty)
            {
                Logger.Println("Invalid command, please try again");
                return;
            }

            CallCMD(values[0]);
        }

        private static void CallCMD(string cmd)
        {
            NetworkTools.CallCommand(cmd);
        }

        private static void Run()
        {
            List<ConnectionDetails> connections = new List<ConnectionDetails>();
            connections = NetworkTools.Scan();
            Serializer.SaveJSON(connections, "connections");
        }

        private static void DisableOutput()
        {
            System.Console.SetOut(System.IO.TextWriter.Null);
        }

        private static void DisableLogging()
        {
            Logger.enabled = false;
        }

        // Using a regular print so it doesn't override the log
        private static void Help()
        {
            DisableLogging();
            System.Console.WriteLine("Available commands:");
            System.Console.WriteLine("newcmd, callcmd");
            System.Console.WriteLine("\nAvailable flags:");
            System.Console.WriteLine("-n\t Quick scan\t -> will not save connections.json");
            System.Console.WriteLine("-l\t No logging\t -> will not override the log.txt");
            System.Console.WriteLine("-s\t Silent\t\t -> will not output to console");
        }
    }
}