using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;


namespace RadioTools
{
    public class ConnectionDetails
    {
        public IPAddress IP {get; private set;}
        public string hostname {get; private set;}
        public bool alive {get; private set;}

        public ConnectionDetails() {}

        public ConnectionDetails(IPAddress ip)
        {
            IP = ip;
            alive = NetworkTools.Ping(IP);
            if(alive)
            {
                Logger.Println("Device found at: " + ip.ToString());
                hostname = NetworkTools.GetHostName(IP);
            }
        }
    }

    public class IPAddressConverter : JsonConverter<IPAddress>
    {
        public override void Write(Utf8JsonWriter writer, IPAddress value, JsonSerializerOptions options) 
        {
            writer.WriteStringValue(value.ToString());
        }

        public override IPAddress Read(ref Utf8JsonReader reader, System.Type objectType, JsonSerializerOptions options)
        {
            return IPAddress.Parse((string)reader.GetString());
        }
        
    }
}