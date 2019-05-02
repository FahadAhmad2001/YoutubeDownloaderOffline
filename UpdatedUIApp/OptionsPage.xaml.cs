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
using Winforms = System.Windows.Forms;
using UpdatedUIApp.ConfigReader;
using System.Diagnostics;
using System.IO;

namespace UpdatedUIApp
{
    /// <summary>
    /// Interaction logic for OptionsPage.xaml
    /// </summary>
    public partial class OptionsPage : Page
    {
        public OptionsPage()
        {
            InitializeComponent();
        }
        public string SavePath = "Default";
        public bool MP3Thumbnail = false;
        public bool AutoUpdate = false;
        public bool UsingCmdAfterDwnld = false;
        public string CmdText = "";
        public string CurrentVersion="";
        ProgramUpdater updater;
        private void ChangeSavePath_Click(object sender, RoutedEventArgs e)
        {
            Winforms.FolderBrowserDialog dialog = new Winforms.FolderBrowserDialog();
            Winforms.DialogResult result = dialog.ShowDialog();
            if (result == Winforms.DialogResult.OK)
            {
                SavePath = dialog.SelectedPath;
                DownloadLocationString.Content = SavePath;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(!ProgramConfigReader.VerifyConfigExists() || !ProgramConfigReader.VerifyRunProgramExists())
            {
                MessageBox.Show("Config file cannot be loaded!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                ConfigData data = ProgramConfigReader.GetCurrentConfigData();
                SavePath = data.DownloadLocation;
                CurrentVersion = data.CurrentVersion;
                MP3Thumbnail = data.IsUsingThumbnail;
                AutoUpdate = data.IsAutoCheckUpdate;
                UsingCmdAfterDwnld = data.IsUsingProgAfterDownload;
                CmdText = data.ProgAfterDownloadText;
            }
            VersionText.Content = "Current Version: " + CurrentVersion;
            UpdateCheckBox.IsChecked = AutoUpdate;
            if (SavePath == "Default")
            {
                DownloadLocationString.Content = "My Documents";
            }
            else
            {
                DownloadLocationString.Content = SavePath;
            }
            MP3Checkbox.IsChecked = MP3Thumbnail;
            ProgramCheckbox.IsChecked = UsingCmdAfterDwnld;
            ProgramContent.Text = CmdText;
        }

        private void UpdateCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            AutoUpdate = true;
        }

        private void UpdateCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            AutoUpdate = false;
        }

        private void ProgramCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            UsingCmdAfterDwnld = true;
        }

        private void ProgramCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            UsingCmdAfterDwnld = false;
        }

        private void MP3Checkbox_Checked(object sender, RoutedEventArgs e)
        {
            MP3Thumbnail = true;
        }

        private void MP3Checkbox_Unchecked(object sender, RoutedEventArgs e)
        {
            MP3Thumbnail = false;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            ConfigData data = new ConfigData();
            data.CurrentVersion = CurrentVersion;
            data.DownloadLocation = SavePath;
            data.IsAutoCheckUpdate = AutoUpdate;
            data.IsUsingProgAfterDownload = UsingCmdAfterDwnld;
            data.IsUsingThumbnail = MP3Thumbnail;
            data.ProgAfterDownloadText = CmdText;
            ProgramConfigReader.WriteConfigData(data);
            MessageBox.Show("Successfully saved", "Options", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ProgramContent_TextChanged(object sender, TextChangedEventArgs e)
        {
            CmdText = ProgramContent.Text;
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("yo");
            //DownloadProgBar.Value = 100;
            VersionInfo info = ProgramConfigReader.GetLatestVersion();
            if(info.NewVersion=="NOT AVAILABLE")
            {
                MessageBox.Show("Cannot contact update server", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            if (info.NewVersion == CurrentVersion)
            {
                MessageBox.Show("Program up to date", "Update Checker", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("New version, " + info.NewVersion + " available from " + info.VersionDate + Environment.NewLine + "Would you like to update?", "Update Available", MessageBoxButton.YesNo, MessageBoxImage.Information);
                if (result == MessageBoxResult.Yes)
                {
                    UpdateButton.Content = "Downloading...";
                    updater = new ProgramUpdater();
                    updater.DownloadProgressUpdated += Updater_DownloadProgressUpdated;
                    updater.DownloadCompleted += Updater_DownloadCompleted;
                    updater.Update();
                }
            }
        }

        private void Updater_DownloadCompleted(object sender, DownloadComplete e)
        {
            //throw new NotImplementedException();
            // MessageBox.Show(e.IsSuccessful.ToString());
            if (e.IsSuccessful)
            {
                Process.Start(Directory.GetCurrentDirectory() + "\\lsyt.exe");
                System.Windows.Application.Current.Shutdown();
            }
            else
            {
                DownloadProgBar.Value = 0;
                UpdateButton.Content = "Check for updates";
                MessageBox.Show("New version failed to download", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Updater_DownloadProgressUpdated(object sender, DownloadProgChanged e)
        {
            //throw new NotImplementedException();
            DownloadProgBar.Value = e.DownloadPercentage;
        }
    }
}
