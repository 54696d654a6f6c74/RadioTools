using System.Runtime.CompilerServices;

namespace RadioTools
{
    public class Program
    {
        private static void Main(string[] args)
        {
            RuntimeHelpers.RunClassConstructor(typeof(Settings).TypeHandle);
            RuntimeHelpers.RunClassConstructor(typeof(Logger).TypeHandle);

            Commands.Parse(args);

            Logger.Finish();
        }
    }
}
