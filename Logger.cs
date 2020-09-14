using System;
using System.IO;
using System.Text;

namespace RadioTools
{
    public static class Logger
    {
        private static StringBuilder log;
        public static bool enabled = true;

        static Logger()
        {
            log = new StringBuilder();
        }

        public static void Print(string text)
        {
            Console.Write(text);
            if(enabled)
                log.Append(text);
        }

        public static void Println(string text)
        {
            Console.WriteLine(text);
            if(enabled)
                log.Append(text + "\n");
        }
        public static void Finish()
        {
            if(enabled)
                File.WriteAllText("log.txt", log.ToString());
        }
    }
}