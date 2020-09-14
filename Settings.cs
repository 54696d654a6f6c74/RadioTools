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
            public string connectionString {get; set;} = "mgradio_client";
            public ushort communicationByteSize {get; set;} = 100;
            public int connectionPort {get; set;} = 1999;
            public string URL {get; set;} = "";
            public string setURLcommand {get; set;} = "SETURL";
            public int timeout {get; set;} = 200;
            public string subnet {get; set;} = "192.168.0.";
            public ushort range {get; set;} = 255;
            public ushort volume {get; set;} = 90;
            public string setVolumeCommand {get; set;} = "SETVOLUME";
        }
    }
}