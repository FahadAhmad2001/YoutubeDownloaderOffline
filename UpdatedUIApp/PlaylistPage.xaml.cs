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
using System.Text.RegularExpressions;

namespace UpdatedUIApp
{
    /// <summary>
    /// Interaction logic for PlaylistPage.xaml
    /// </summary>
    public partial class PlaylistPage : Page
    {
        
        public PlaylistPage()
        {
            InitializeComponent();
        }
        string playListURL;
        List<VideoQualityInfo> VidQualityInfoList = new List<VideoQualityInfo>();
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
        private void GetPlaylistMetadata()
        {
            MetadataScraper scraper = new MetadataScraper();
            GlobalMetadataScrape = scraper.GetPlaylistMetadata(playListURL);
            List<VideoMetadataDisplay> newList = new List<VideoMetadataDisplay>();
            ListOfVideos.Background.Opacity = 0.45;
            VidQualityInfoList = new List<VideoQualityInfo>();
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
                        if(CurrentItem.TryFindVisualChildByName("VideoFileSizeLabel", out Label label))
                        {
                            label.Content = "Total File Size: " + SelectedVidQuality.FileSize + " (VIDEO) + " + SelectedAudQuality.FileSize + " (AUDIO)";
                        }
                    }
                }
                VidQualityInfoList.Remove(CurrentVid);
                VidQualityInfoList.Add(CurrentVidUpdated);
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
                if (CurrentItem.TryFindVisualChildByName("VideoFileSizeLabel", out Label label))
                {
                    label.Content = "Total File Size: " + SelectedVidQuality.FileSize + " (VIDEO) + " + SelectedAudQuality.FileSize + " (AUDIO)";
                }
            }
            VidQualityInfoList.Remove(CurrentVid);
            VidQualityInfoList.Add(CurrentVidUpdated);
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
    }
}
