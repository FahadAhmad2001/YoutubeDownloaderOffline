Imports System.Threading
Imports System.Text.RegularExpressions
Imports System.IO
Public Class Form1
    Dim GetVidThumbnail As New Process()
    Dim GetVidThumbnailInfo As New ProcessStartInfo(Application.StartupPath & "\youtube-dl.exe")
    Dim GetVidTitle As New Process()
    Dim GetVidTitleInfo As New ProcessStartInfo(Application.StartupPath & "\youtube-dl.exe")
    Dim EditBAT As StreamWriter
    Dim EditLogs As StreamWriter
    Dim VidURL As String
    Dim PicURL As String
    Dim VidTitle As String
    Dim GetAllFormats As New Process()
    Dim GetAllFormatsInfo As New ProcessStartInfo(Application.StartupPath & "\youtube-dl.exe")
    Dim AudioCount As Integer
    Dim VideoCount As Integer
    Dim Audios As String
    Dim Videos As String
    Dim CurrentAudNumber As String
    Dim CurrentAudSize As String
    Dim CurrentAudFormat As String
    Dim CurrentVidNumber As String
    Dim CurrentVidSize As String
    Dim CurrentVidFormat As String
    Dim CurrentVidRes As String
    Dim firstVid As String
    Dim SelectedAudioNumber As String
    Dim SelectedAudioSize As String
    Dim SelectedVideoSize As String
    Dim SelectedVideoNumber As String
    Dim NeedToConvert As String
    Dim SelectedAudioFormat As String
    Dim SelectedVideoFormat As String
    Dim HasSelectedQuality As String
    Dim AutoVideoDownloader As Thread
    Dim DownloadRunning As String
    Dim NormalDownload As New Process()
    Dim NormalDownloadInfo As New ProcessStartInfo(Application.StartupPath & "\NormalDownload.bat")
    Dim ETA As String
    Dim CurrentProgress As String
    Dim CurrentSpeed As String
    Dim FileNameToSave As String
    Dim ConvertDownloadedVid As New Process()
    Dim ConvertDownloadedVidInfo As New ProcessStartInfo(Application.StartupPath & "\ffmpeg.exe")
    Dim SameSelectedFormat As String
    Dim CheckVersion As StreamReader
    Public CurrentVersion As String
    Public NewVersion As String
    Public CurrentVDate As String
    Public NewVDate As String
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Label3.Text = ""
        Label4.Text = ""
        Label7.Text = ""
        Label8.Text = ""
        'PictureBox1.Image = Image.FromFile(Application.StartupPath & "\thumbnail.jpg")
        'Audios(0) = "sample"
        ' Audios(1) = "sample"
        'Videos(0) = "sample"
        'Videos(1) = "sample"
        firstVid = True
        DownloadRunning = "FALSE"

        If File.Exists(Application.StartupPath & "\config.txt") Then
            CheckVersion = New StreamReader(Application.StartupPath & "\config.txt")
            Dim VersionContents As String
            VersionContents = CheckVersion.ReadToEnd()
            CheckVersion.Close()
            Dim output1() As String
            output1 = VersionContents.Split(":")
            CurrentVersion = output1(0)
            CurrentVDate = output1(1)
            If My.Computer.Network.Ping("serverwebsite.ddns.net") Then
                If File.Exists(Application.StartupPath & "\remoteversion.txt") Then
                    File.Delete(Application.StartupPath & "\remoteversion.txt")
                End If
                My.Computer.Network.DownloadFile("ftp://serverwebsite.ddns.net/downloads/youtubedownload/remoteversion.txt", Application.StartupPath & "\remoteversion.txt")
                CheckVersion = New StreamReader(Application.StartupPath & "\remoteversion.txt")
                Dim NewVersionContents As String
                NewVersionContents = CheckVersion.ReadToEnd()
                CheckVersion.Close()
                Dim output2() As String
                output2 = NewVersionContents.Split(":")
                NewVersion = output2(0)
                NewVDate = output2(1)
                If output1(0) = output2(0) Then

                Else
                    Dim reply As DialogResult = MessageBox.Show("A new version, " & output2(0) & " is available, from " & output2(1) & ". Would you like to update?", "Update available", MessageBoxButtons.YesNoCancel)
                    If reply = DialogResult.Yes Then
                        Form2.ShowDialog()
                    End If
                End If
            Else
                MsgBox("Cannot contact update server")
            End If
        Else
            MsgBox("Cannot find config.txt file")
        End If
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

        VidURL = TextBox1.Text
        If VidURL.Contains("youtube.com/") Then
            HasSelectedQuality = ""
            AudioCount = 0
            VideoCount = 0
            If firstVid = False Then
                AudioCount = 0
                VideoCount = 0
                'Array.Clear(Audios, 0, Audios.Length)
                'Array.Clear(Videos, 0, Videos.Length)
                Audios = String.Empty
                Videos = String.Empty
            End If
            Label8.Text = "Getting Metadata..."
            If File.Exists(Application.StartupPath & "\thumbnail.jpg") Then
                File.Delete(Application.StartupPath & "\thumbnail.jpg")
            End If
            GetVidThumbnailInfo.Arguments = "--get-thumbnail " & VidURL
            GetVidThumbnailInfo.UseShellExecute = False
            GetVidThumbnailInfo.RedirectStandardOutput = True
            GetVidThumbnailInfo.CreateNoWindow = True
            GetVidThumbnailInfo.WindowStyle = ProcessWindowStyle.Hidden
            GetVidThumbnail.StartInfo = GetVidThumbnailInfo
            GetVidThumbnail.Start()
            PicURL = GetVidThumbnail.StandardOutput.ReadToEnd()
            GetVidThumbnail.WaitForExit()
            GetVidThumbnail.Close()
            My.Computer.Network.DownloadFile(PicURL, Application.StartupPath & "\thumbnail.jpg")
            PictureBox1.Image = Image.FromFile(Application.StartupPath & "\thumbnail.jpg")
            GetVidTitleInfo.UseShellExecute = False
            GetVidTitleInfo.RedirectStandardOutput = True
            GetVidTitleInfo.WindowStyle = ProcessWindowStyle.Hidden
            GetVidTitleInfo.CreateNoWindow = True
            GetVidTitle.StartInfo = GetVidTitleInfo
            GetVidTitleInfo.Arguments = "--get-title " & VidURL
            GetVidTitle.Start()
            VidTitle = GetVidTitle.StandardOutput.ReadToEnd()
            GetVidTitle.WaitForExit()
            GetVidTitle.Close()
            Label3.Text = VidTitle
            'Label8.Text = ""
            GetAllFormatsInfo.Arguments = "-F " & VidURL
            GetAllFormatsInfo.CreateNoWindow = True
            GetAllFormatsInfo.UseShellExecute = False
            GetAllFormatsInfo.WindowStyle = ProcessWindowStyle.Hidden
            GetAllFormatsInfo.RedirectStandardOutput = True
            GetAllFormats.StartInfo = GetAllFormatsInfo
            GetAllFormats.Start()
            Dim out1 As String
            out1 = GetAllFormats.StandardOutput.ReadToEnd()
            GetAllFormats.WaitForExit()
            GetAllFormats.Close()
            'MsgBox(out1)
            EditLogs = New StreamWriter(Application.StartupPath & "\sampleout.txt")
            EditLogs.Write(out1)
            EditLogs.Close()
            'If out1.Contains(Chr(10)) Then
            'MsgBox("contains vbCrLf")
            'End If
            '   If out1.Contains(Chr(32)) Then
            '  MsgBox("contains ASCII spaces")
            'End If
            Dim output1() As String
            output1 = Regex.Split(out1, "note")
            Dim output2() As String
            output2 = output1(1).Split(Chr(10))
            For Each item As String In output2
                'MsgBox(item)
                If item.Contains("audio only") Then
                    'Dim TempCount As Integer
                    'TempCount = AudioCount
                    Dim output3() As String
                    output3 = Regex.Split(item, "          ")
                    CurrentAudNumber = output3(0)
                    If item.Contains("m4a") Then
                        CurrentAudFormat = "m4a"
                    ElseIf item.Contains("webm") Then
                        CurrentAudFormat = "webm"
                    End If
                    Dim output4() As String
                    output4 = Regex.Split(item, "k, ")
                    CurrentAudSize = output4(1)
                    'TempCount = TempCount + 1
                    'MsgBox(CurrentAudNumber & "TRIM" & CurrentAudFormat & "TRIM" & CurrentAudSize)
                    Audios = Audios & CurrentAudNumber & "TRIM" & CurrentAudFormat & "TRIM" & CurrentAudSize & "NEXT"

                    AudioCount = AudioCount + 1
                ElseIf item.Contains("video only") Then
                    ' Dim TempCount As Integer
                    ' TempCount = AudioCount + 1
                    Dim output3() As String
                    output3 = Regex.Split(item, "          ")
                    CurrentVidNumber = output3(0)
                    If item.Contains("mp4") Then
                        CurrentVidFormat = "mp4"
                    ElseIf item.Contains("webm") Then
                        CurrentVidFormat = "webm"
                    End If
                    Dim output4() As String
                    output4 = Regex.Split(item, "k , ")
                    Dim output5() As String
                    output5 = Regex.Split(output4(0), "x")
                    Dim output7() As String
                    output7 = Regex.Split(output5(1), Chr(32))
                    'CurrentVidRes = output7(1)
                    If output5(1).Contains("144p") Then
                        CurrentVidRes = "144p"
                    ElseIf output5(1).Contains("240p") Then
                        CurrentVidRes = "240p"
                    ElseIf output5(1).Contains("360p") Then
                        CurrentVidRes = "360p"
                    ElseIf output5(1).Contains("480p") Then
                        CurrentVidRes = "480p"
                    ElseIf output5(1).Contains("720p") Then
                        CurrentVidRes = "720p"
                    ElseIf output5(1).Contains("1080p") Then
                        CurrentVidRes = "1080p"
                    End If
                    'MsgBox(CurrentVidRes)
                    Dim output6() As String
                    output6 = Regex.Split(item, "only, ")
                    CurrentVidSize = output6(1)
                    VideoCount = VideoCount + 1
                    Videos = Videos & CurrentVidNumber & "TRIM" & CurrentVidFormat & "TRIM" & CurrentVidRes & "TRIM" & CurrentVidSize & "NEXT"

                    ComboBox1.Items.Add(CurrentVidFormat & " - " & CurrentVidRes)
                End If
            Next
            TrackBar1.Maximum = AudioCount - 1
            Label8.Text = ""
            'MsgBox(Videos)
            ComboBox1.Text = "Select a quality:"
            Label4.Text = "Select a video and audio quality:"
            'MsgBox(AudioCount)
        Else
            MsgBox("Not a valid YouTube URL")
            GoTo EndMetadata
        End If
