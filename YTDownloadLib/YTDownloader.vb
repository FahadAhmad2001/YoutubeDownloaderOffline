Imports System.IO
Imports System.Text.RegularExpressions
Public Class YTDownloader
    Dim DownType As DownloadType
    Dim DownloadVid As Process = New Process()
    Dim AudQualt As AudQuality
    Dim VidQualt As VidQuality
    Dim VidID As String
    Public Event DownloadProgressChanged(ByVal ProgData As DownloadProgress)
    Public Event DownloadCompleted(ByVal ProgData As DownloadProgress)
    Public Sub DownloadVideo(ByVal url As String, ByVal type As DownloadType, Optional ByVal audqual As AudQuality = Nothing, Optional ByVal vidqual As VidQuality = Nothing)
        VidID = url.Split("=")(1)
        DownloadVid.EnableRaisingEvents = True
        Dim DownloadVidInfo As New ProcessStartInfo(Directory.GetCurrentDirectory & "\VidDownload.bat")
        DownloadVidInfo.CreateNoWindow = True
        DownloadVidInfo.WindowStyle = ProcessWindowStyle.Hidden
        DownloadVidInfo.UseShellExecute = False
        DownloadVidInfo.RedirectStandardOutput = True
        DownloadVidInfo.RedirectStandardError = True
        DownType = type
        If audqual.AudNo IsNot Nothing And vidqual.VidNo IsNot Nothing Then
            AudQualt = audqual
            VidQualt = vidqual
        End If
        Dim editBAT As StreamWriter
        editBAT = New StreamWriter(Directory.GetCurrentDirectory & "\VidDownload.bat")
        If DownType = DownloadType.Standard Then
            editBAT.WriteLine("youtube-dl " & url & " --newline")
        ElseIf DownType = DownloadType.CustomQuality Then
            editBAT.WriteLine("youtube-dl -f" & vidqual.VidNo & "+" & audqual.AudNo & " " & url & " --newline")
            AudQualt = audqual
            VidQualt = vidqual
        ElseIf DownType = DownloadType.MP3Only Then
            editBAT.WriteLine("youtube-dl --extract-audio --audio-format mp3 --newline " & url)
        ElseIf DownType = DownloadType.MP3Pic Then
            editBAT.WriteLine("youtube-dl --extract-audio --audio-format mp3 --newline --embed-thumbnail " & url)
        End If
        editBAT.Close()
        DownloadVid.StartInfo = DownloadVidInfo
        AddHandler DownloadVid.OutputDataReceived, AddressOf OutputReciever
        AddHandler DownloadVid.ErrorDataReceived, AddressOf ErrorReciever
        AddHandler DownloadVid.Exited, AddressOf DownloadComplete
        DownloadVid.Start()
        DownloadVid.BeginOutputReadLine()
        DownloadVid.BeginErrorReadLine()
    End Sub
    Private Sub ErrorReciever(sendingProcess As Object, output As DataReceivedEventArgs)

    End Sub
    Private Sub OutputReciever(sendingProcess As Object, output As DataReceivedEventArgs)
        If Not String.IsNullOrEmpty(output.Data) Then
            If output.Data.Contains("[ffmpeg] Destination: ") And (DownType = DownloadType.MP3Only Or DownType = DownloadType.MP3Pic) Then
                'Label8.Text = "Converting to MP3..."
                Dim output1() As String
                output1 = Regex.Split(output.Data, "ffmpeg] Destination: ")
                'MP3NameToSave = output1(1)
                Dim prog As DownloadProgress = New DownloadProgress()
                prog.ProgType = ProgressType.FileNameGotten
                prog.ProgressInfo = output1(1)
                prog.AudQual = AudQualt
                prog.VidQual = VidQualt
                prog.VidID = VidID
                RaiseEvent DownloadProgressChanged(prog)
                prog = New DownloadProgress()
                prog.ProgType = ProgressType.Converting
                prog.ProgressInfo = "Converting to MP3..."
                prog.VidID = VidID
                RaiseEvent DownloadProgressChanged(prog)
            End If
            If output.Data.ToString().Contains("[download] Destination: ") And (DownType = DownloadType.CustomQuality) Then
                Dim output3() As String
                output3 = Regex.Split(output.Data, "download] Destination: ")
                Dim CutText As String
                If output.Data.ToString().Contains(".f" & VidQualt.VidNo & "." & VidQualt.Format) Then
                    CutText = ".f" & VidQualt.VidNo & "." & VidQualt.Format
                Else 'If output.Data.ToString().Contains(".f" & AudQualt.AudNo & "." & AudQualt.Format) Then
                    'CutText = ".f" & AudQualt.AudNo & "." & AudQualt.Format
                    GoTo EndRaiseEvent
                End If
                Dim output4() As String
                output4 = Regex.Split(output3(1), CutText)
                'FileNameToSave = output4(0)
                'MsgBox(FileNameToSave)
                Dim prog As DownloadProgress = New DownloadProgress()
                prog.ProgType = ProgressType.FileNameGotten
                prog.ProgressInfo = output4(0)
                prog.AudQual = AudQualt
                prog.VidQual = VidQualt
                prog.VidID = VidID
                RaiseEvent DownloadProgressChanged(prog)
EndRaiseEvent:
            End If
            If output.Data.ToString().Contains("[download]") = True And output.Data.ToString().Contains(" Destination: ") = False And output.Data.Contains(" 100% of ") = False Then
                Dim TextToShow As String
                Dim output5() As String
                output5 = Regex.Split(output.Data.ToString(), "download] ")
                TextToShow = output5(1)
                TextToShow.TrimStart(Chr(32))
                TextToShow = "Downloading: " & TextToShow
                Dim output6() As String
                output6 = Regex.Split(TextToShow, "% of ")
                Dim CurrentProgress As String
                CurrentProgress = output6(0)
                Dim output7() As String
                output7 = Regex.Split(CurrentProgress, "Downloading: ")
                CurrentProgress = output7(1)
                Dim IntProgress As Integer
                Dim DecProgress As Decimal = Decimal.Parse(CurrentProgress)
                IntProgress = DecProgress * 10
                'ProgressBar1.Value = IntProgress
                Dim prog As DownloadProgress = New DownloadProgress()
                prog.ProgType = ProgressType.ProgressChanged
                prog.ProgressPercentageFromThousand = IntProgress
                prog.ProgressInfo = TextToShow
                prog.AudQual = AudQualt
                prog.VidQual = VidQualt
                prog.VidID = VidID
                RaiseEvent DownloadProgressChanged(prog)
            End If
        End If
    End Sub
    Private Sub DownloadComplete(sendingProcess As Object, output As EventArgs)
        Dim prog As DownloadProgress = New DownloadProgress()
        prog.ProgType = ProgressType.Completed
        prog.ProgressPercentageFromThousand = 100
        prog.ProgressInfo = "Download Completed"
        prog.DownType = DownType
        prog.AudQual = AudQualt
        prog.VidQual = VidQualt
        prog.VidID = VidID
        RaiseEvent DownloadCompleted(prog)
        DownloadVid.Close()
    End Sub
End Class
Public Enum DownloadType
    Standard = 0
    CustomQuality
    MP3Only
    MP3Pic
End Enum
Public Enum ProgressType
    ProgressChanged = 0
    FileNameGotten
    Converting
    Completed
End Enum
Public Structure DownloadProgress
    Dim ProgType As ProgressType
    Dim ProgressPercentageFromThousand As Integer
    Dim ProgressInfo As String
    Dim DownType As DownloadType
    Dim AudQual As AudQuality
    Dim VidQual As VidQuality
    Dim VidID As String
End Structure
