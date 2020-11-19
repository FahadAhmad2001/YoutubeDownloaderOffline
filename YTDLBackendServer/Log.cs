using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace YTDLBackendServer
{
    public class Log
    {
        static string CurrentLogContents = "";
        public static void WriteLog(LogType type, string content)
        {
            string logContent = type.ToString().ToUpper() + ": " + content;
            Console.WriteLine(DateTime.Now + " " + logContent);
            CurrentLogContents = CurrentLogContents + DateTime.Now + "    " + logContent + "\n";
        }
        public static void StartLog()
        {
            if (File.Exists("server.log"))
            {
                File.Delete("server.log");
            }
            Timer loggingInterval = new Timer(FlushLog, null, 0, 1000);
        }
        private static void FlushLog(object o)
        {
            File.AppendAllText("server.log", CurrentLogContents);
            CurrentLogContents = "";
        }
    }
    public enum LogType
    {
        General =0,
        Info,
        Warning,
        Error
    }
}
