using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using System;

namespace RadioTools
{
    public class ConnectionDetails : IEquatable<ConnectionDetails>
    {
        // Check Settings.cs line 14 for the reason
        // why these have public setters. I swear, 
        // I'm never doing JSON serialization again....

        public IPAddress IP {get; set;}
        public string hostname {get; set;}
        public bool alive {get; set;}

        public ConnectionDetails() {}

        public ConnectionDetails(IPAddress _ip, string _hostname, bool _alive)
        {
            IP = _ip;
            hostname = _hostname;
            alive = _alive;
        }

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

        public bool Equals(ConnectionDetails other)
        {
            if(hostname != null)
                return hostname == other.hostname;
            Logger.Println("Warning, comparing based on IP!");
            Logger.Println("Returning " + (IP == other.IP).ToString());
            return IP == other.IP;
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