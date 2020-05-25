using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace YTDLBackendServer
{
    public class ServerConfigReader
    {
        public static ServerConfig ReadConfig()
        {
            ServerConfig returnConfig = new ServerConfig();
            returnConfig.port = 9000;
            returnConfig.isDefaultDownloadDirectory = true;
            returnConfig.downloadDirectory = "";
            if (File.Exists("config.ini"))
            {
                Log.WriteLog(LogType.Info, "Reading config.ini...");
                string fileText = File.ReadAllText("config.ini");
                string[] fileContents = fileText.Split(';');
                foreach (string option in fileContents)
                {
                    if (option.Contains("DownloadDir="))
                    {
                        if (option.Contains("DownloadDir=Default"))
                        {
                            Log.WriteLog(LogType.Info, "Download directory set to default");
                        }
                        else
                        {
                            returnConfig.isDefaultDownloadDirectory = false;
                            string[] optionSplit = option.Split('=');
                            returnConfig.downloadDirectory = optionSplit[1];
                            Log.WriteLog(LogType.Info, "Download directory set to " + optionSplit[1]);
                        }
                    }
                    else if (option.Contains("PortNumber="))
                    {
                        string[] optionSplit = option.Split('=');
                        int portNo = int.Parse(optionSplit[1]);
                        Log.WriteLog(LogType.Info, "Port set to " + optionSplit[1]);
                    }
                }
            }
            else
            {
                Log.WriteLog(LogType.Info, "Creating config.ini with default download dir and port 9000");
                File.WriteAllText("config.ini", "DownloadDir=Default;\nPortNumber=9000;");
            }
            return returnConfig;
        }
    }
    public struct ServerConfig
    {
        public int port;
        public string downloadDirectory;
        public bool isDefaultDownloadDirectory;
    }
}
