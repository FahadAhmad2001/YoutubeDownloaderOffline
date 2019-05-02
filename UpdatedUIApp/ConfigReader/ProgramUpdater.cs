using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Windows.Forms;


namespace UpdatedUIApp.ConfigReader
{
    public class ProgramUpdater
    {
        WebClient DownloadClient;
        public void Update()
        {
            if(File.Exists(Directory.GetCurrentDirectory() + "\\lsyt.exe"))
            {
                File.Delete(Directory.GetCurrentDirectory() + "\\lsyt.exe");
            }
            DownloadClient = new WebClient();
            DownloadClient.DownloadProgressChanged += DownloadClient_DownloadProgressChanged;
            DownloadClient.DownloadFileCompleted += DownloadClient_DownloadFileCompleted;
            DownloadClient.DownloadFileAsync(new Uri("http://89.203.4.93:500/youtubedownload/files/lsyoutubedownload.exe"), Directory.GetCurrentDirectory() + "\\lsyt.exe");
        }

        private void DownloadClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            //throw new NotImplementedException();
            DownloadComplete args = new DownloadComplete();
            args.IsSuccessful = File.Exists(Directory.GetCurrentDirectory() + "\\lsyt.exe");
            OnDownloadCompleted(args);
        }

        private void DownloadClient_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //throw new NotImplementedException();
            //File.AppendAllText("yo.txt",e.ProgressPercentage.ToString());
            DownloadProgChanged args = new DownloadProgChanged();
            args.DownloadPercentage = e.ProgressPercentage;
            OnDownloadChanged(args);
        }
        public event EventHandler<DownloadProgChanged> DownloadProgressUpdated;
        public event EventHandler<DownloadComplete> DownloadCompleted;
        protected virtual void OnDownloadChanged(DownloadProgChanged args)
        {
            EventHandler<DownloadProgChanged> eventHandle = DownloadProgressUpdated;
            if (eventHandle != null)
            {
                eventHandle(this, args);
            }
        }
        protected virtual void OnDownloadCompleted(DownloadComplete args)
        {
            EventHandler<DownloadComplete> eventHandle = DownloadCompleted;
            if (eventHandle != null)
            {
                eventHandle(this, args);
            }
        }
    }
    public class DownloadProgChanged : EventArgs
    {
        public int DownloadPercentage { get; set; }
    }
    public class DownloadComplete : EventArgs
    {
        public bool IsSuccessful { get; set; }
    }
}
