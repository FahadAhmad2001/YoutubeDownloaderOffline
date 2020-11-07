Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Public Class MetadataScraper
    Dim CurrentFolder As String = Directory.GetCurrentDirectory()
    Public Function GetThumbnail(ByVal url As String) As String
        Dim GetThumbProc As Process = New Process()
        Dim GetThumbInfo As ProcessStartInfo = New ProcessStartInfo(CurrentFolder & "\youtube-dlc.exe")
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
        Dim GetTitleInfo As ProcessStartInfo = New ProcessStartInfo("youtube-dlc.exe")
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
        Dim GetAudQualInfo As ProcessStartInfo = New ProcessStartInfo("youtube-dlc.exe")
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
                If output4.Count > 1 Then
                    CurrentAudio.FileSize = output4(1)
                Else
                    CurrentAudio.FileSize = "Unknown size"
                End If
                list.Add(CurrentAudio)
            End If
        Next
        Return list
    End Function
    Public Function GetVideoQualities(ByVal url As String) As List(Of VidQuality)
        Dim GetVidQual As Process = New Process()
        Dim GetVidQualInfo As ProcessStartInfo = New ProcessStartInfo("youtube-dlc.exe")
        GetVidQualInfo.UseShellExecute = False
        GetVidQualInfo.RedirectStandardOutput = True
        GetVidQualInfo.CreateNoWindow = True
        GetVidQualInfo.WindowStyle = ProcessWindowStyle.Hidden
        GetVidQualInfo.Arguments = "-F " & url
        GetVidQual.StartInfo = GetVidQualInfo
        GetVidQual.Start()
        Dim output As String
        Dim error1 As String
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
                If output6.Count > 1 Then
                    CurrentVideo.FileSize = output6(1)
                Else
                    CurrentVideo.FileSize = "Unknown size"
                End If
                list.Add(CurrentVideo)
            End If
        Next
        Return list
    End Function

    Function VideoQualityParser(ByVal text As String) As List(Of VidQuality)

    End Function

    Public Function GetPlaylistMetadata(ByVal playlistURL As String) As PlaylistMetadataScrape
        Dim list As List(Of MetadataScrape) = New List(Of MetadataScrape)
        Dim GetVidList As Process = New Process()
        Dim GetVidListInfo As ProcessStartInfo = New ProcessStartInfo("youtube-dlc.exe")
        GetVidListInfo.UseShellExecute = False
        GetVidListInfo.RedirectStandardOutput = True
        GetVidListInfo.CreateNoWindow = True
        GetVidListInfo.WindowStyle = ProcessWindowStyle.Hidden
        GetVidListInfo.Arguments = "-j --flat-playlist --skip-download " & playlistURL
        GetVidList.StartInfo = GetVidListInfo
        GetVidList.Start()
        Dim output As String
        output = GetVidList.StandardOutput.ReadToEnd()
        GetVidList.WaitForExit()
        GetVidList.Close()
        Dim GetVidQual As Process = New Process()
        Dim GetVidQualInfo As ProcessStartInfo = New ProcessStartInfo("youtube-dlc.exe")
        GetVidQualInfo.UseShellExecute = False
        GetVidQualInfo.RedirectStandardOutput = True
        GetVidQualInfo.CreateNoWindow = True
        GetVidQualInfo.WindowStyle = ProcessWindowStyle.Hidden
        GetVidQualInfo.Arguments = "-F --skip-download " & playlistURL
        GetVidQual.StartInfo = GetVidQualInfo
        GetVidQual.Start()
        Dim QualOutput As String
        QualOutput = GetVidQual.StandardOutput.ReadToEnd()
        GetVidQual.WaitForExit()
        GetVidQual.Close()
        Console.WriteLine(QualOutput)
        Dim GetVidThumbnail As Process = New Process()
        Dim GetVidThumbnailInfo As ProcessStartInfo = New ProcessStartInfo("youtube-dlc.exe")
        GetVidThumbnailInfo.UseShellExecute = False
        GetVidThumbnailInfo.RedirectStandardOutput = True
        GetVidThumbnailInfo.CreateNoWindow = True
        GetVidThumbnailInfo.WindowStyle = ProcessWindowStyle.Hidden
        GetVidThumbnailInfo.Arguments = "--get-thumbnail --skip-download " & playlistURL
        GetVidThumbnail.StartInfo = GetVidThumbnailInfo
        GetVidThumbnail.Start()
        Dim ThumbnailOutput As String
        ThumbnailOutput = GetVidThumbnail.StandardOutput.ReadToEnd()
        GetVidThumbnail.WaitForExit()
        GetVidThumbnail.Close()
        'MsgBox(output)
        Dim output1() As String
        output1 = output.Split(vbLf)
        Dim ThumbnailSplit() As String
        ThumbnailSplit = ThumbnailOutput.Split(vbLf)
        Dim QualitiesSplit() As String
        QualitiesSplit = Regex.Split(QualOutput, " Available formats for ")
        For Each item As String In output1
            If item.Contains("id") Then
                Dim item1 As String
                item1 = item.TrimStart("{", Chr(34))
                item1 = item1.TrimEnd(Chr(34), "}")
                'MsgBox(item1)
                Dim NewScrape As MetadataScrape = New MetadataScrape()
                Dim output2() As String
                '  output2 = Regex.Split(item, Chr(34) & ", " & Chr(34) & "ie_key" & Chr(34))
                '  Dim output3() As String
                '  output3 = Regex.Split(output2(0), Chr(34) & "id" & Chr(34) & ": " & Chr(34))
                '  NewScrape.VidID = output3(1)
                '  NewScrape.VidURL = "https://www.youtube.com/watch?v=" & NewScrape.VidID
                output2 = Regex.Split(item1, Chr(34) & ", " & Chr(34))
                For Each result As String In output2
                    If result.Contains("title" & Chr(34) & ": " & Chr(34)) Then
                        Dim output3() As String
                        output3 = Regex.Split(result, "itle" & Chr(34) & ": " & Chr(34))
                        NewScrape.VidTitle = output3(1)
                    ElseIf result.Contains("id" & Chr(34) + ": " + Chr(34)) Then
                        Dim output3() As String
                        output3 = Regex.Split(result, "d" & Chr(34) + ": " + Chr(34))
                        NewScrape.VidID = output3(1)
                        NewScrape.VidURL = "https://www.youtube.com/watch?v=" + NewScrape.VidID
                    End If
                Next
                For Each url As String In ThumbnailSplit
                    If url.Contains(NewScrape.VidID) Then
                        NewScrape.ThumbnailURL = url
                        Exit For
                    End If
                Next
                '   Dim output4() As String
                '   output4 = Regex.Split(output2(0), Chr(34) & ", " & Chr(34) & "title" & Chr(34) & ": " & Chr(34))
                '   Dim output5() As String
                '   output5 = Regex.Split(output4(0), Chr(34) & ", " & Chr(34) & "_type" & Chr(34))
                '   NewScrape.VidTitle = output5(0)
                Dim FoundQuality As Boolean = False
                For Each quality As String In QualitiesSplit
                    If quality.Contains(NewScrape.VidID) And (quality.Contains(NewScrape.VidID & ": Downloading webpage") = False) Then
                        FoundQuality = True
                        'MsgBox(quality)
                        Dim output6() As String
                        output6 = Regex.Split(quality, "resolution note")
                        Dim output7() As String
                        output7 = output6(1).Split(vbLf)
                        Dim AllVideoQualities As List(Of VidQuality) = New List(Of VidQuality)
                        Dim AllAudQualities As List(Of AudQuality) = New List(Of AudQuality)
                        For Each EachQual As String In output7
                            If EachQual.Contains("audio only") Then
                                Dim CurrentAudio As AudQuality = New AudQuality()
                                If EachQual.Contains("webm") Then
                                    CurrentAudio.Format = "webm"
                                ElseIf EachQual.Contains("m4a") Then
                                    CurrentAudio.Format = "m4a"
                                End If
                                Dim output8() As String
                                output8 = Regex.Split(EachQual, "Hz\), ")
                                If output8.Count > 1 Then
                                    CurrentAudio.FileSize = output8(1)
                                Else
                                    CurrentAudio.FileSize = "Unknown size"
                                End If
                                Dim output9() As String
                                output9 = Regex.Split(EachQual, "          ")
                                CurrentAudio.AudNo = output9(0)
                                AllAudQualities.Add(CurrentAudio)
                            ElseIf EachQual.Contains("video only") Then
                                Dim CurrentVideo As VidQuality = New VidQuality()
                                Dim output8() As String
                                output8 = Regex.Split(EachQual, "          ")
                                CurrentVideo.VidNo = output8(0)
                                Dim output9() As String
                                output9 = Regex.Split(EachQual, "k , ")
                                Dim output10() As String
                                output10 = Regex.Split(output9(0), "x")
                                If output10(1).Contains("144p") Then
                                    CurrentVideo.Resolution = "144p"
                                ElseIf output10(1).Contains("240p") Then
                                    CurrentVideo.Resolution = "240p"
                                ElseIf output10(1).Contains("360p") Then
                                    CurrentVideo.Resolution = "360p"
                                ElseIf output10(1).Contains("480p") Then
                                    CurrentVideo.Resolution = "480p"
                                ElseIf output10(1).Contains("720p") Then
                                    CurrentVideo.Resolution = "720p"
                                ElseIf output10(1).Contains("1080p") Then
                                    CurrentVideo.Resolution = "1080p"
                                End If
                                Dim output11() As String
                                output11 = Regex.Split(EachQual, "only, ")
                                If output11.Count > 1 Then
                                    CurrentVideo.FileSize = output11(1)
                                Else
                                    CurrentVideo.FileSize = "Unknown size"
                                End If
                                If EachQual.Contains("mp4") Then
                                    CurrentVideo.Format = "mp4"
                                ElseIf EachQual.Contains("webm") Then
                                    CurrentVideo.Format = "webm"
                                End If
                                AllVideoQualities.Add(CurrentVideo)
                            End If
                        Next
                        NewScrape.AudQualities = AllAudQualities
                        NewScrape.VidQualities = AllVideoQualities
                        Exit For
                    End If
                Next
                If FoundQuality = False Then
                    'MsgBox("error") //need to add error handling here (and in other places)
                End If
                list.Add(NewScrape)
            End If
        Next
        Dim PlaylistMetadata As PlaylistMetadataScrape = New PlaylistMetadataScrape()
        PlaylistMetadata.VideoMetadataList = list
        Return PlaylistMetadata
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
    Dim VidID As String
End Structure
Public Structure PlaylistMetadataScrape
    Dim VideoMetadataList As List(Of MetadataScrape)
    Dim PlaylistName As String
End Structure

