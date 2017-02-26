using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Otaku
{
    public class Logger
    {
        private static Logger _instance;
        public static Logger Instance => _instance ?? (_instance = new Logger());

        private Logger()
        {
        }

        [Conditional("Debug")]
        public void Debug(string message,
            [CallerFilePath] string filePath = "",
            [CallerLineNumber] int lineNumber = 0)
        {
            Console.WriteLine($"${filePath}:${lineNumber}: ${message}");
        }
    }
}