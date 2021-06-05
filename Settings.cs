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
            public ushort maxTasksPerThread {get; set;} = 5;
            public int connectionPort {get; set;} = 1999;
            public int timeout {get; set;} = 200;
            public string subnet {get; set;} = "192.168.0.";
            public ushort range {get; set;} = 255;
            public ushort cmdNameSize {get; set;} = 32;
            public ushort defPacketSize {get; set;} = 32;
            public string newCMDRequest {get; set;} = "n";
            public string callCMDRequest {get; set;} = "x";
            public string getCMDsRequest {get; set;} = "g";
        }
    }
}