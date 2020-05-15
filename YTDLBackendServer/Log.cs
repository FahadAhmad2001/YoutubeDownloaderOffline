using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace YTDLBackendServer
{
    public class Log
    {
        public static void WriteLog(LogType type, string content)
        {
            string logContent = type.ToString().ToUpper() + ": " + content;
            Console.WriteLine(DateTime.Now + " " + logContent);
            File.AppendAllText("server.log", DateTime.Now + "    " + logContent + "\n");
        }
        public static void StartLog()
        {
            if (File.Exists("server.log"))
            {
                File.Delete("server.log");
            }
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
