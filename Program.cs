using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace RadioTools
{
    public class Program
    {
        private static void Main(string[] args)
        {
            RuntimeHelpers.RunClassConstructor(typeof(Settings).TypeHandle);
            RuntimeHelpers.RunClassConstructor(typeof(Logger).TypeHandle);

            List<ConnectionDetails> connections = new List<ConnectionDetails>();
            
            connections = NetworkTools.Scan();
            
            // Save the ones that are alive, when rescanning, I can check if those hostnames are alive
            // and if notm I can print which connections were lost!
            
            Serializer.SaveJSON(connections, "connections");

            NetworkTools.SetURLs(connections);

            Logger.Finish();
        }
    }
}
