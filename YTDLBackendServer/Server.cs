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

namespace YTDLBackendServer
{
    public class Server
    {
        string downloadPath;
        bool isDefaultDir;
        HttpListener listener;
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
        }
    }
}
