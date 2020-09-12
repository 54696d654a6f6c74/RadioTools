using System.Collections.Generic;

namespace RadioTools
{
    public static class Commands
    {
        private delegate void ExecuteCommand();
        private static Dictionary<string, ExecuteCommand> mapper;

        public static void Parse(string[] args)
        {
            if(args.Length == 0 || args[0] == "")
            {
                Run();
                return;
            }

            mapper = new Dictionary<string, ExecuteCommand>();

            mapper.Add("scan", Scan);
            mapper.Add("seturls", SetURLs);
            mapper.Add("setvol", SetVolume);

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

        }

        private static void Run()
        {
            List<ConnectionDetails> connections = new List<ConnectionDetails>();
            connections = NetworkTools.Scan();
            Serializer.SaveJSON(connections, "connections");
            NetworkTools.SetURLs(connections);
        }
    }
}