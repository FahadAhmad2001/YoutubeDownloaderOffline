using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using YTDownloadLib;

namespace YTDLBackendServer
{
    public class ServerMaintainance
    {
        string folderPath = Directory.GetCurrentDirectory();
        public void Start()
        {
            Timer maintainanceInterval = new Timer(TimerCallBack, null, 0, 14400000);
        }
        public void TimerCallBack(object o)
        {
            Log.WriteLog(LogType.Info, "Starting maintainance...");
            if (!File.Exists(folderPath + "\\maintainance.lck"))
            {
                File.Create(folderPath + "\\maintainance.lck").Dispose();
            }
            Log.WriteLog(LogType.Info, "Checking for youtube-dl updates...");
            YTDLUpdater updater = new YTDLUpdater();
            updater.UpdatingDownloader += Updater_UpdatingDownloader;
            updater.DownloaderUpdateError += Updater_DownloaderUpdateError;
            updater.CheckYTDLUpdates();
            Log.WriteLog(LogType.Info, "Checking and deleting any old files...");
            string[] webmFiles = Directory.GetFiles(folderPath, "*.webm");
            foreach (string eachFile in webmFiles)
            {
                DateTime lastModified = File.GetLastWriteTime(eachFile);
                TimeSpan difference = DateTime.Now.Subtract(lastModified);
                if (difference.Hours > 4)
                {
                    File.Delete(eachFile);
                }

            }
            string[] mp4Files = Directory.GetFiles(folderPath, "*.mp4");
            foreach (string eachFile in mp4Files)
            {
                DateTime lastModified = File.GetLastWriteTime(eachFile);
                TimeSpan difference = DateTime.Now.Subtract(lastModified);
                if (difference.Hours > 4)
                {
                    File.Delete(eachFile);
                }

            }
            string[] mkvFiles = Directory.GetFiles(folderPath, "*.mkv");
            foreach (string eachFile in mkvFiles)
            {
                DateTime lastModified = File.GetLastWriteTime(eachFile);
                TimeSpan difference = DateTime.Now.Subtract(lastModified);
                if (difference.Hours > 4)
                {
                    File.Delete(eachFile);
                }

            }
            string[] jpgFiles = Directory.GetFiles(folderPath, "*.jpg");
            foreach (string eachFile in jpgFiles)
            {
                DateTime lastModified = File.GetLastWriteTime(eachFile);
                TimeSpan difference = DateTime.Now.Subtract(lastModified);
                if (difference.Hours > 4)
                {
                    File.Delete(eachFile);
                }

            }
            string[] mp3Files = Directory.GetFiles(folderPath, "*.mp3");
            foreach (string eachFile in mp3Files)
            {
                DateTime lastModified = File.GetLastWriteTime(eachFile);
                TimeSpan difference = DateTime.Now.Subtract(lastModified);
                if (difference.Hours > 4)
                {
                    File.Delete(eachFile);
                }

            }
            Log.WriteLog(LogType.Info, "Maintainance complete, deleting maintainance.lck...");
            File.Delete(folderPath + "\\maintainance.lck");
            Log.WriteLog(LogType.Info, "Next maintainance after 4 hours");
        }

        private void Updater_DownloaderUpdateError(string message)
        {
            //throw new NotImplementedException();
            Log.WriteLog(LogType.Error, "Failed to update youtube-dlc:\n" + message + "\n If this continues to occur please make sure your Internet works and try redownloading the program\nIf it still doesn't work please file an issue at https://github.com/FahadAhmad2001/YoutubeDownloaderOffline/issues");
        }

        private void Updater_UpdatingDownloader(string version)
        {
            //throw new NotImplementedException();
            Log.WriteLog(LogType.Info, "Updating youtube-dl to version " + version);
        }
    }
}