EndMetadata:
    End Sub

    Private Sub Label5_Click(sender As Object, e As EventArgs) Handles Label5.Click

    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        Dim CurrentQualitySelected As String
        CurrentQualitySelected = ComboBox1.SelectedItem.ToString()
        Dim output1() As String
        output1 = Regex.Split(CurrentQualitySelected, " - ")
        Dim output2() As String
        output2 = Regex.Split(Videos, "NEXT")
        Dim SelectedVideoIndex As String
        For Each item As String In output2
            If item.Contains(output1(0)) = True And item.Contains(output1(1)) = True Then
                SelectedVideoIndex = item
            End If
        Next
        Dim output3() As String
        output3 = Regex.Split(SelectedVideoIndex, "TRIM")
        SelectedVideoNumber = output3(0)
        SelectedVideoSize = output3(3)
        SelectedVideoFormat = output3(1)
        If HasSelectedQuality.Contains("VIDEO") = False Then
            HasSelectedQuality = HasSelectedQuality + "VIDEO"
        End If
        If HasSelectedQuality.Contains("AUDIO") = True And HasSelectedQuality.Contains("VIDEO") = True Then
            Label4.Text = "Total File Size: " & SelectedVideoSize & " (VIDEO), " & SelectedAudioSize & " (AUDIO)"
        End If
    End Sub

    Private Sub TrackBar1_Scroll(sender As Object, e As EventArgs) Handles TrackBar1.Scroll
        'MsgBox(TrackBar1.Value)
        Dim output1() As String
        output1 = Regex.Split(Audios, "NEXT")
        Dim SelectedItem As String
        SelectedItem = output1(TrackBar1.Value)
        Dim output2() As String
        output2 = Regex.Split(SelectedItem, "TRIM")
        SelectedAudioSize = output2(2)
        SelectedAudioNumber = output2(0)
        SelectedAudioFormat = output2(1)
        If HasSelectedQuality.Contains("AUDIO") = False Then
            HasSelectedQuality = HasSelectedQuality + "AUDIO"
        End If
        If HasSelectedQuality.Contains("AUDIO") = True And HasSelectedQuality.Contains("VIDEO") = True Then
            Label4.Text = "Total File Size: " & SelectedVideoSize & " (VIDEO), " & SelectedAudioSize & " (AUDIO)"
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        SameSelectedFormat = ""
        If DownloadRunning = "FALSE" Then
            AutoVideoDownloader = New Thread(AddressOf NormalDownloader)
            AutoVideoDownloader.Start()
            'MsgBox("Started thread")
            DownloadRunning = "TRUE"
            GoTo EndStart
        Else
            MsgBox("Please wait for the current download to complete")
        End If
