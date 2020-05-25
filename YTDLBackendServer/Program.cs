using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace YTDLBackendServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("LightSpeed YTDL Backend Server");
            Console.WriteLine("For more info checkout https://github.com/FahadAhmad2001/YoutubeDownloaderOffline");
            Log.StartLog();
            Thread MaintainanceThread = new Thread(RunMaintainance);
            Log.WriteLog(LogType.Info, "Starting maintainance thread...");
            MaintainanceThread.Start();
            Thread ServerThread = new Thread(RunServer);
            Log.WriteLog(LogType.Info, "Starting server thread...");
            ServerThread.Start();
            Console.ReadLine();
        }
        static void RunServer()
        {
            Server server = new Server();
            ServerConfig config = ServerConfigReader.ReadConfig();
            Log.WriteLog(LogType.Info, "Attempting to start server...");
            server.Start(config.port, config.isDefaultDownloadDirectory, config.downloadDirectory);
        }
        static void RunMaintainance()
        {
            ServerMaintainance maintainance = new ServerMaintainance();
            maintainance.Start();
        }
    }
}
