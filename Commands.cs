using System.Collections.Generic;

namespace RadioTools
{
    public static class Commands
    {
        private delegate void ExecuteCommand();

        private static Dictionary<string, ExecuteCommand> mapper;
        private static List<char> flags;

        public static void Parse(string[] args)
        {
            if(args.Length == 0 || args[0] == "")
            {
                Run();
                return;
            }

            if(ParseFlags(args) == false)
                return;

            mapper = new Dictionary<string, ExecuteCommand>();

            mapper.Add("scan", Scan);
            mapper.Add("seturls", SetURLs);
            mapper.Add("setvol", SetVolume);
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

        private static bool ParseFlags(string[] args)
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
                            return false;
                        }
                    }
                }
                else
                {
                    Logger.Println("Invalid arguments, please try again");
                    return false;
                }
            }
            return true;
        }

        private static void Scan()
        {
            List<ConnectionDetails> connections = new List<ConnectionDetails>();
            connections = NetworkTools.Scan();

            if(!flags.Contains('n'))
                Serializer.SaveJSON(connections, "connections");
        }

        private static void SetURLs()
        {
            List<ConnectionDetails> connections = Serializer.LoadJSON<List<ConnectionDetails>>("connections");
            SetURLs(connections);
        }

        private static void SetURLs(List<ConnectionDetails> connections)
        {
            NetworkTools.SetURLs(connections);
        }

        private static void SetVolume()
        {
            List<ConnectionDetails> connections = Serializer.LoadJSON<List<ConnectionDetails>>("connections");
            SetVolume(connections);
        }

        private static void SetVolume(List<ConnectionDetails> connections)
        {
            NetworkTools.SetVolume(connections);
        }

        private static void Run()
        {
            List<ConnectionDetails> connections = new List<ConnectionDetails>();
            connections = NetworkTools.Scan();
            Serializer.SaveJSON(connections, "connections");
            SetURLs(connections);
            SetVolume(connections);
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
            System.Console.WriteLine("scan, seturls, setvol");
            System.Console.WriteLine("\nAvailable flags:");
            System.Console.WriteLine("-n\t Quick scan\t -> will not save connections.json");
            System.Console.WriteLine("-l\t No logging\t -> will not override the log.txt");
            System.Console.WriteLine("-s\t Silent\t\t -> will not output to console");
        }
    }
}