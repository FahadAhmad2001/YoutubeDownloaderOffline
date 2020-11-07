using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using YTDownloadLib;
using System.IO;
using System.Net;
using System.Xml;
using System.Text.RegularExpressions;

namespace YTDLBackendServer
{
    public class Server
    {
        string downloadPath;
        bool isDefaultDir;
        HttpListener listener;
        List<YTDownload> RunningDownloads = new List<YTDownload>();
        public async void Start(int port, bool isDefaultDownloadDir, string downloadDir="")
        {
            isDefaultDir = isDefaultDownloadDir;
            if (isDefaultDownloadDir)
            {
                downloadPath = Directory.GetCurrentDirectory();
            }
            else
            {
                downloadPath = downloadDir;
            }
            listener = new HttpListener();
            listener.Prefixes.Add("http://+:" + port.ToString() + "/getmetadata/");
            listener.Prefixes.Add("http://+:" + port.ToString() + "/downloadvid/");
            listener.Prefixes.Add("http://+:" + port.ToString() + "/download/");
            Log.WriteLog(LogType.Info, "Attempting to listen on http://0.0.0.0:" + port.ToString() + "/");
            listener.Start();
            Log.WriteLog(LogType.Info, "Server started, waiting for events...");
            while (listener.IsListening)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                try
                {
                    await ProcessRequest(context);
                }
                catch (Exception ex)
                {
                    Log.WriteLog(LogType.Error,ex.ToString());
                }
            }
        }
        private async Task ProcessRequest(HttpListenerContext context)
        {
            HttpListenerRequest request = context.Request;
            Log.WriteLog(LogType.Info, "Recieved a request for " + request.Url);
            if (request.RawUrl.Contains("getmetadata"))
            {
            StartCheckMaintainance:
                if (File.Exists(downloadPath + "\\maintainance.lck"))
                {
                    Log.WriteLog(LogType.Info, "Delaying request for 4 seconds as maintainance is currently running");
                    Thread.Sleep(4000);
                    goto StartCheckMaintainance;
                }
                string VidID = request.QueryString.GetValues(request.QueryString.AllKeys[0])[0];
                string VidURL = "https://www.youtube.com/watch?v=" + VidID;
                MetadataScraper scraper = new MetadataScraper();
                MetadataScrape scrape = new MetadataScrape();
                scrape = scraper.GetMetadata(VidURL);
                HttpListenerResponse response = context.Response;
                MemoryStream xmlData = new MemoryStream();
                //from https://stackoverflow.com/questions/16371037/save-xml-content-to-a-variable answer by Adil and https://csharp.net-tutorials.com/xml/writing-xml-with-the-xmlwriter-class/
                using (XmlWriter writer = XmlWriter.Create(xmlData))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement("VideoMetadata");
                    writer.WriteStartElement("VideoID");
                    writer.WriteString(VidID);
                    writer.WriteEndElement();
                    writer.WriteStartElement("Title");
                    writer.WriteString(scrape.VidTitle);
                    writer.WriteEndElement();
                    writer.WriteStartElement("ThumbnailURL");
                    writer.WriteString(scrape.ThumbnailURL);
                    writer.WriteEndElement();
                    writer.WriteStartElement("VideoQualities");
                    foreach(VidQuality quality in scrape.VidQualities)
                    {
                        writer.WriteStartElement("vidquality");
                        writer.WriteAttributeString("format", quality.Format);
                        writer.WriteAttributeString("size", quality.FileSize);
                        writer.WriteAttributeString("resolution", quality.Resolution);
                        writer.WriteAttributeString("ID", quality.VidNo);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteStartElement("AudioQualities");
                    foreach(AudQuality quality in scrape.AudQualities)
                    {
                        writer.WriteStartElement("audquality");
                        writer.WriteAttributeString("format", quality.Format);
                        writer.WriteAttributeString("size", quality.FileSize);
                        writer.WriteAttributeString("ID",quality.AudNo);
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();
                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
                string xmlString = ASCIIEncoding.UTF8.GetString(xmlData.ToArray());
                if (request.HttpMethod == "OPTIONS")
                {
                    response.AddHeader("Access-Control-Allow-Headers", "*");
                }
                response.AppendHeader("Access-Control-Allow-Origin", "*");
                byte[] buffer = Encoding.UTF8.GetBytes(xmlString);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.ContentType = "text/xml; encoding='utf-8'";
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                Log.WriteLog(LogType.Info, "Successfully sent output");
                response.Close();
            }
            else if (request.RawUrl.Contains("downloadvid"))
            {
            StartCheckMaintainance:
                if (File.Exists(downloadPath + "\\maintainance.lck"))
                {
                    Log.WriteLog(LogType.Info, "Delaying request for 4 seconds as maintainance is currently running");
                    Thread.Sleep(4000);
                    goto StartCheckMaintainance;
                }
                YTDownloader downloader = new YTDownloader();
                downloader.DownloadCompleted += Downloader_DownloadCompleted;
                downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
                string urlID = request.QueryString.GetValues(request.QueryString.AllKeys[0])[0];
                string isMP3Only = request.QueryString.GetValues(request.QueryString.AllKeys[1])[0];
                if (isMP3Only.ToLower().Contains("true"))
                {
                    string isUsingThumbnail = request.QueryString.GetValues(request.QueryString.AllKeys[2])[0];
                    DownloadType mp3DownloadType;
                    if (isUsingThumbnail.ToLower().Contains("true"))
                    {
                        mp3DownloadType = DownloadType.MP3Pic;
                    }
                    else
                    {
                        mp3DownloadType = DownloadType.MP3Only;
                    }
                    string vidURL = "https://www.youtube.com/watch?v=" + urlID;
                    VidQuality vQuality = new VidQuality();
                    vQuality.VidNo = "MP3-Only";
                    AudQuality aQuality = new AudQuality();
                    aQuality.AudNo = urlID;
                    YTDownload download = new YTDownload();
                    download.context = context;
                    download.aQuality = aQuality;
                    download.vQuality = vQuality;
                    download.VidID = urlID;
                    download.DownType = mp3DownloadType;
                    RunningDownloads.Add(download);
                    HttpListenerResponse response = context.Response;
                    if (request.HttpMethod == "OPTIONS")
                    {
                        response.AddHeader("Access-Control-Allow-Headers", "*");
                    }
                    response.AppendHeader("Access-Control-Allow-Origin", "*");
                    context.Response.SendChunked = true;
                    downloader.DownloadVideo(vidURL, mp3DownloadType, aQuality, vQuality);
                }
                else
                {
                    try
                    {
                        int VidID =  int.Parse(request.QueryString.GetValues(request.QueryString.AllKeys[2])[0]);
                        int AudID = int.Parse(request.QueryString.GetValues(request.QueryString.AllKeys[3])[0]);
                        string vidURL = "https://www.youtube.com/watch?v=" + urlID;
                        VidQuality vQuality = new VidQuality();
                        vQuality.VidNo = VidID.ToString();
                        AudQuality aQuality = new AudQuality();
                        aQuality.AudNo = AudID.ToString();
                        YTDownload download = new YTDownload();
                        download.aQuality = aQuality;
                        download.vQuality = vQuality;
                        download.VidID = urlID;
                        download.context = context;
                        download.DownType = DownloadType.CustomQuality;
                        RunningDownloads.Add(download);
                        HttpListenerResponse response = context.Response;
                        if (request.HttpMethod == "OPTIONS")
                        {
                            response.AddHeader("Access-Control-Allow-Headers", "*");
                        }
                        response.AppendHeader("Access-Control-Allow-Origin", "*");
                        context.Response.SendChunked = true;
                        downloader.DownloadVideo(vidURL, DownloadType.CustomQuality, aQuality, vQuality);
                    }
                    catch(Exception ex)
                    {
                        Log.WriteLog(LogType.Error, ex.ToString());
                    }

                }
            }
            else if (request.RawUrl.Contains("download"))
            {
            StartCheckMaintainance:
                if (File.Exists(downloadPath + "\\maintainance.lck"))
                {
                    Log.WriteLog(LogType.Info, "Delaying request for 4 seconds as maintainance is currently running");
                    Thread.Sleep(4000);
                    goto StartCheckMaintainance;
                }
                try
                {
                    HttpListenerResponse response = context.Response;
                    string linkName = request.QueryString.GetValues(request.QueryString.AllKeys[0])[0];
                    string fileName = linkName.Replace('*', ' ');
                    fileName = fileName.Replace('|', '&');
                    fileName = fileName.Replace('<', '=');
                    if (request.HttpMethod == "OPTIONS")
                    {
                        response.AddHeader("Access-Control-Allow-Headers", "*");
                    }
                    response.AppendHeader("Access-Control-Allow-Origin", "*");
                    if (File.Exists(downloadPath + "\\" + fileName))
                    {
                        string contentType = "";
                        FileInfo info = new FileInfo(fileName);
                        if (info.Extension.ToLower().Contains("mp4"))
                        {
                            contentType = "video/mp4";
                        }
                        else if (info.Extension.ToLower().Contains("mp3"))
                        {
                            contentType = "audio/mpeg";
                        }
                        else if (info.Extension.ToLower().Contains("webm"))
                        {
                            contentType = "video/webm";
                        }
                        else if (info.Extension.ToLower().Contains("mkv"))
                        {
                            contentType = "video/x-matroska";
                        }
                        response.StatusCode = (int)HttpStatusCode.OK;
                        using (FileStream stream = File.OpenRead(downloadPath + "\\" + fileName))
                        {
                            response.ContentType = contentType;
                            response.ContentLength64 = info.Length;
                            response.AddHeader(
                            "Content-Disposition",
                            "Attachment; filename=\"" + Path.GetFileName(downloadPath + "\\" + fileName) + "\"");
                            stream.CopyTo(response.OutputStream);

                        }
                    }
                    else
                    {
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        string responseString = "File not found";
                        byte[] data = Encoding.UTF8.GetBytes(responseString);
                        response.OutputStream.Write(data, 0, data.Length);
                    }
                    response.Close();
                }
                catch(Exception ex)
                {
                    Log.WriteLog(LogType.Error, "Error while downloading a file from server: " + ex.ToString());
                }
            }
        }

        private void Downloader_DownloadProgressChanged(DownloadProgress ProgData)
        {
            try
            {
                if (ProgData.ProgType == ProgressType.ProgressChanged)
                {
                    foreach(YTDownload download in RunningDownloads)
                    {
                        if(download.VidID==ProgData.VidID && download.vQuality.VidNo==ProgData.VidQual.VidNo && download.aQuality.AudNo == ProgData.AudQual.AudNo)
                        {
                            string responseString = ProgData.ProgressInfo + "\n";
                            download.context.Response.StatusCode = (int)HttpStatusCode.OK;
                            byte[] data = Encoding.UTF8.GetBytes(responseString);
                            download.context.Response.OutputStream.Write(data, 0, data.Length);
                            download.context.Response.OutputStream.Flush();
                        }
                    }
                }
                else if (ProgData.ProgType == ProgressType.FileNameGotten)
                {
                    foreach(YTDownload download in RunningDownloads)
                    {
                        if(download.VidID == ProgData.VidID && download.vQuality.VidNo == ProgData.VidQual.VidNo && download.aQuality.AudNo == ProgData.AudQual.AudNo)
                        {
                            Log.WriteLog(LogType.General, "Attempting to set filname for VidID " + download.VidID + " to " + ProgData.ProgressInfo);
                            SetFileName(download, ProgData.ProgressInfo);
                        }
                    }
                }
            }
            catch(InvalidOperationException ex)
            {
                Log.WriteLog(LogType.Warning, ex.ToString() + " - this is a warning (need to fix in future updates)");
            }
            catch(Exception ex)
            {
                Log.WriteLog(LogType.Error, ex.ToString());
            }
        }

        private void Downloader_DownloadCompleted(DownloadProgress ProgData)
        {
            try
            {
                if (ProgData.ProgType == ProgressType.Completed)
                {
                    Log.WriteLog(LogType.General, "Successfully downloaded file for vidID: " + ProgData.VidID);
                    YTDownload finishedDownload = new YTDownload();
                    foreach (YTDownload download in RunningDownloads)
                    {
                        if (download.VidID==ProgData.VidID && download.aQuality.AudNo == ProgData.AudQual.AudNo && download.vQuality.VidNo == ProgData.VidQual.VidNo)
                        {
                            if (File.Exists(downloadPath + "\\" + download.OriginalFileName))
                            {
                                try
                                {
                                    File.Copy(downloadPath + "\\" + download.OriginalFileName, downloadPath + "\\" + download.FileName);
                                    if (File.Exists(downloadPath + "\\" + download.FileName))
                                    {
                                        File.Delete(downloadPath + "\\" + download.OriginalFileName);
                                    }
                                    else
                                    {
                                        Log.WriteLog(LogType.Error, "Failed to rename downloaded file from " + download.OriginalFileName + " to " + download.FileName);
                                    }
                                }
                                catch(Exception ex)
                                {

                                }
                                
                            }
                            else
                            {
                                Log.WriteLog(LogType.Warning, "Unable to find original file for download with video ID " + download.VidID + ", user may not be able to download it\nOriginal file name: " + download.OriginalFileName + "\nModified file name: " + download.FileName);
                            }
                            string linkName = download.FileName.Replace(' ', '*');
                            linkName = linkName.Replace('&', '|');
                            linkName = linkName.Replace('=', '<');
                            download.context.Response.StatusCode = (int)HttpStatusCode.OK;
                            string responseString = "COMPLETED:" + linkName;
                            byte[] data = Encoding.UTF8.GetBytes(responseString);
                            download.context.Response.OutputStream.Write(data, 0, data.Length);
                            download.context.Response.OutputStream.Flush();
                            download.context.Response.Close();
                            finishedDownload = download;
                        }
                    }
                    RunningDownloads.Remove(finishedDownload);
                    Log.WriteLog(LogType.General, "Download VidID: " + finishedDownload.VidID + " successfully completed");
                }
            }
            catch(Exception ex)
            {
                Log.WriteLog(LogType.Error, ex.ToString());
            }
        }

        private void SetFileName(YTDownload download, string fileName)
        {
            try
            {
                YTDownload correctDownload;
                correctDownload = download;
                Log.WriteLog(LogType.General, "setting filename for download vidID:" + download.VidID);
                if (download.DownType == DownloadType.MP3Only || download.DownType == DownloadType.MP3Pic)
                {
                    if(fileName.Contains("-" + correctDownload.VidID + ".mp3"))
                    {
                        string[] newFileName = Regex.Split(fileName, "-" + correctDownload.VidID);
                        correctDownload.FileName = newFileName[0] + ".mp3";
                    }
                    else
                    {
                        correctDownload.FileName = fileName;
                    }
                    correctDownload.OriginalFileName = fileName;
                }
                else
                {
                    //need to figure out how to get file extension, currently it has to be passed in QueryString for non-MP3 downloads....
                    if (download.context.Request.QueryString.GetValues(download.context.Request.QueryString.AllKeys[4])[0] == "webm" && download.context.Request.QueryString.GetValues(download.context.Request.QueryString.AllKeys[5])[0] == "webm")
                    {
                        if (fileName.Contains("-" + correctDownload.VidID))
                        {
                            string[] newFileName = Regex.Split(fileName, "-" + correctDownload.VidID);
                            correctDownload.FileName = newFileName[0] + ".webm";
                        }
                        else
                        {
                            correctDownload.FileName = fileName + ".webm";
                        }
                        correctDownload.OriginalFileName = fileName + ".webm";
                    }
                    else if (download.context.Request.QueryString.GetValues(download.context.Request.QueryString.AllKeys[4])[0] == "mp4" && download.context.Request.QueryString.GetValues(download.context.Request.QueryString.AllKeys[5])[0] == "m4a")
                    {
                        if (fileName.Contains("-" + correctDownload.VidID))
                        {
                            string[] newFileName = Regex.Split(fileName, "-" + correctDownload.VidID);
                            correctDownload.FileName = newFileName[0] + ".mp4";
                        }
                        else
                        {
                            correctDownload.FileName = fileName + ".mp4";
                        }
                        correctDownload.OriginalFileName = fileName + ".mp4";
                    }
                    else
                    {
                        if (fileName.Contains("-" + correctDownload.VidID))
                        {
                            string[] newFileName = Regex.Split(fileName, "-" + correctDownload.VidID);
                            correctDownload.FileName = newFileName[0] + ".mkv";
                        }
                        else
                        {
                            correctDownload.FileName = fileName + ".mkv";
                        }
                        correctDownload.OriginalFileName = fileName + ".mkv";
                    }
                }
                Log.WriteLog(LogType.General, "set filename to " + download.FileName);
                RunningDownloads.Remove(download);
                RunningDownloads.Add(correctDownload);
            }
            catch (Exception ex)
            {
                Log.WriteLog(LogType.Error, "Error while setting file name: " + ex.ToString());
            }
        }
    }
    public struct YTDownload
    {
        public HttpListenerContext context;
        public VidQuality vQuality;
        public AudQuality aQuality;
        public string FileName;
        public string OriginalFileName;
        public string VidID;
        public DownloadType DownType;
    }
}