EndStart:
    End Sub
    Public Sub NormalDownloader()
        ' MsgBox("sub started")
        NeedToConvert = ""
        If SelectedVideoFormat.Contains("webm") And SelectedAudioFormat.Contains("m4a") Then
            NeedToConvert = "TRUE"
            MsgBox("Format error. Please select a different audio quality")
            MsgBox("Cross format support is currently under development")
            GoTo EndDownloading
        End If
        If SelectedVideoFormat.Contains("mp4") And SelectedAudioFormat.Contains("webm") Then
            NeedToConvert = "TRUE"
            MsgBox("Format error. Please select a different audio quality")
            MsgBox("Cross format support is currently under development")
            GoTo EndDownloading
        End If
        If SelectedVideoFormat.Contains("mp4") And SelectedAudioFormat.Contains("m4a") Then
            NeedToConvert = ""
            SameSelectedFormat = "mp4"
        End If
        If SelectedVideoFormat.Contains("webm") And SelectedAudioFormat.Contains("webm") Then
            NeedToConvert = ""
            SameSelectedFormat = "webm"
        End If
        Dim command As String
        command = "-f " & SelectedVideoNumber & "+" & SelectedAudioNumber & " " & VidURL & " --newline"
        EditLogs = New StreamWriter(Application.StartupPath & "\tempfile.txt")
        EditLogs.WriteLine(command)
        EditLogs.Close()
        EditBAT = New StreamWriter(Application.StartupPath & "\NormalDownload.bat")
        EditBAT.WriteLine("cd " & Chr(34) & Application.StartupPath & Chr(34))
        EditBAT.WriteLine("youtube-dl " & command)
        EditBAT.Close()
        'NormalDownloadInfo.Arguments = command
        NormalDownloadInfo.CreateNoWindow = True
        NormalDownloadInfo.UseShellExecute = False
        NormalDownloadInfo.WindowStyle = ProcessWindowStyle.Hidden
        NormalDownloadInfo.RedirectStandardOutput = True
        NormalDownloadInfo.RedirectStandardError = True
        NormalDownload.StartInfo = NormalDownloadInfo
        NormalDownload.Start()
        EditLogs = New StreamWriter(Application.StartupPath & "\downloadlog.txt")
        Dim OutputReader As StreamReader = NormalDownload.StandardOutput
        ReadOutput(OutputReader)
        'MsgBox("reading output")
        NormalDownload.WaitForExit()
        NormalDownload.Close()
        EditLogs.Close()
        'MsgBox(NormalDownload.ExitCode.ToString)
        If NeedToConvert.Contains("TRUE") Then
            Dim command2 As String
            command2 = Chr(34) & FileNameToSave & ".mkv" & Chr(34) & " " & Chr(34) & FileNameToSave & ".mp4" & Chr(34)
            ConvertDownloadedVidInfo.WindowStyle = ProcessWindowStyle.Hidden
            ConvertDownloadedVidInfo.Arguments = command2
            ConvertDownloadedVidInfo.CreateNoWindow = True

            ConvertDownloadedVid.StartInfo = ConvertDownloadedVidInfo
            ConvertDownloadedVid.Start()
            Label8.Text = "Converting..."
            MsgBox("converting...")
            ConvertDownloadedVid.WaitForExit()
            ConvertDownloadedVid.Close()
            Label8.Text = ""
            If File.Exists(Application.StartupPath & "\" & FileNameToSave & ".mp4") Then
                File.Copy(Application.StartupPath & "\" & FileNameToSave & ".mp4", My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\" & FileNameToSave & ".mp4")
            Else
                MsgBox("Error in downloading the video, please contact support@serverwebsite.ddns.net")
            End If
        Else
            If SameSelectedFormat.Contains("mp4") Then
                If File.Exists(Application.StartupPath & "\" & FileNameToSave & ".mp4") Then
                    File.Copy(Application.StartupPath & "\" & FileNameToSave & ".mp4", My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\" & FileNameToSave & ".mp4")
                    File.Delete(Application.StartupPath & "\" & FileNameToSave & ".mp4")
                Else
                    MsgBox("Error in downloading the video, please contact support@serverwebsite.ddns.net")
                    GoTo EndDownloading
                End If
            ElseIf SameSelectedFormat.Contains("webm") Then
                If File.Exists(Application.StartupPath & "\" & FileNameToSave & ".webm") Then
                    File.Copy(Application.StartupPath & "\" & FileNameToSave & ".webm", My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\" & FileNameToSave & ".webm")
                    File.Delete(Application.StartupPath & "\" & FileNameToSave & ".webm")
                Else
                    MsgBox("Error in downloading the video, please contact support@serverwebsite.ddns.net")
                    GoTo EndDownloading
                End If
            End If
        End If

        NeedToConvert = ""
        DownloadRunning = "FALSE"
        ProgressBar1.Value = 0
        Label7.Text = ""
        MsgBox("Download completed")
EndDownloading:
        AutoVideoDownloader.Abort()
    End Sub
    Private Async Sub ReadOutput(OutputReader As StreamReader)
        Dim newline As String = Await OutputReader.ReadLineAsync()
        If newline IsNot Nothing Then
            EditLogs.WriteLine(newline)
            MsgBox(newline)
            If newline.Contains("[download]") = True And newline.Contains("Destination: ") = False And newline.Contains("100%") = False And newline.Contains("[youtube]") = False Then
                MsgBox("progressbar")
                Dim output1() As String
                output1 = Regex.Split(newline, " ETA ")
                ETA = output1(1)
                Dim output2() As String
                output2 = Regex.Split(output1(0), " at ")
                Dim output3() As String
                output3 = newline.Split("]")
                Dim TextToShow As String
                TextToShow = output3(1)
                TextToShow = TextToShow.TrimStart(Chr(32))
                'Label7.Text = "Downloading: " & TextToShow
                Dim output4() As String
                output4 = Regex.Split(TextToShow, "% of ")
                CurrentProgress = output4(0)
                Dim IntProgress As Integer
                Dim DecProgress As Decimal = CurrentProgress
                IntProgress = DecProgress * 10
                'ProgressBar1.Value = IntProgress
                MsgBox(ETA & " was ETA, ")
            ElseIf newline.Contains("[download]") = True And newline.Contains("[youtube]") = False And newline.Contains("Destination: ") = True Then
                MsgBox("Naming")
                Dim output1() As String
                output1 = Regex.Split(newline, "[download] Destination: ")
                ' MsgBox("trimmed output, ")

                'MsgBox(output1(0))
                Dim output2() As String
                Dim CutText As String
                If newline.Contains(".f" & SelectedVideoNumber & "." & SelectedVideoFormat) Then
                    CutText = ".f" & SelectedVideoNumber & "." & SelectedVideoFormat
                ElseIf newline.Contains(".f" & SelectedAudioNumber & "." & SelectedAudioFormat) Then
                    CutText = ".f" & SelectedAudioNumber & "." & SelectedAudioFormat
                End If
                output2 = Regex.Split(output1(0), CutText)
                FileNameToSave = output2(0)
                Dim output3() As String
                If FileNameToSave.Contains("[download] Destination: ") Then
                    'MsgBox("regex not working")
                End If
                output3 = Regex.Split(FileNameToSave, "Destination: ")
                FileNameToSave = output3(1)
                'MsgBox("File Name " & FileNameToSave)
                If NeedToConvert = "TRUE" Then

                Else

                End If
            End If
        End If
        ReadOutput(OutputReader)
    End Sub
End Class
