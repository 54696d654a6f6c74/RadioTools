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

            mapper = new Dictionary<string, ExecuteCommand>();
            flags = new List<char>();

            for(int i = 1; i < args.Length; i++)
            {
                if(args[i].ToString()[0] == '-')
                {
                    foreach(char c in args[i].Remove(0))
                        flags.Add(c);
                }
                else
                {
                    Logger.Println("Invalid arguments, please try again");
                    return;
                }
            }

            foreach(char flag in flags)
                Logger.Println("Found flag: " + flag);

            mapper.Add("scan", Scan);
            mapper.Add("seturls", SetURLs);
            mapper.Add("setvol", SetVolume);

            if(!mapper.ContainsKey(args[0]))
            {
                Logger.Println("Invalid command, please try again");
                return;
            }

            mapper[args[0]]();
        }

        private static void Scan()
        {
            List<ConnectionDetails> connections = new List<ConnectionDetails>();
            connections = NetworkTools.Scan();
            Serializer.SaveJSON(connections, "connections");
        }

        private static void SetURLs()
        {
            List<ConnectionDetails> connections = Serializer.LoadJSON<List<ConnectionDetails>>("connections");
            NetworkTools.SetURLs(connections);
        }

        private static void SetVolume()
        {
            throw new System.NotImplementedException();
        }

        private static void Run()
        {
            List<ConnectionDetails> connections = new List<ConnectionDetails>();
            connections = NetworkTools.Scan();
            Serializer.SaveJSON(connections, "connections");
            NetworkTools.SetURLs(connections);
        }

        private static void DisableOutput()
        {
            System.Console.SetOut(System.IO.TextWriter.Null);
        }
    }
}