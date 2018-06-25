Imports System.Net
Imports System.IO
Public Class Form2
    Dim WithEvents NewDownloader As New System.Net.WebClient
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Label1.Text = "Current version: " & Form1.CurrentVersion & " from " & Form1.CurrentVDate
        Label2.Text = "New version " & Form1.NewVersion & " from " & Form1.NewVDate
        If File.Exists(Application.StartupPath & "\lsyoutubedownload.exe") Then
            File.Delete(Application.StartupPath & "\lsyoutubedownload.exe")
        End If
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        'NewDownloader = New Net.WebClient()
        Dim DownloadUrl As Uri = New Uri("http://serverwebsite.ddns.net:500/youtubedownload/files/lsyoutubedownload.exe")
        AddHandler NewDownloader.DownloadProgressChanged, AddressOf DownloadProgressChanged
        AddHandler NewDownloader.DownloadFileCompleted, AddressOf DownloadCompleted
        NewDownloader.DownloadFileAsync(DownloadUrl, Application.StartupPath & "\lsyoutubedownload.exe")
    End Sub
    Public Sub DownloadProgressChanged(Sender As Object, e As DownloadProgressChangedEventArgs)
        ProgressBar1.Value = e.ProgressPercentage
    End Sub
    Public Sub DownloadCompleted()
        If File.Exists(Application.StartupPath & "\lsyoutubedownload.exe") Then
            Process.Start(Application.StartupPath & "\lsyoutubedownload.exe")
            Application.Exit()
        Else
            MsgBox("Error in downloading")
            Me.Close()
        End If
    End Sub
End Class