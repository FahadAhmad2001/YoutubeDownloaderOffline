using System;
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
using UpdatedUIApp.Resources;
using YTDownloadLib;
using System.IO;
using System.Threading;
using System.Reflection;
using UpdatedUIApp.Resources;
using UpdatedUIApp.ConfigReader;
using System.Text.RegularExpressions;

namespace UpdatedUIApp
{
    /// <summary>
    /// Interaction logic for PlaylistPage.xaml
    /// </summary>
    public partial class PlaylistPage : Page
    {
        YTDownloader downloader = new YTDownloader();
        ConfigData Cfgdata;
        public PlaylistPage()
        {
            InitializeComponent();
            
        }

        

        string playListURL;
        List<VideoQualityInfo> VidQualityInfoList = new List<VideoQualityInfo>();
        DownloadType CurrentDownloadType;
        string CurrentDownloadID = "";
        string FileName = "";
        bool IsCrossFormat=false;
        bool IsDefaultSaveLoc = false;
        bool IsdownloadingMP3 = false;
        string SaveLoc = "";
        int TotalVideos = 0;
        int CurrentVideo = 0;
        string Fileformat = "";
        bool IsUsingMP3Thumbnail = false;
        object ItemsControlLock;
        public string AppPath = Directory.GetCurrentDirectory();
        VidQuality CurrentVidQuality = new VidQuality();
        AudQuality CurrentAudQuality = new AudQuality();
        List<VideoQualityInfo> AllVideosInfoList = new List<VideoQualityInfo>();
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("DownloadPage.xaml", UriKind.Relative));
        }
        Thread ThumbnailThread;
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (URLBox.Text.Contains("https://www.youtube.com/playlist?list="))
            {
                playListURL = URLBox.Text;
                GetPlaylistMetadata(); //need to change this to a seperate thread
               // ThumbnailThread = new Thread(GetPlaylistMetadata);
               // ThumbnailThread.Start();
            }
            else
            {
                MessageBox.Show("Not a valid playlist URL", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        PlaylistMetadataScrape GlobalMetadataScrape;
        List<VideoMetadataDisplay> newList = new List<VideoMetadataDisplay>();
        private void GetPlaylistMetadata()
        {
            MetadataScraper scraper = new MetadataScraper();
            GlobalMetadataScrape = scraper.GetPlaylistMetadata(playListURL);
            ListOfVideos.Background.Opacity = 0.45;
            VidQualityInfoList = new List<VideoQualityInfo>();
            ItemsControlLock = new object();
            foreach (MetadataScrape scrape in GlobalMetadataScrape.VideoMetadataList)
            {
                VideoQualityInfo VidQualInfo = new VideoQualityInfo();
                VidQualInfo.HasSelectedAudio = false;
                VidQualInfo.HasSelectedVideo = false;
                VidQualInfo.IsMP3Only = false;
                VidQualInfo.SelectedAudQuality = 0;
                VidQualInfo.SelectedVidQuality = 0;
                VidQualInfo.VidTitle = scrape.VidTitle;
                VidQualInfo.VidID = scrape.VidID;
                VidQualInfo.AudQualityList = scrape.AudQualities;
                VidQualInfo.VidQualityList = scrape.VidQualities;
                List<string> QualitiesList = new List<string>();
                foreach(VidQuality quality in scrape.VidQualities)
                {
                    QualitiesList.Add(quality.Format + " - " + quality.Resolution + " (ID:" + quality.VidNo + ")");
                }
                QualitiesList.Add("MP3 Only");
                VidQualityInfoList.Add(VidQualInfo);
                newList.Add(new VideoMetadataDisplay() { VideoTitleText = scrape.VidTitle, VideoStatusText = "", VideoThumbnailURL = scrape.ThumbnailURL, VideoTotalSizeText = "Please select an audio and video quality", AudioCount = scrape.AudQualities.Count,VideoQualitiesList=QualitiesList });
            }
            ListOfVideos.ItemsSource = newList;
            TotalVideos = VidQualityInfoList.Count;
            BindingOperations.EnableCollectionSynchronization(newList, ItemsControlLock);
        }
        
        private void VideoComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //MessageBox.Show(ListOfVideos.Items.CurrentPosition.ToString());
            //from https://docs.microsoft.com/en-us/dotnet/framework/wpf/data/how-to-find-datatemplate-generated-elements
             ComboBox VideoComboBox = sender as ComboBox;
             UIElement CurrentItem = (UIElement)ListOfVideos.ItemContainerGenerator.ContainerFromItem(ListOfVideos.Items.Count-1);
             string videoTitleText="";
             string selectedItemText="";
             foreach (object item in ListOfVideos.Items)
             {
                 UIElement currentItem = (UIElement)ListOfVideos.ItemContainerGenerator.ContainerFromItem(item);
                 ContentPresenter currentContentPresenter = (ContentPresenter)currentItem;
                 DataTemplate currentDataTemplate = currentContentPresenter.ContentTemplate;
                 ComboBox itemComboBox = (ComboBox)currentDataTemplate.FindName("VideoComboBox", currentContentPresenter);
                 if (itemComboBox == VideoComboBox)
                 {
                     Label videoTitle = (Label)currentDataTemplate.FindName("VideoTitleLabel", currentContentPresenter);
                     videoTitleText = videoTitle.Content.ToString();
                    // MessageBox.Show(videoTitleText);
                     CurrentItem = currentItem;
                     selectedItemText = itemComboBox.SelectedItem.ToString();
                 }
             }
             if (CurrentItem != null) //This check needs to be replaced with a proper one that actually works. Also, need to change it to get the Comboboxes from the Utils class
             {
                 VideoQualityInfo CurrentVid = new VideoQualityInfo();
                 VideoQualityInfo CurrentVidUpdated = new VideoQualityInfo();
                 foreach(VideoQualityInfo EachVid in VidQualityInfoList)
                 {
                     if (videoTitleText == EachVid.VidTitle)
                     {
                         CurrentVid = EachVid;
                         CurrentVidUpdated = EachVid;
                     }
                 }
                 if(selectedItemText=="MP3 Only")
                 {
                     CurrentVidUpdated.IsMP3Only = true;

                 }
                 else
                 {
                     CurrentVidUpdated.IsMP3Only = false;
                     string[] output0 = Regex.Split(selectedItemText, "ID:");
                     string[] output1 = output0[1].Split(')');
                     CurrentVidUpdated.SelectedVidQuality = int.Parse(output1[0]);
                     CurrentVidUpdated.HasSelectedVideo = true;
                 }
                if (CurrentVidUpdated.IsMP3Only)
                {
                    if (CurrentItem.TryFindVisualChildByName("VideoFileSizeLabel", out Label label))
                    {
                        label.Content = "Total File Size: " + CurrentVidUpdated.AudQualityList[CurrentVidUpdated.AudQualityList.Count-1].FileSize + " (AUDIO ONLY)";
                    }
                }
                else
                {
                    if(CurrentVidUpdated.HasSelectedAudio && CurrentVidUpdated.HasSelectedVideo)
                    {
                        AudQuality SelectedAudQuality= new AudQuality();
                        VidQuality SelectedVidQuality= new VidQuality();
                        foreach(AudQuality quality in CurrentVidUpdated.AudQualityList)
                        {
                            if (quality.AudNo == CurrentVidUpdated.SelectedAudQuality.ToString())
                            {
                                SelectedAudQuality = quality;
                            }
                        }
                        foreach(VidQuality quality in CurrentVidUpdated.VidQualityList)
                        {
                            if (quality.VidNo == CurrentVidUpdated.SelectedVidQuality.ToString())
                            {
                                SelectedVidQuality = quality;
                            }
                        }
                        if ((SelectedAudQuality.Format == "webm" && SelectedVidQuality.Format == "mp4") || (SelectedAudQuality.Format == "m4a" && SelectedVidQuality.Format == "webm"))
                        {
                            CurrentVidUpdated.IsCrossFormat = true;
                        }
                        if (CurrentItem.TryFindVisualChildByName("VideoFileSizeLabel", out Label label))
                        {
                            label.Content = "Total File Size: " + SelectedVidQuality.FileSize + " (VIDEO) + " + SelectedAudQuality.FileSize + " (AUDIO)";
                        }
                    }
                }
                VidQualityInfoList.Remove(CurrentVid);
                VidQualityInfoList.Add(CurrentVidUpdated);
                AllVideosInfoList = VidQualityInfoList;
             }
             //ContentPresenter CurrentContentPresenter = /*FindVisualChild<ContentPresenter>(CurrentItem)*/ //(ContentPresenter)CurrentItem;
                                                                                                             /*DataTemplate CurrentDataTemplate = CurrentContentPresenter.ContentTemplate;
                                                                                                             Label VideoTitle = (Label)CurrentDataTemplate.FindName("VideoTitleLabel", CurrentContentPresenter);
                                                                                                             string VideoTitleText = VideoTitle.Content.ToString();
                                                                                                             MessageBox.Show(VideoTitleText);*/
           /* var listView = sender as ListView;
            object item = ListOfVideos.Items.CurrentItem;
            var itemContainer = ListOfVideos.ItemContainerGenerator.ContainerFromItem(item) as ListViewItem;

            if (itemContainer.TryFindVisualChildByName("VideoTitleLabel", out Label label))
            {
                var videoTitleText = label.Content as string;
                MessageBox.Show(videoTitleText);
            }*/
        }
       
        private void VideoAudioSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            UIElement CurrentItem = (UIElement)ListOfVideos.ItemContainerGenerator.ContainerFromItem(ListOfVideos.Items.Count - 1);
            string videoTitleText="";

            foreach (object item in ListOfVideos.Items)
            {
                UIElement currentItem = (UIElement)ListOfVideos.ItemContainerGenerator.ContainerFromItem(item);
                ContentPresenter currentContentPresenter = (ContentPresenter)currentItem;
                DataTemplate currentDataTemplate = currentContentPresenter.ContentTemplate;
                Slider itemSlider = (Slider)currentDataTemplate.FindName("VideoAudioSlider", currentContentPresenter);
                if (itemSlider == slider)
                {
                    Label videoTitle = (Label)currentDataTemplate.FindName("VideoTitleLabel", currentContentPresenter);
                    videoTitleText = videoTitle.Content.ToString();
                    CurrentItem = currentItem;
                }
            }
            VideoQualityInfo CurrentVid = new VideoQualityInfo();
            VideoQualityInfo CurrentVidUpdated = new VideoQualityInfo();
            foreach (VideoQualityInfo EachVid in VidQualityInfoList)
            {
                if (videoTitleText == EachVid.VidTitle)
                {
                    CurrentVid = EachVid;
                    CurrentVidUpdated = EachVid;
                }
            }
            if (CurrentVid.IsMP3Only)
            {
                return;
            }
            CurrentVidUpdated.HasSelectedAudio = true;
            int value = (int)slider.Value;
            CurrentVidUpdated.SelectedAudQuality = int.Parse(CurrentVidUpdated.AudQualityList[value - 1].AudNo);
            if(CurrentVidUpdated.HasSelectedAudio && CurrentVidUpdated.HasSelectedVideo)
            {
                AudQuality SelectedAudQuality = new AudQuality();
                VidQuality SelectedVidQuality = new VidQuality();
                foreach (AudQuality quality in CurrentVidUpdated.AudQualityList)
                {
                    if (quality.AudNo == CurrentVidUpdated.SelectedAudQuality.ToString())
                    {
                        SelectedAudQuality = quality;
                    }
                }
                foreach (VidQuality quality in CurrentVidUpdated.VidQualityList)
                {
                    if (quality.VidNo == CurrentVidUpdated.SelectedVidQuality.ToString())
                    {
                        SelectedVidQuality = quality;
                    }
                }
                if((SelectedAudQuality.Format=="webm"&&SelectedVidQuality.Format=="mp4") || (SelectedAudQuality.Format == "m4a" && SelectedVidQuality.Format == "webm"))
                {
                    CurrentVidUpdated.IsCrossFormat = true;
                }
                if (CurrentItem.TryFindVisualChildByName("VideoFileSizeLabel", out Label label))
                {
                    label.Content = "Total File Size: " + SelectedVidQuality.FileSize + " (VIDEO) + " + SelectedAudQuality.FileSize + " (AUDIO)";
                }
            }
            VidQualityInfoList.Remove(CurrentVid);
            VidQualityInfoList.Add(CurrentVidUpdated);
            AllVideosInfoList = VidQualityInfoList;
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (AllVideosInfoList.Count > 0)
            {
                CurrentVideo = CurrentVideo + 1;
                PlaylistStatus.Dispatcher.Invoke(new Action(() => PlaylistStatus.Content = "Status:["+CurrentVideo.ToString()+"/"+TotalVideos.ToString()+"] Downloading youtube.com/watch?v=" + AllVideosInfoList[0].VidID));
                if (AllVideosInfoList[0].IsMP3Only)
                {
                    IsdownloadingMP3 = true;
                    if (IsUsingMP3Thumbnail)
                    {
                        CurrentDownloadID = AllVideosInfoList[0].VidID;
                        CurrentDownloadType = DownloadType.MP3Pic;
                        downloader.DownloadVideo("https://www.youtube.com/watch?v=" + AllVideosInfoList[0].VidID, DownloadType.MP3Pic);
                    }
                    else
                    {
                        CurrentDownloadID = AllVideosInfoList[0].VidID;
                        CurrentDownloadType = DownloadType.MP3Only;
                        downloader.DownloadVideo("https://www.youtube.com/watch?v=" + AllVideosInfoList[0].VidID, DownloadType.MP3Only);
                    }
                }
                else
                {
                    IsdownloadingMP3 = false;
                    CurrentDownloadType = DownloadType.CustomQuality;
                    CurrentDownloadID = AllVideosInfoList[0].VidID;
                    VideoQualityInfo NewDownloadInfo = AllVideosInfoList[0];
                    AudQuality aQuality = new AudQuality();
                    aQuality.AudNo = NewDownloadInfo.SelectedAudQuality.ToString();
                    VidQuality vQuality = new VidQuality();
                    vQuality.VidNo = NewDownloadInfo.SelectedVidQuality.ToString();
                    foreach (VidQuality quality in NewDownloadInfo.VidQualityList)
                    {
                        if (quality.VidNo == vQuality.VidNo)
                        {
                            vQuality.Format = quality.Format;
                        }
                    }
                    foreach (AudQuality quality in NewDownloadInfo.AudQualityList)
                    {
                        if (quality.AudNo == aQuality.AudNo)
                        {
                            aQuality.Format = quality.Format;
                        }
                    }
                    IsCrossFormat = NewDownloadInfo.IsCrossFormat;
                    CurrentAudQuality = aQuality;
                    CurrentVidQuality = vQuality;
                    downloader.DownloadVideo("https://www.youtube.com/watch/?v=" + NewDownloadInfo.VidID, DownloadType.CustomQuality, aQuality, vQuality);
                }
            }
        }

        private void Downloader_DownloadProgressChanged(DownloadProgress ProgData)
        {
            if (ProgData.ProgType == ProgressType.ProgressChanged)
            {
                lock (ItemsControlLock)
                {
                    foreach (VideoQualityInfo info in AllVideosInfoList)
                    {
                        if (info.VidID == ProgData.VidID)
                        {
                          //  foreach (object item in ListOfVideos.Items)
                          //  {
                              //  UIElement currentItem = (UIElement)ListOfVideos.ItemContainerGenerator.ContainerFromItem(item);
                              //  ContentPresenter currentContentPresenter = (ContentPresenter)currentItem;
                              //  DataTemplate currentDataTemplate = currentContentPresenter.ContentTemplate;
                              //  if (currentItem.TryFindVisualChildByName("VideoProgressLabel", out Label label))
                              //  {
                              //      label.Content = "Status: " + ProgData.ProgressInfo;
                              //  }
                           // }
                           foreach(VideoMetadataDisplay display in newList)
                           {
                                if (display.VideoTitleText == info.VidTitle)
                                {
                                    display.VideoStatusText = "Status: " + ProgData.ProgressInfo;
                                }
                           }
                        }
                    }
                }
                PlaylistStatus.Dispatcher.Invoke(new Action(() => PlaylistStatus.Content = "Status: [" + CurrentVideo.ToString() + "/" + TotalVideos.ToString() + "] " + ProgData.ProgressInfo));
            }
            else if (ProgData.ProgType == ProgressType.FileNameGotten)
            {
                if (IsdownloadingMP3)
                {
                    FileName = ProgData.ProgressInfo;
                    Fileformat = "mp3";
                }
                else
                {
                    if (IsCrossFormat)
                    {
                        FileName = ProgData.ProgressInfo + ".mkv";
                        Fileformat = "mkv";
                    }
                    else
                    {
                        if (CurrentVidQuality.Format == "mp4")
                        {
                            FileName = ProgData.ProgressInfo + ".mp4";
                            Fileformat = "mp4";
                        }
                        else if (CurrentVidQuality.Format == "webm")
                        {
                            FileName = ProgData.ProgressInfo + ".webm";
                            Fileformat = "webm";
                        }
                    }
                }
            }
            else if (ProgData.ProgType == ProgressType.Converting)
            {
                PlaylistStatus.Dispatcher.Invoke(new Action(() => PlaylistStatus.Content = "Status: [" + CurrentVideo.ToString() + "/" + TotalVideos.ToString() + "] Converting to MP3..."));
            }
        }

        private void Downloader_DownloadCompleted(DownloadProgress ProgData)
        {
            string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (File.Exists(AppPath + "\\" + FileName))
            {
                if (IsDefaultSaveLoc)
                {
                    if (File.Exists(documents + "\\" + FileName))
                    {
                        MessageBoxResult reply = MessageBox.Show("File already exists in My documents, would you like to overwrite it?", "File exists", MessageBoxButton.YesNo, MessageBoxImage.Information);
                        if (reply == MessageBoxResult.Yes)
                        {
                            File.Delete(documents + "\\" + FileName);
                            File.Copy(AppPath + "\\" + FileName, documents + "\\" + FileName);
                            File.Delete(AppPath + "\\" + FileName);
                        }
                        else
                        {
                            File.Copy(AppPath + "\\" + FileName, documents + "\\" + FileName + "-NEW-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "." + Fileformat);
                            File.Delete(AppPath + "\\" + FileName);
                        }
                    }
                    else
                    {
                        File.Copy(AppPath + "\\" + FileName, documents + "\\" + FileName);
                        File.Delete(AppPath + "\\" + FileName);
                    }
                }
                else
                {
                    if (Directory.Exists(SaveLoc))
                    {
                        if (File.Exists(SaveLoc + "\\" + FileName))
                        {
                            MessageBoxResult result = MessageBox.Show("File already exists in save location, would you like to overwrite it?", "File exists", MessageBoxButton.YesNo, MessageBoxImage.Information);
                            if (result == MessageBoxResult.Yes)
                            {
                                File.Delete(SaveLoc + "\\" + FileName);
                                File.Copy(AppPath + "\\" + FileName, SaveLoc + "\\" + FileName);
                                File.Delete(AppPath + "\\" + FileName);
                            }
                            else
                            {
                                File.Copy(AppPath + "\\" + FileName, SaveLoc + "\\" + FileName + "-NEW-" + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + "." + Fileformat);
                                File.Delete(AppPath + "\\" + FileName);
                            }
                        }
                        else
                        {
                            File.Copy(AppPath + "\\" + FileName, SaveLoc + "\\" + FileName);
                            File.Delete(AppPath + "\\" + FileName);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Save directory could not be found, saving to My Documents", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        if(File.Exists(documents+"\\" + FileName))
                        {
                            MessageBoxResult reply = MessageBox.Show("File already exists in My documents, would you like to overwrite it?", "File exists", MessageBoxButton.YesNo, MessageBoxImage.Information);
                            if (reply == MessageBoxResult.Yes)
                            {
                                File.Delete(documents + "\\" + FileName);
                                File.Copy(AppPath + "\\" + FileName, documents + "\\" + FileName);
                                File.Delete(AppPath + "\\" + FileName);
                            }
                            else
                            {
                                File.Copy(AppPath + "\\" + FileName, documents + "\\" + FileName+ "-NEW-" +DateTime.Now.Day+"-"+DateTime.Now.Month+"-"+DateTime.Now.Year+" " +DateTime.Now.Hour+"-"+DateTime.Now.Minute+"-"+DateTime.Now.Second+"."+Fileformat);
                                File.Delete(AppPath + "\\" + FileName);
                            }
                        }
                        else
                        {
                            File.Copy(AppPath + "\\" + FileName, documents + "\\" + FileName);
                            File.Delete(AppPath + "\\" + FileName);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Failed to download the following video:\nhttps://www.youtube.com/watch/?v=" + ProgData.VidID+"\nfrom the playlist","Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
            VideoQualityInfo CurrentVid = new VideoQualityInfo();
            foreach(VideoQualityInfo info in AllVideosInfoList)
            {
                if (info.VidID == ProgData.VidID)
                {
                    CurrentVid = info;
                }
            }
            AllVideosInfoList.Remove(CurrentVid);
            if (AllVideosInfoList.Count > 0)
            {
                CurrentVideo = CurrentVideo + 1;
                PlaylistStatus.Dispatcher.Invoke(new Action(() => PlaylistStatus.Content = "Status: [" + CurrentVideo.ToString() + "/" + TotalVideos.ToString() + "] Downloading youtube.com/watch?v=" + AllVideosInfoList[0].VidID));
                downloader = new YTDownloader();
                downloader.DownloadCompleted += Downloader_DownloadCompleted;
                downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
                if (AllVideosInfoList[0].IsMP3Only)
                {
                    IsdownloadingMP3 = true;
                    if (IsUsingMP3Thumbnail)
                    {
                        CurrentDownloadID = AllVideosInfoList[0].VidID;
                        CurrentDownloadType = DownloadType.MP3Pic;
                        downloader.DownloadVideo("https://www.youtube.com/watch?v=" + AllVideosInfoList[0].VidID, DownloadType.MP3Pic);
                    }
                    else
                    {
                        CurrentDownloadID = AllVideosInfoList[0].VidID;
                        CurrentDownloadType = DownloadType.MP3Only;
                        downloader.DownloadVideo("https://www.youtube.com/watch?v=" + AllVideosInfoList[0].VidID, DownloadType.MP3Only);
                    }
                }
                else
                {
                    IsdownloadingMP3 = false;
                    CurrentDownloadType = DownloadType.CustomQuality;
                    CurrentDownloadID = AllVideosInfoList[0].VidID;
                    VideoQualityInfo NewDownloadInfo = AllVideosInfoList[0];
                    AudQuality aQuality = new AudQuality();
                    aQuality.AudNo = NewDownloadInfo.SelectedAudQuality.ToString();
                    VidQuality vQuality = new VidQuality();
                    vQuality.VidNo = NewDownloadInfo.SelectedVidQuality.ToString();
                    foreach(VidQuality quality in NewDownloadInfo.VidQualityList)
                    {
                        if (quality.VidNo == vQuality.VidNo)
                        {
                            vQuality.Format = quality.Format;
                        }
                    }
                    foreach(AudQuality quality in NewDownloadInfo.AudQualityList)
                    {
                        if (quality.AudNo == aQuality.AudNo)
                        {
                            aQuality.Format = quality.Format;
                        }
                    }
                    IsCrossFormat = NewDownloadInfo.IsCrossFormat;
                    CurrentAudQuality = aQuality;
                    CurrentVidQuality = vQuality;
                    downloader.DownloadVideo("https://www.youtube.com/watch/?v=" + NewDownloadInfo.VidID, DownloadType.CustomQuality, aQuality, vQuality);
                }
            }
            else
            {
                PlaylistStatus.Dispatcher.Invoke(new Action(() => PlaylistStatus.Content = "Status:"));
                MessageBox.Show("Completed playlist download", "Finished", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            downloader.DownloadCompleted += Downloader_DownloadCompleted;
            downloader.DownloadProgressChanged += Downloader_DownloadProgressChanged;
            if (!ProgramConfigReader.VerifyConfigExists() || !ProgramConfigReader.VerifyRunProgramExists())
            {
                MessageBox.Show("Config file cannot be loaded!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                Cfgdata = ProgramConfigReader.GetCurrentConfigData();
                if (Cfgdata.DownloadLocation == "Default")
                {
                    IsDefaultSaveLoc = true;
                }
                else
                {
                    IsDefaultSaveLoc = false;
                }
                SaveLoc = Cfgdata.DownloadLocation;
                if (Cfgdata.IsUsingThumbnail)
                {
                    IsUsingMP3Thumbnail = true;
                }
                else
                {
                    IsUsingMP3Thumbnail = false;
                }
            }
            
        }
    }
    public class VideoMetadataDisplay
    {
        public string VideoTitleText { get; set; }
        public int AudioCount { get; set; }
        public string VideoThumbnailURL { get; set; }
        public string VideoStatusText { get; set; }
        public string VideoTotalSizeText { get; set; }
        public List<string> VideoQualitiesList { get; set; }
    }
    public struct VideoQualityInfo
    {
        public string VidID;
        public bool HasSelectedAudio;
        public bool HasSelectedVideo;
        public int SelectedAudQuality;
        public int SelectedVidQuality;
        public bool IsMP3Only;
        public string VidTitle;
        public List<AudQuality> AudQualityList;
        public List<VidQuality> VidQualityList;
        public bool IsCrossFormat;
    }
}
