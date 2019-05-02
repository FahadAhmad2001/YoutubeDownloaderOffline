using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using System.Windows.Forms;

namespace UpdatedUIApp.ConfigReader
{
    public class ProgramConfigReader
    {
        public static VersionInfo GetLatestVersion()
        {
            VersionInfo info = new VersionInfo();
            Ping ping = new Ping();
            if (ping.Send("89.203.4.93").Status == IPStatus.Success)
            {
                WebClient client = new WebClient();
                if (File.Exists(Directory.GetCurrentDirectory() + "\\remoteversion.txt"))
                {
                    File.Delete(Directory.GetCurrentDirectory() + "\\remoteversion.txt");
                }
                client.DownloadFile("http://89.203.4.93:500/youtubedownload/remoteversion.txt", Directory.GetCurrentDirectory() + "\\remoteversion.txt");
                StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "\\remoteversion.txt");
                string content = reader.ReadToEnd();
                string[] parts = content.Split(new char[] { ':' });
                info.NewVersion = parts[0];
                info.VersionDate = parts[1];
                return info;
            }
            info.NewVersion = "NOT AVAILABLE";
            info.VersionDate = "NOT AVAILABLE";
            return info;
        }
        public static bool VerifyConfigExists()
        {
            if(File.Exists(Directory.GetCurrentDirectory() + "\\config.ini"))
            {
                return true;
            }
            return false;
        }
        public static bool VerifyRunProgramExists()
        {
            if (File.Exists(Directory.GetCurrentDirectory() + "\\program.txt"))
            {
                return true;
            }
            return false;
        }
        public static ConfigData GetCurrentConfigData()
        {
            ConfigData data = new ConfigData();
            if (VerifyConfigExists())
            {
                StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "\\config.ini");
                string content = reader.ReadToEnd();
                reader.Close();
                string[] parts = content.Split(Environment.NewLine.ToCharArray());
                string[] split = parts[0].Split(new char[] { '=' });
                data.CurrentVersion = split[1];
                if (parts[2].Contains("true"))
                {
                    data.IsAutoCheckUpdate = true;
                }
                if (parts[4].Contains("true"))
                {
                    data.IsUsingThumbnail = true;
                }
                if (parts[6].Contains("true"))
                {
                    data.IsUsingProgAfterDownload = true;
                }
                split= parts[8].Split(new char[] { '=' });
                data.DownloadLocation = split[1];
            }
            if (VerifyRunProgramExists())
            {
                StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "\\program.txt");
                string content = reader.ReadToEnd();
                reader.Close();
                data.ProgAfterDownloadText = content;
            }
            return data;
        }
        public static void WriteConfigData(ConfigData data)
        {
            string content = "Version=" + data.CurrentVersion + Environment.NewLine;
            content = content + "AutoCheckUpdate=" + data.IsAutoCheckUpdate.ToString().ToLower() + Environment.NewLine;
            content = content + "DownloadMP3Thumbnail=" + data.IsUsingThumbnail.ToString().ToLower() + Environment.NewLine;
            content = content + "RunProgramAfterDownload=" + data.IsUsingProgAfterDownload.ToString().ToLower() + Environment.NewLine;
            content = content + "SaveLocation=" + data.DownloadLocation;
            StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "\\config.ini");
            writer.Write(content);
            writer.Close();
            writer = new StreamWriter(Directory.GetCurrentDirectory() + "\\program.txt");
            writer.WriteLine(data.ProgAfterDownloadText);
            writer.Close();
        }
    }
    public struct ConfigData
    {
        public string CurrentVersion;
        public string DownloadLocation;
        public bool IsUsingThumbnail;
        public bool IsUsingProgAfterDownload;
        public string ProgAfterDownloadText;
        public bool IsAutoCheckUpdate;
    }
    public struct VersionInfo
    {
        public string NewVersion;
        public string VersionDate;
    }
}
