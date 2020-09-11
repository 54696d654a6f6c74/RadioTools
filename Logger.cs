using System;
using System.IO;
using System.Text;

namespace RadioTools
{
    public static class Logger
    {
        private static StringBuilder log;

        static Logger()
        {
            log = new StringBuilder();
        }

        public static void Print(string text)
        {
            Console.Write(text);
            log.Append(text);
        }

        public static void Println(string text)
        {
            Console.WriteLine(text);
            log.Append(text + "\n");
        }
        public static void Finish()
        {
            File.WriteAllText("log.txt", log.ToString());
        }
    }
}