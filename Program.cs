using System.Runtime.CompilerServices;

namespace RadioTools
{
    public class Program
    {
        private static void Main(string[] args)
        {
            RuntimeHelpers.RunClassConstructor(typeof(Settings).TypeHandle);
            RuntimeHelpers.RunClassConstructor(typeof(Logger).TypeHandle);
            
            // Save the ones that are alive, when rescanning, I can check if those hostnames are alive
            // and if notm I can print which connections were lost!

            Commands.Parse(args);

            Logger.Finish();
        }
    }
}
