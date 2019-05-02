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
using System.Reflection;
using System.IO;
using YTDownloadLib;
using UpdatedUIApp.ConfigReader;

namespace UpdatedUIApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        public DownloadPage DownPage = new DownloadPage();
        public OptionsPage OptPage = new OptionsPage();
        private void Background_Loaded(object sender, RoutedEventArgs e)
        {
            BitmapImage b = new BitmapImage();
            b.BeginInit();
            b.UriSource = new Uri(@"Resources/back.jpg", UriKind.RelativeOrAbsolute);
            b.DecodePixelHeight = 420;
            b.DecodePixelWidth = 792;
            b.EndInit();
            background.Source = b;
            
            //background
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("yo");
            downloadButton.FontWeight = FontWeights.Bold;
            optionsButton.FontWeight = FontWeights.Normal;
            Mainframe.Navigate(new Uri("DownloadPage.xaml", UriKind.Relative));
        }

        private void OptionsButton_Click(object sender, RoutedEventArgs e)
        {
            downloadButton.FontWeight = FontWeights.Normal;
            optionsButton.FontWeight = FontWeights.Bold;
            Mainframe.Navigate(new Uri("OptionsPage.xaml", UriKind.Relative));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            YTDLUpdater updater = new YTDLUpdater();
            updater.UpdatingDownloader += Updater_UpdatingDownloader;
            updater.UpdateYTDL();
            if (ProgramConfigReader.GetCurrentConfigData().IsAutoCheckUpdate)
            {
                VersionInfo info = ProgramConfigReader.GetLatestVersion();
                if ((info.NewVersion != "NOT AVAILABLE") && (info.NewVersion != ProgramConfigReader.GetCurrentConfigData().CurrentVersion))
                {
                    MessageBoxResult result = MessageBox.Show("New version available, " + info.NewVersion + " from " + info.VersionDate + Environment.NewLine + "Would you like to update?", "Update Available", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (result == MessageBoxResult.Yes)
                    {
                        Mainframe.Navigate(new Uri("OptionsPage.xaml", UriKind.Relative));
                        return;
                    }
                }
            }
            Mainframe.Navigate(new Uri("DownloadPage.xaml", UriKind.Relative));
        }
        private void Updater_UpdatingDownloader(string version)
        {
            // throw new NotImplementedException();
            MessageBox.Show("Please wait a moment,\nUpdating downloader to version " + version);
        }
    }
}
