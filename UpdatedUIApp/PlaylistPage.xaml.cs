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
            foreach (MetadataScrape scrape in GlobalMetadataScrape.VideoMetadataList)
            {
                List<string> QualitiesList = new List<string>();
                foreach(VidQuality quality in scrape.VidQualities)
                {
                    QualitiesList.Add(quality.Format + " - " + quality.Resolution + " (ID:" + quality.VidNo + ")");
                }
                newList.Add(new VideoMetadataDisplay() { VideoTitleText = scrape.VidTitle, VideoStatusText = "", VideoThumbnailURL = scrape.ThumbnailURL, VideoTotalSizeText = "Please select an audio and video quality", AudioCount = scrape.AudQualities.Count,VideoQualitiesList=QualitiesList });
            }
            ListOfVideos.ItemsSource = newList;
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
}
