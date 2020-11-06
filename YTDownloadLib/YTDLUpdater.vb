Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Net
Public Class YTDLUpdater
    Dim UpdateYT As Process = New Process()
    Dim UpdateYTInfo As ProcessStartInfo = New ProcessStartInfo(Directory.GetCurrentDirectory() & "\youtube-dl.exe")
    Public Event UpdatingDownloader(ByVal version As String)
    Public Event DownloaderUpdateError(ByVal message As String)
    Dim LatestVersion As String
    Public Sub CheckYTDLUpdates()
        If File.Exists(Directory.GetCurrentDirectory & "\youtube-dlc.exe") Then
            ServicePointManager.Expect100Continue = True
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls Or SecurityProtocolType.Ssl3 Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12
            Dim wclient As WebClient
            wclient = New WebClient()
            wclient.Headers.Add("user-agent", "YTDownloadLib-Updater")
            Dim LatestVersionData As String
            LatestVersionData = wclient.DownloadString("https://api.github.com/repos/blackjack4494/yt-dlc/releases/latest")
            Dim output1() As String
            output1 = Regex.Split(LatestVersionData, "tag_name" & Chr(34) & ":" & Chr(34))
            Dim output2() As String
            output2 = Regex.Split(output1(1), Chr(34) & ",")
            LatestVersion = output2(0)
            Dim GetCurrentVersion As Process = New Process()
            Dim GetCurrentVersionInfo As ProcessStartInfo = New ProcessStartInfo(Directory.GetCurrentDirectory & "\youtube-dlc.exe")
            GetCurrentVersionInfo.Arguments = "--version"
            GetCurrentVersionInfo.UseShellExecute = False
            GetCurrentVersionInfo.RedirectStandardOutput = True
            GetCurrentVersionInfo.CreateNoWindow = True
            GetCurrentVersionInfo.WindowStyle = ProcessWindowStyle.Hidden
            GetCurrentVersion.StartInfo = GetCurrentVersionInfo
            Dim CurrentVersionOutput As String
            GetCurrentVersion.Start()
            CurrentVersionOutput = GetCurrentVersion.StandardOutput.ReadToEnd()
            GetCurrentVersion.WaitForExit()
            GetCurrentVersion.Close()
            CurrentVersionOutput = CurrentVersionOutput.TrimEnd(vbCr, vbLf, vbCrLf)
            If CurrentVersionOutput = LatestVersion Then

            Else
                RaiseEvent UpdatingDownloader(LatestVersion)
                UpdateYTDL()
            End If
        Else
            UpdateYTDL()
        End If
    End Sub
    Public Sub UpdateYTDL()
        Try
            If File.Exists(Directory.GetCurrentDirectory & "\youtube-dlc.exe") Then
                File.Delete(Directory.GetCurrentDirectory & "\youtube-dlc.exe")
            End If
            ServicePointManager.Expect100Continue = False
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls Or SecurityProtocolType.Ssl3 Or SecurityProtocolType.Tls11 Or SecurityProtocolType.Tls12
            Dim wclient As WebClient = New WebClient()
            wclient.Headers.Add("user-agent", "YTDownloadLib-Updater")
            wclient.DownloadFile("https://github.com/blackjack4494/yt-dlc/releases/latest/download/youtube-dlc.exe", Directory.GetCurrentDirectory & "\youtube-dlc.exe")
            If File.Exists(Directory.GetCurrentDirectory & "\youtube-dlc.exe") Then

            Else
                Throw New Exception("Failed to update youtube-dlc")
            End If
        Catch ex As Exception
            RaiseEvent DownloaderUpdateError(ex.ToString())
        End Try

    End Sub
    Private Sub OutputData(sendingProcess As Object, output As DataReceivedEventArgs)
        If Not String.IsNullOrEmpty(output.Data) Then
            If output.Data.Contains("Updating to version ") Then
                Dim output1() As String
                output1 = Regex.Split(output.Data, " version ")
                RaiseEvent UpdatingDownloader(output1(1))
            End If
        End If
    End Sub
End Class
