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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (URLBox.Text.Contains("https://www.youtube.com/playlist?list="))
            {
                playListURL = URLBox.Text;
                
            }
            else
            {
                MessageBox.Show("Not a valid playlist URL", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
