namespace RadioTools
{
    public static class Settings
    {
        public static Data dat {get; private set;}

        static Settings()
        {
            dat = Serializer.LoadJSON<Data>("settings");
        }

        // These have a public set because
        // the built in JSON serializer
        // is too retarded to serialize
        // private setter properties......
        public class Data
        {
            public string URL {get; set;} = "";
            public int timeout {get; set;} = 200;
            public string subnet {get; set;} = "192.168.0.";
            public ushort range {get; set;} = 255;
        }
    }
}