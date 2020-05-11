Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Public Class MetadataScraper
    Dim CurrentFolder As String = Directory.GetCurrentDirectory()
    Public Function GetThumbnail(ByVal url As String) As String
        Dim GetThumbProc As Process = New Process()
        Dim GetThumbInfo As ProcessStartInfo = New ProcessStartInfo(CurrentFolder & "\youtube-dl.exe")
        GetThumbInfo.UseShellExecute = False
        GetThumbInfo.CreateNoWindow = True
        GetThumbInfo.RedirectStandardOutput = True
        GetThumbInfo.WindowStyle = ProcessWindowStyle.Hidden
        GetThumbInfo.Arguments = "--get-thumbnail " & url
        GetThumbProc.StartInfo = GetThumbInfo
        GetThumbProc.Start()
        Dim output As String = GetThumbProc.StandardOutput.ReadToEnd()
        GetThumbProc.WaitForExit()
        GetThumbProc.Close()
        Return output
    End Function
    Public Sub DownloadThumbnail(ByVal url As String, ByVal overwrite As Boolean, ByVal filepath As String)
        Dim ThumbURL As String
        ThumbURL = GetThumbnail(url)
        If File.Exists(filepath) Then
            If overwrite Then
                File.Delete(filepath)
                My.Computer.Network.DownloadFile(ThumbURL, filepath)
            End If
        Else
            My.Computer.Network.DownloadFile(ThumbURL, filepath)
        End If
    End Sub
    Public Function GetTitle(ByVal url As String) As String
        Dim GetTitleProc As Process = New Process()
        Dim GetTitleInfo As ProcessStartInfo = New ProcessStartInfo("youtube-dl.exe")
        GetTitleInfo.UseShellExecute = False
        GetTitleInfo.CreateNoWindow = True
        GetTitleInfo.RedirectStandardOutput = True
        GetTitleInfo.WindowStyle = ProcessWindowStyle.Hidden
        GetTitleInfo.Arguments = "--get-title " & url
        GetTitleProc.StartInfo = GetTitleInfo
        GetTitleProc.Start()
        Dim output As String = GetTitleProc.StandardOutput.ReadToEnd()
        GetTitleProc.WaitForExit()
        GetTitleProc.Close()
        Return output
    End Function
    Public Function GetAudioQualities(ByVal url As String) As List(Of AudQuality)
        Dim GetAudQual As Process = New Process()
        Dim GetAudQualInfo As ProcessStartInfo = New ProcessStartInfo("youtube-dl.exe")
        GetAudQualInfo.UseShellExecute = False
        GetAudQualInfo.RedirectStandardOutput = True
        GetAudQualInfo.CreateNoWindow = True
        GetAudQualInfo.WindowStyle = ProcessWindowStyle.Hidden
        GetAudQualInfo.Arguments = "-F " & url
        GetAudQual.StartInfo = GetAudQualInfo
        GetAudQual.Start()
        Dim output As String
        output = GetAudQual.StandardOutput.ReadToEnd()
        GetAudQual.WaitForExit()
        GetAudQual.Close()
        Dim output1() As String
        output1 = Regex.Split(output, "note")
        Dim output2() As String
        output2 = output1(1).Split(Chr(10))
        Dim list As List(Of AudQuality) = New List(Of AudQuality)
        For Each item As String In output2
            If item.Contains("audio only") Then
                Dim CurrentAudio As AudQuality = New AudQuality()
                Dim output3() As String
                output3 = Regex.Split(item, "          ")
                CurrentAudio.AudNo = output3(0)
                If item.Contains("m4a") Then
                    CurrentAudio.Format = "m4a"
                ElseIf item.Contains("webm") Then
                    CurrentAudio.Format = "webm"
                End If
                Dim output4() As String
                output4 = Regex.Split(item, "Hz\), ")
                CurrentAudio.FileSize = output4(1)
                list.Add(CurrentAudio)
            End If
        Next
        Return list
    End Function
    Public Function GetVideoQualities(ByVal url As String) As List(Of VidQuality)
        Dim GetVidQual As Process = New Process()
        Dim GetVidQualInfo As ProcessStartInfo = New ProcessStartInfo("youtube-dl.exe")
        GetVidQualInfo.UseShellExecute = False
        GetVidQualInfo.RedirectStandardOutput = True
        GetVidQualInfo.CreateNoWindow = True
        GetVidQualInfo.WindowStyle = ProcessWindowStyle.Hidden
        GetVidQualInfo.Arguments = "-F " & url
        GetVidQual.StartInfo = GetVidQualInfo
        GetVidQual.Start()
        Dim output As String
        output = GetVidQual.StandardOutput.ReadToEnd()
        GetVidQual.WaitForExit()
        GetVidQual.Close()
        Dim output1() As String
        output1 = Regex.Split(output, "note")
        Dim output2() As String
        output2 = output1(1).Split(Chr(10))
        Dim list As List(Of VidQuality) = New List(Of VidQuality)
        For Each item As String In output2
            If item.Contains("video only") Then
                Dim CurrentVideo As VidQuality = New VidQuality()
                ' Dim TempCount As Integer
                ' TempCount = AudioCount + 1
                Dim output3() As String
                output3 = Regex.Split(item, "          ")
                CurrentVideo.VidNo = output3(0)
                If item.Contains("mp4") Then
                    CurrentVideo.Format = "mp4"
                ElseIf item.Contains("webm") Then
                    CurrentVideo.Format = "webm"
                End If
                Dim output4() As String
                output4 = Regex.Split(item, "k , ")
                Dim output5() As String
                output5 = Regex.Split(output4(0), "x")
                Dim output7() As String
                output7 = Regex.Split(output5(1), Chr(32))
                'CurrentVidRes = output7(1)
                If output5(1).Contains("144p") Then
                    CurrentVideo.Resolution = "144p"
                ElseIf output5(1).Contains("240p") Then
                    CurrentVideo.Resolution = "240p"
                ElseIf output5(1).Contains("360p") Then
                    CurrentVideo.Resolution = "360p"
                ElseIf output5(1).Contains("480p") Then
                    CurrentVideo.Resolution = "480p"
                ElseIf output5(1).Contains("720p") Then
                    CurrentVideo.Resolution = "720p"
                ElseIf output5(1).Contains("1080p") Then
                    CurrentVideo.Resolution = "1080p"
                End If
                'MsgBox(CurrentVidRes)
                Dim output6() As String
                output6 = Regex.Split(item, "only, ")
                CurrentVideo.FileSize = output6(1)
                list.Add(CurrentVideo)
            End If
        Next
        Return list
    End Function
    Public Function GetMetadata(ByVal url As String, Optional ByVal downloadThumbnailPic As Boolean = False, Optional ByVal filepath As String = "") As MetadataScrape
        Dim scrape As MetadataScrape = New MetadataScrape()
        scrape.ThumbnailURL = GetThumbnail(url)
        If downloadThumbnailPic Then
            DownloadThumbnail(url, True, filepath)
        End If
        scrape.VidURL = url
        scrape.VidTitle = GetTitle(url)
        Dim vidlist As List(Of VidQuality) = New List(Of VidQuality)
        vidlist = GetVideoQualities(url)
        Dim audlist As List(Of AudQuality) = New List(Of AudQuality)
        audlist = GetAudioQualities(url)
        scrape.AudQualities = audlist
        scrape.AudioCount = audlist.Count
        scrape.VidQualities = vidlist
        scrape.VideoCount = vidlist.Count
        Return scrape
    End Function
End Class
Public Structure VidQuality
    Dim Resolution As String
    Dim VidNo As String
    Dim FileSize As String
    Dim Format As String
End Structure
Public Structure AudQuality
    Dim AudNo As String
    Dim FileSize As String
    Dim Format As String
End Structure
Public Structure MetadataScrape
    Dim VidURL As String
    Dim VidQualities As List(Of VidQuality)
    Dim AudQualities As List(Of AudQuality)
    Dim ThumbnailURL As String
    Dim VidTitle As String
    Dim AudioCount As Integer
    Dim VideoCount As Integer
End Structure

