﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Net;
using YTDownloadLib;
using UpdatedUIApp.ConfigReader;

namespace UpdatedUIApp
{
    /// <summary>
    /// Interaction logic for DownloadPage.xaml
    /// </summary>
    public partial class DownloadPage : Page
    {
        public string VidURL;
        public string VidTitleName;
        public Thread MetadataThread;
        public bool firstMetadata;
        public string AppPath = Directory.GetCurrentDirectory();
        public Process GetThumbnail = new Process();
        public ProcessStartInfo GetThumbnailInfo = new ProcessStartInfo("youtube-dl.exe");
        public Process GetTitle = new Process();
        public ProcessStartInfo GetTitleInfo = new ProcessStartInfo("youtube-dl.exe");
        public WebClient MetadataHelper = new WebClient();
        public MetadataScrape VidInfo;
        public List<AudQuality> AudQualities = new List<AudQuality>();
        public List<VidQuality> VidQualities = new List<VidQuality>();
        public string SelectedAQuality = "";
        public VidQuality SelectedVidQuality;
        public AudQuality SelectedAudQuality;
        public VidQuality DownloadVidQuality;
        public AudQuality DownloadAudQuality;
        public bool IsDownloadingMP3 = false;
        public bool IsDownloadRunning = false;
        public YTDownloader VidDownloader;
        public string FileNameNoEXT = "";
        public Process ConvertVid;
        public Thread ConvertingThread;
        public bool usingMP3Thumbnail = false;
        public string currentVersion = "";
        public bool AutoUpdate = false;
        public bool UsePostDownloadCmd = false;
        public string CmdText = "";
        public string SaveLoc = "Default";
        public DownloadPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            StatusText.Content = "";
            DownloadStatus.Content = "";
            VidTitle.Content = "";
            VidQualityBox.IsEnabled = false;
            AudioQualityBox.IsEnabled = false;
            firstMetadata = true;
            FileSizeLabel.Content = "";
            if (!ProgramConfigReader.VerifyConfigExists() || !ProgramConfigReader.VerifyRunProgramExists())
            {
                MessageBox.Show("Config file cannot be loaded!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                ConfigData data = ProgramConfigReader.GetCurrentConfigData();
                currentVersion = data.CurrentVersion;
                SaveLoc = data.DownloadLocation;
                AutoUpdate = data.IsAutoCheckUpdate;
                UsePostDownloadCmd = data.IsUsingProgAfterDownload;
                usingMP3Thumbnail = data.IsUsingThumbnail;
                CmdText = data.ProgAfterDownloadText;
            }
           /* if (AutoUpdate)
            {
                VersionInfo info = ProgramConfigReader.GetLatestVersion();
                if((info.NewVersion!="NOT AVAILABLE") && (info.NewVersion != currentVersion))
                {
                   MessageBoxResult result = MessageBox.Show("New version available, " + info.NewVersion + " from " + info.VersionDate + Environment.NewLine + "Would you like to update?", "Update Available",MessageBoxButton.YesNo,MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        this.NavigationService.Navigate(new Uri("OptionsPage.xaml", UriKind.Relative));
                    }
                }
            }*/
        }

        

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsDownloadRunning)
            {
                MessageBox.Show("Please wait for the current download to complete");
            }
            else
            {
                StatusText.Content = "Downloading...";
                VidDownloader = new YTDownloader();
                VidDownloader.DownloadProgressChanged += VidDownloader_DownloadProgressChanged;
                VidDownloader.DownloadCompleted += VidDownloader_DownloadCompleted;
                if (IsDownloadingMP3)
                {
                    if (usingMP3Thumbnail)
                    {
                        VidDownloader.DownloadVideo(VidURL, DownloadType.MP3Pic, SelectedAudQuality, SelectedVidQuality);
                    }
                    else
                    {
                        VidDownloader.DownloadVideo(VidURL, DownloadType.MP3Only, SelectedAudQuality, SelectedVidQuality);
                    }
                }
                else
                {
                    DownloadAudQuality = SelectedAudQuality;
                    DownloadVidQuality = SelectedVidQuality;
                    VidDownloader.DownloadVideo(VidURL, DownloadType.CustomQuality, SelectedAudQuality, SelectedVidQuality);
                }
            }
        }

        private void VidDownloader_DownloadCompleted(DownloadProgress ProgData)
        {
            //throw new NotImplementedException();
            //MessageBox.Show(ProgData.ProgressInfo + " " + ProgData.ProgType.ToString() + " " + ProgData.DownType + " " + ProgData.ProgressPercentage + "%");
            if (IsDownloadingMP3)
            {
                //IsDownloadRunning = false;
                string savepath = "";
                string message = "";
                if (SaveLoc == "Default")
                {
                    savepath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    message = " to My Documents";
                }
                else
                {
                    savepath = SaveLoc;
                }
                if(File.Exists(AppPath+ "\\" + FileNameNoEXT))
                {
                    if(File.Exists(savepath + "\\" + FileNameNoEXT))
                    {
                        MessageBoxResult result = MessageBox.Show("This MP3 has been downloaded previously. Would you like to overwrite it?" + Environment.NewLine + "Otherwise its saved with a different filename", "Overwrite", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            File.Delete(savepath + "\\" +FileNameNoEXT);
                            File.Copy(AppPath + "\\" + FileNameNoEXT, savepath + "\\" + FileNameNoEXT);
                            File.Delete(AppPath + "\\" + FileNameNoEXT);
                            if (UsePostDownloadCmd)
                            {
                                Process.Start("cmd.exe", CmdText);
                            }
                        }
                        else
                        {
                            File.Copy(AppPath + "\\" + FileNameNoEXT, savepath + "\\" + FileNameNoEXT + " -NEW-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + ".mp3");
                            File.Delete(AppPath + "\\" + FileNameNoEXT);
                            if (UsePostDownloadCmd)
                            {
                                Process.Start("cmd.exe", CmdText);
                            }
                        }
                    }
                    else
                    {
                        File.Copy(AppPath + "\\" + FileNameNoEXT, savepath + "\\" + FileNameNoEXT);
                        File.Delete(AppPath + "\\" + FileNameNoEXT);
                        if (UsePostDownloadCmd)
                        {
                            Process.Start("cmd.exe", CmdText);
                        }
                    }
                    StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                    DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                    DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                    IsDownloadRunning = false;
                    DownloadAudQuality = new AudQuality();
                    DownloadVidQuality = new VidQuality();
                    FileNameNoEXT = "";
                    MessageBox.Show("MP3 Downloaded successfully and saved" + message);
                }
                else
                {
                   // MessageBox.Show(FileNameNoEXT);
                    StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                    DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                    DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                    IsDownloadRunning = false;
                    DownloadAudQuality = new AudQuality();
                    DownloadVidQuality = new VidQuality();
                    FileNameNoEXT = "";
                    MessageBox.Show("MP3 failed to download");
                }
            }
            else
            {
                if((DownloadVidQuality.Format=="mp4" && DownloadAudQuality.Format=="webm") ||(DownloadVidQuality.Format == "webm" && DownloadAudQuality.Format == "m4a"))
                {
                    if (DownloadVidQuality.Format == "mp4")
                    {
                        StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = "Converting to MP4"));
                        ConvertingThread = new Thread(() => ConvertFromMKV(FileNameNoEXT,true));
                        ConvertingThread.Start();
                    }
                    else
                    {
                        StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = "Converting to WEBM"));
                        ConvertingThread = new Thread(() => ConvertFromMKV(FileNameNoEXT, false));
                        ConvertingThread.Start();
                    }
                }
                else if (DownloadVidQuality.Format == "mp4" && DownloadAudQuality.Format == "m4a")
                {
                    if(!File.Exists(AppPath + "\\" + FileNameNoEXT + ".mp4"))
                    {
                        StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                        DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                        DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                        IsDownloadRunning = false;
                        DownloadAudQuality = new AudQuality();
                        DownloadVidQuality = new VidQuality();
                        FileNameNoEXT = "";
                        MessageBox.Show("Video failed to download");
                        return;
                    }
                    if (SaveLoc == "Default")
                    {
                        if(File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + FileNameNoEXT + ".mp4"))
                        {
                            MessageBoxResult result = MessageBox.Show("This video has been downloaded previously. Would you like to overwrite it?" + Environment.NewLine + "Otherwise its saved with a different filename", "Overwrite", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                            {
                                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + FileNameNoEXT + ".mp4");
                                File.Copy(AppPath + "\\" + FileNameNoEXT + ".mp4", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + FileNameNoEXT + ".mp4");
                                File.Delete(AppPath + "\\" + FileNameNoEXT + ".mp4");
                                StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                                DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                                DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                                if (UsePostDownloadCmd)
                                {
                                    Process.Start("cmd.exe", CmdText);
                                }
                                MessageBox.Show("Video successfully downloaded to My Documents");
                            }
                            else
                            {
                                File.Copy(AppPath + "\\" + FileNameNoEXT + ".mp4", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + FileNameNoEXT + " -NEW-" + DateTime.Now.Day + "-"+DateTime.Now.Month+"-"+DateTime.Now.Year+ " "+DateTime.Now.Hour+"-"+DateTime.Now.Minute+ ".mp4");
                                File.Delete(AppPath + "\\" + FileNameNoEXT + ".mp4");
                                StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                                DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                                DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                                if (UsePostDownloadCmd)
                                {
                                    Process.Start("cmd.exe", CmdText);
                                }
                                MessageBox.Show("Video successfully downloaded to My Documents");
                            }
                        }
                        else
                        {
                            File.Copy(AppPath + "\\" + FileNameNoEXT + ".mp4", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + FileNameNoEXT + ".mp4");
                            File.Delete(AppPath + "\\" + FileNameNoEXT + ".mp4");
                            StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                            DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                            DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                            if (UsePostDownloadCmd)
                            {
                                Process.Start("cmd.exe", CmdText);
                            }
                            MessageBox.Show("Video successfully downloaded to My Documents");
                        }
                    }
                    else
                    {
                        if (File.Exists(SaveLoc + "\\" + FileNameNoEXT + ".mp4"))
                        {
                            MessageBoxResult result = MessageBox.Show("This video has been downloaded previously. Would you like to overwrite it?" + Environment.NewLine + "Otherwise its saved with a different filename", "Overwrite", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                            {
                                File.Delete(SaveLoc + "\\" + FileNameNoEXT + ".mp4");
                                File.Copy(AppPath + "\\" + FileNameNoEXT + ".mp4", SaveLoc + "\\" + FileNameNoEXT + ".mp4");
                                File.Delete(AppPath + "\\" + FileNameNoEXT + ".mp4");
                                StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                                DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                                DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                                if (UsePostDownloadCmd)
                                {
                                    Process.Start("cmd.exe", CmdText);
                                }
                                MessageBox.Show("Video successfully downloaded");
                            }
                            else
                            {
                                File.Copy(AppPath + "\\" + FileNameNoEXT + ".mp4", SaveLoc + "\\" + FileNameNoEXT + " -NEW-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + ".mp4");
                                File.Delete(AppPath + "\\" + FileNameNoEXT + ".mp4");
                                StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                                DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                                DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                                if (UsePostDownloadCmd)
                                {
                                    Process.Start("cmd.exe", CmdText);
                                }
                                MessageBox.Show("Video successfully downloaded");
                            }
                        }
                        else
                        {
                            File.Copy(AppPath + "\\" + FileNameNoEXT + ".mp4", SaveLoc + "\\" + FileNameNoEXT + ".mp4");
                            File.Delete(AppPath + "\\" + FileNameNoEXT + ".mp4");
                            StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                            DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                            DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                            if (UsePostDownloadCmd)
                            {
                                Process.Start("cmd.exe", CmdText);
                            }
                            MessageBox.Show("Video successfully downloaded");
                        }
                    }
                    IsDownloadRunning = false;
                    DownloadAudQuality = new AudQuality();
                    DownloadVidQuality = new VidQuality();
                    FileNameNoEXT = "";
                }
                else if (DownloadVidQuality.Format == "webm" && DownloadAudQuality.Format == "webm")
                {
                    if (!File.Exists(AppPath + "\\" + FileNameNoEXT + ".webm"))
                    {
                        StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                        DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                        DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                        IsDownloadRunning = false;
                        DownloadAudQuality = new AudQuality();
                        DownloadVidQuality = new VidQuality();
                        FileNameNoEXT = "";
                        MessageBox.Show("Video failed to download");
                        return;
                    }
                    if (SaveLoc == "Default")
                    {
                        if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + FileNameNoEXT + ".webm"))
                        {
                            MessageBoxResult result = MessageBox.Show("This video has been downloaded previously. Would you like to overwrite it?" + Environment.NewLine + "Otherwise its saved with a different filename", "Overwrite", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                            {
                                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + FileNameNoEXT + ".webm");
                                File.Copy(AppPath + "\\" + FileNameNoEXT + ".webm", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + FileNameNoEXT + ".webm");
                                File.Delete(AppPath + "\\" + FileNameNoEXT + ".webm");
                                StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                                DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                                DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                                if (UsePostDownloadCmd)
                                {
                                    Process.Start("cmd.exe", CmdText);
                                }
                                MessageBox.Show("Video successfully downloaded to My Documents");
                            }
                            else
                            {
                                File.Copy(AppPath + "\\" + FileNameNoEXT + ".webm", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + FileNameNoEXT + " -NEW-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + ".webm");
                                File.Delete(AppPath + "\\" + FileNameNoEXT + ".webm");
                                StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                                DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                                DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                                if (UsePostDownloadCmd)
                                {
                                    Process.Start("cmd.exe", CmdText);
                                }
                                MessageBox.Show("Video successfully downloaded to My Documents");
                            }
                        }
                        else
                        {
                            File.Copy(AppPath + "\\" + FileNameNoEXT + ".webm", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + FileNameNoEXT + ".webm");
                            File.Delete(AppPath + "\\" + FileNameNoEXT + ".webm");
                            StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                            DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                            DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                            if (UsePostDownloadCmd)
                            {
                                Process.Start("cmd.exe", CmdText);
                            }
                            MessageBox.Show("Video successfully downloaded to My Documents");
                        }
                    }
                    else
                    {
                        if (File.Exists(SaveLoc + "\\" + FileNameNoEXT + ".webm"))
                        {
                            MessageBoxResult result = MessageBox.Show("This video has been downloaded previously. Would you like to overwrite it?" + Environment.NewLine + "Otherwise its saved with a different filename", "Overwrite", MessageBoxButton.YesNo, MessageBoxImage.Question);
                            if (result == MessageBoxResult.Yes)
                            {
                                File.Delete(SaveLoc + "\\" + FileNameNoEXT + ".webm");
                                File.Copy(AppPath + "\\" + FileNameNoEXT + ".webm", SaveLoc + "\\" + FileNameNoEXT + ".webm");
                                File.Delete(AppPath + "\\" + FileNameNoEXT + ".webm");
                                StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                                DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                                DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                                if (UsePostDownloadCmd)
                                {
                                    Process.Start("cmd.exe", CmdText);
                                }
                                MessageBox.Show("Video successfully downloaded");
                            }
                            else
                            {
                                File.Copy(AppPath + "\\" + FileNameNoEXT + ".webm", SaveLoc + "\\" + FileNameNoEXT + " -NEW-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + ".webm");
                                File.Delete(AppPath + "\\" + FileNameNoEXT + ".webm");
                                StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                                DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                                DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                                if (UsePostDownloadCmd)
                                {
                                    Process.Start("cmd.exe", CmdText);
                                }
                                MessageBox.Show("Video successfully downloaded");
                            }
                        }
                        else
                        {
                            File.Copy(AppPath + "\\" + FileNameNoEXT + ".webm", SaveLoc + "\\" + FileNameNoEXT + ".webm");
                            File.Delete(AppPath + "\\" + FileNameNoEXT + ".webm");
                            StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                            DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                            DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                            if (UsePostDownloadCmd)
                            {
                                Process.Start("cmd.exe", CmdText);
                            }
                            MessageBox.Show("Video successfully downloaded");
                        }
                    }
                    IsDownloadRunning = false;
                    DownloadAudQuality = new AudQuality();
                    DownloadVidQuality = new VidQuality();
                    FileNameNoEXT = "";
                }
            }
            
        }
        public void ConvertFromMKV(string filename, bool IsToMP4)
        {
            ConvertVid = new Process();
            ProcessStartInfo ConvertVidInfo = new ProcessStartInfo(AppPath + "\\ConvertDownload.bat");
            StreamWriter editBAt = new StreamWriter(AppPath + "\\ConvertDownload.bat");
            if (IsToMP4)
            {
                editBAt.WriteLine("ffmpeg -i " + '"' + FileNameNoEXT + ".mkv" + '"' +" " +'"' + FileNameNoEXT + ".mp4" + '"');
            }
            else
            {
                editBAt.WriteLine("ffmpeg -i " + '"' + FileNameNoEXT + ".mkv" + '"' + " " + '"' + FileNameNoEXT + ".webm" + '"');
            }
            editBAt.Close();
            ConvertVidInfo.UseShellExecute = false;
            ConvertVidInfo.RedirectStandardError = true;
            ConvertVidInfo.CreateNoWindow = true;
            ConvertVidInfo.WindowStyle = ProcessWindowStyle.Hidden;
            ConvertVid.StartInfo = ConvertVidInfo;
            ConvertVid.ErrorDataReceived += ConvertVid_ErrorDataReceived;
            ConvertVid.Start();
            ConvertVid.BeginErrorReadLine();
            ConvertVid.WaitForExit();
            ConvertVid.Close();
            if (IsToMP4)
            {
                if (File.Exists(AppPath + "\\" + FileNameNoEXT + ".mp4"))
                {
                    string savePath = "";
                    string message = "";
                    if (SaveLoc == "Default")
                    {
                        savePath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        message = " to My Documents";
                    }
                    else
                    {
                        savePath = SaveLoc;
                    }
                    if(File.Exists(savePath + "\\" + FileNameNoEXT + ".mp4"))
                    {
                        MessageBoxResult result = MessageBox.Show("This video has been downloaded previously. Would you like to overwrite it?" + Environment.NewLine + "Otherwise its saved with a different filename", "Overwrite", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            File.Delete(savePath + "\\" + FileNameNoEXT + ".mp4");
                            File.Copy(AppPath + "\\" + FileNameNoEXT + ".mp4", savePath + "\\" + FileNameNoEXT + ".mp4");
                            File.Delete(AppPath + "\\" + FileNameNoEXT + ".mp4");
                            File.Delete(AppPath + "\\" + FileNameNoEXT + ".mkv");
                            if (UsePostDownloadCmd)
                            {
                                Process.Start("cmd.exe", CmdText);
                            }
                        }
                        else
                        {
                            File.Copy(AppPath + "\\" + FileNameNoEXT + ".mp4", savePath + "\\" + FileNameNoEXT + " -NEW-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + ".mp4");
                            File.Delete(AppPath + "\\" + FileNameNoEXT + ".mp4");
                            File.Delete(AppPath + "\\" + FileNameNoEXT + ".mkv");
                            if (UsePostDownloadCmd)
                            {
                                Process.Start("cmd.exe", CmdText);
                            }
                        }
                    }
                    else
                    {
                        File.Copy(AppPath + "\\" + FileNameNoEXT + ".mp4", savePath + "\\" + FileNameNoEXT + ".mp4");
                        File.Delete(AppPath + "\\" + FileNameNoEXT + ".mp4");
                        File.Delete(AppPath + "\\" + FileNameNoEXT + ".mkv");
                        if (UsePostDownloadCmd)
                        {
                            Process.Start("cmd.exe", CmdText);
                        }
                    }
                    DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                    DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                    StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                    MessageBox.Show("Video downloaded successfully and saved " + message);
                }
                else
                {
                    DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                    DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                    StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                    MessageBox.Show("Failed to download video");
                }
            }
            else
            {
                if (File.Exists(AppPath + "\\" + FileNameNoEXT + ".webm"))
                {
                    string savePath = "";
                    string message = "";
                    if (SaveLoc == "Default")
                    {
                        savePath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        message = " to My Documents";
                    }
                    else
                    {
                        savePath = SaveLoc;
                    }
                    if (File.Exists(savePath + "\\" + FileNameNoEXT + ".webm"))
                    {
                        MessageBoxResult result = MessageBox.Show("This video has been downloaded previously. Would you like to overwrite it?" + Environment.NewLine + "Otherwise its saved with a different filename", "Overwrite", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            File.Delete(savePath + "\\" + FileNameNoEXT + ".webm");
                            File.Copy(AppPath + "\\" + FileNameNoEXT + ".webm", savePath + "\\" + FileNameNoEXT + ".webm");
                            File.Delete(AppPath + "\\" + FileNameNoEXT + ".webm");
                            File.Delete(AppPath + "\\" + FileNameNoEXT + ".mkv");
                            if (UsePostDownloadCmd)
                            {
                                Process.Start("cmd.exe", CmdText);
                            }
                        }
                        else
                        {
                            File.Copy(AppPath + "\\" + FileNameNoEXT + ".webm", savePath + "\\" + FileNameNoEXT + " -NEW-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + ".webm");
                            File.Delete(AppPath + "\\" + FileNameNoEXT + ".webm");
                            File.Delete(AppPath + "\\" + FileNameNoEXT + ".mkv");
                            if (UsePostDownloadCmd)
                            {
                                Process.Start("cmd.exe", CmdText);
                            }
                        }
                    }
                    else
                    {
                        File.Copy(AppPath + "\\" + FileNameNoEXT + ".webm", savePath + "\\" + FileNameNoEXT + ".webm");
                        File.Delete(AppPath + "\\" + FileNameNoEXT + ".webm");
                        File.Delete(AppPath + "\\" + FileNameNoEXT + ".mkv");
                        if (UsePostDownloadCmd)
                        {
                            Process.Start("cmd.exe", CmdText);
                        }
                    }
                    DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                    DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                    StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                    MessageBox.Show("Video downloaded successfully and saved " + message);
                }
                else
                {
                    DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = 0));
                    DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ""));
                    StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));
                    MessageBox.Show("Failed to download video");
                }
            }
            IsDownloadRunning = false;
            DownloadAudQuality = new AudQuality();
            DownloadVidQuality = new VidQuality();
            FileNameNoEXT = "";
        }

        private void ConvertVid_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            // throw new NotImplementedException();
            if (!string.IsNullOrEmpty(e.Data))
            {
                //MessageBox.Show(e.Data);
                if(e.Data.Contains("frame="))
                {
                    string[] sparts;
                    string line = e.Data;
                    line.Replace("frame=", "");
                    line = new Regex("frame=").Replace(line, "");
                    sparts = Regex.Split(line, "fps=");
                    DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = "Converting Video: frame " + sparts[0]));
                }
                
            }
        }

        private void VidDownloader_DownloadProgressChanged(DownloadProgress ProgData)
        {
            //throw new NotImplementedException();
            //MessageBox.Show(ProgData.ProgressInfo + " " + ProgData.ProgType.ToString() + " " + ProgData.DownType + " " + ProgData.ProgressPercentage + "%");
            if (ProgData.ProgType == ProgressType.ProgressChanged)
            {
                DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = ProgData.ProgressInfo));
                DownloadProgressBar.Dispatcher.Invoke(new Action(() => DownloadProgressBar.Value = ProgData.ProgressPercentageFromThousand));
            }
            else if(ProgData.ProgType == ProgressType.Converting)
            {
                DownloadStatus.Dispatcher.Invoke(new Action(() => DownloadStatus.Content = "Converting to MP3..."));
            }
            else if (ProgData.ProgType == ProgressType.FileNameGotten)
            {
                FileNameNoEXT = ProgData.ProgressInfo;
            }
        }

        public void SetVideoMetadata()
        {
            //StatusText.Content = "Status:Getting Metadata...";
            //StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = "Status: Getting Metadata"));
            StatusText.Content = "Status: Getting Metadata";
            //thumbnail.Source = back;
            //if()
            //  thumbnail.Dispatcher.Invoke(new Action(() =>
            //  BitmapImage back = new BitmapImage()
            //  back.BeginInit()
            //  back.UriSource = new Uri(@"Resources/blank.jpg", UriKind.RelativeOrAbsolute)
            //  back.EndInit()
            //thumbnail.Source=back))
            //thumbnail.Source.
           // if (File.Exists(AppPath + "\\thumbnail.jpg"))
           // {
           //     File.Delete(AppPath + "\\thumbnail.jpg");
           // }
            //    GetThumbnailInfo.UseShellExecute = false;

            /*    GetThumbnailInfo.Arguments = "--get-thumbnail " + VidURL;
                GetThumbnailInfo.CreateNoWindow = true;
                GetThumbnailInfo.WindowStyle = ProcessWindowStyle.Hidden;
                GetThumbnailInfo.RedirectStandardOutput = true;
                GetThumbnail.StartInfo = GetThumbnailInfo;
                GetThumbnail.Start();
                string ThumbURL = GetThumbnail.StandardOutput.ReadToEnd();
                GetThumbnail.WaitForExit();
                GetThumbnail.Close();
                string[] URLoutputs = Regex.Split(ThumbURL, "\n");
                ThumbURL = URLoutputs[0];
                //ThumbURL=ThumbURL.TrimEnd()
                MessageBox.Show("Starting pic download");
                MetadataHelper.DownloadFile("http://192.168.1.9/new_logo_label.png", "thumbnail.png");
                BitmapImage thumbnailPic = new BitmapImage();
                thumbnailPic.BeginInit();
                thumbnailPic.UriSource = new Uri(@"thumbnail.jpg", UriKind.RelativeOrAbsolute);
                thumbnailPic.EndInit();
                GetTitleInfo.CreateNoWindow = true;
                GetTitleInfo.Arguments = "--get-title " + VidURL;
                GetTitleInfo.UseShellExecute = false;
                GetTitleInfo.RedirectStandardOutput = true;
                GetTitleInfo.WindowStyle = ProcessWindowStyle.Hidden;
                GetTitle.StartInfo = GetTitleInfo;
                GetTitle.Start();
                VidTitleName = GetTitle.StandardOutput.ReadToEnd();
                GetTitle.WaitForExit();
                GetTitle.Close();
                VidTitle.Dispatcher.Invoke(new Action(() => VidTitle.Content=VidTitleName));
                StatusText.Dispatcher.Invoke(new Action(() => StatusText.Content = ""));*/
            MetadataScraper scraper = new MetadataScraper();
            VidInfo = scraper.GetMetadata(VidURL, true, AppPath + "\\thumbnail.jpg");
            VidTitle.Content = VidInfo.VidTitle;
            thumbnail.Source = null;
            BitmapImage thumbnailPic = new BitmapImage();
            thumbnailPic.BeginInit();
            thumbnailPic.UriSource = new Uri(VidInfo.ThumbnailURL, UriKind.RelativeOrAbsolute);
           // thumbnailPic.CacheOption = BitmapCacheOption.None;
            thumbnailPic.EndInit();
            thumbnail.Source = thumbnailPic;
            VidQualities = VidInfo.VidQualities;
            AudQualities = VidInfo.AudQualities;
            VidQualityBox.Items.Clear();
            foreach(VidQuality quality in VidQualities)
            {
                VidQualityBox.Items.Add(quality.Format + " - " + quality.Resolution);
            }
            VidQualityBox.Items.Add("MP3 Only");
            AudioQualityBox.Maximum = VidInfo.AudioCount;
            FileSizeLabel.Content = "Please select a video and audio quality";
            StatusText.Content = "";
            SelectedAQuality = "";
            SelectedVidQuality = new VidQuality();
            SelectedAudQuality = new AudQuality();
            //AudioQualityBox.
        }
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!firstMetadata)
            {
                //if (MetadataThread.ThreadState == System.Threading.ThreadState.Running)
                //{
                //    MessageBox.Show("Please wait for current video to be processed", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                //    return;
               // }
                
            }
            if (VidURLBox.Text.Contains("youtube.com/"))
            {
                VidURL = VidURLBox.Text;
                string[] URLoutput1 = Regex.Split(VidURL, "\n");
                VidURL = URLoutput1[0];
                if (firstMetadata)
                {
                    BitmapImage back = new BitmapImage();
                    back.BeginInit();
                    back.UriSource = new Uri(@"Resources/blank.jpg", UriKind.RelativeOrAbsolute);
                    back.EndInit();
                    thumbnail.Source = back;
                    VidQualityBox.IsEnabled = true;
                    AudioQualityBox.IsEnabled = true;
                    // MetadataThread = new Thread(new ThreadStart(SetVideoMetadata));
                    //MetadataThread.Start();
                    SetVideoMetadata();
                    firstMetadata = false;
                }
                else
                {
                    BitmapImage back = new BitmapImage();
                    back.BeginInit();
                    back.UriSource = new Uri(@"Resources/blank.jpg", UriKind.RelativeOrAbsolute);
                    back.EndInit();
                    thumbnail.Source = back;
                    SetVideoMetadata();
                    //MetadataThread = new Thread(new ThreadStart(SetVideoMetadata));
                   // MetadataThread.Start();
                }
            }
            else
            {
                MessageBox.Show("Not a valid YouTube URL", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void VidQualityBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // MessageBox.Show(SelectedAQuality);
            if (VidQualityBox.SelectedItem == null)
            {
                return;
            }
            if(VidQualityBox.SelectedItem.ToString()=="MP3 Only")
            {
                IsDownloadingMP3 = true;
                FileSizeLabel.Content = "Total File Size: " + AudQualities[AudQualities.Count - 1].FileSize + " (AUDIO ONLY)";
                return;
            }
            IsDownloadingMP3 = false;
            string[] output = Regex.Split(VidQualityBox.SelectedItem.ToString(), " - ");
            foreach(VidQuality quality in VidQualities)
            {
                if(quality.Resolution==output[1] && quality.Format == output[0])
                {
                    SelectedVidQuality = quality;
                }
            }
            SelectedAQuality = SelectedAQuality + "VIDEO";
            if(SelectedAQuality.Contains("AUDIO") && SelectedAQuality.Contains("VIDEO"))
            {
                FileSizeLabel.Content = "Total File Size: " + SelectedVidQuality.FileSize + " (VIDEO), " + SelectedAudQuality.FileSize + " (AUDIO)";
            }
        }

        private void AudioQualityBox_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(AudQualities.Count!= 0)
            {
                if (IsDownloadingMP3)
                {
                    return;
                }
               // MessageBox.Show(SelectedAQuality);
                int value = (int)AudioQualityBox.Value;
                SelectedAudQuality = AudQualities[value - 1];
                SelectedAQuality = SelectedAQuality + "AUDIO";
                if (SelectedAQuality.Contains("AUDIO") && SelectedAQuality.Contains("VIDEO"))
                {
                    FileSizeLabel.Content = "Total File Size: " + SelectedVidQuality.FileSize + " (VIDEO), " + SelectedAudQuality.FileSize + " (AUDIO)";
                }
            }
            
        }
    }
}
