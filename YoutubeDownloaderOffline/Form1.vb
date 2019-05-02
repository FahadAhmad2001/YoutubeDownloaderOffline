Imports System.Threading
Imports System.Text.RegularExpressions
Imports System.IO
Imports MS.WindowsAPICodePack
Imports Microsoft.WindowsAPICodePack
Imports Microsoft.WindowsAPICodePack.Taskbar
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
    Dim WithEvents NormalDownload As New Process()
    Dim NormalDownloadInfo As New ProcessStartInfo(Application.StartupPath & "\NormalDownload.bat")
    Dim ETA As String
    Dim CurrentProgress As String
    Dim CurrentSpeed As String
    Dim FileNameToSave As String
    Dim ConvertDownloadedVid As New Process()
    Dim ConvertDownloadedVidInfo As New ProcessStartInfo(Application.StartupPath & "\ffmpeg.exe")
    Dim WithEvents MP3Download As New Process()
    Dim MP3DownloadInfo As New ProcessStartInfo(Application.StartupPath & "\MP3Download.bat")
    Dim SameSelectedFormat As String
    Dim CheckVersion As StreamReader
    Public CurrentVersion As String
    Public NewVersion As String
    Public CurrentVDate As String
    Public NewVDate As String
    Dim AllFiles As String
    Public MissingFiles As String
    Dim MP3NameToSave As String
    Dim WithEvents UpdateYTDL As New Process()
    Dim UpdateYTDLInfo As New ProcessStartInfo(Application.StartupPath & "\youtube-dl.exe")
    Dim KeepThumbnailMP3 As Boolean
    Dim PostDownloadCmd As Boolean
    Dim PDCommand As String
    Dim SettingsContents As String
    Dim UseAutoCC As Boolean
    Dim UseSubtitles As Boolean
    Dim DownloadAllCC As Boolean
    Dim SaveLocation As String
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
        AllFiles = ""
        MissingFiles = ""
        System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = False
        If File.Exists(Application.StartupPath & "\blank.jpg") Then

        Else
            If AllFiles.Contains("FALSE") = False Then
                AllFiles = "FALSE"
            End If
            MissingFiles = MissingFiles & "blank.jpg" & ":"
        End If
        If File.Exists(Application.StartupPath & "\avcodec-57.dll") Then

        Else
            If AllFiles.Contains("FALSE") = False Then
                AllFiles = "FALSE"
            End If
            MissingFiles = MissingFiles & "avcodec-57.dll" & ":"
        End If
        If File.Exists(Application.StartupPath & "\avdevice-57.dll") Then

        Else
            If AllFiles.Contains("FALSE") = False Then
                AllFiles = "FALSE"
            End If
            MissingFiles = MissingFiles & "avdevice-57.dll" & ":"
        End If
        If File.Exists(Application.StartupPath & "\avfilter-6.dll") Then

        Else
            If AllFiles.Contains("FALSE") = False Then
                AllFiles = "FALSE"
            End If
            MissingFiles = MissingFiles & "avfilter-6.dll" & ":"
        End If
        If File.Exists(Application.StartupPath & "\avformat-57.dll") Then

        Else
            If AllFiles.Contains("FALSE") = False Then
                AllFiles = "FALSE"
            End If
            MissingFiles = MissingFiles & "avformat-57.dll" & ":"
        End If
        If File.Exists(Application.StartupPath & "\avutil-55.dll") Then

        Else
            If AllFiles.Contains("FALSE") = False Then
                AllFiles = "FALSE"
            End If
            MissingFiles = MissingFiles & "avutil-55.dll" & ":"
        End If
        If File.Exists(Application.StartupPath & "\ffmpeg.exe") Then

        Else
            If AllFiles.Contains("FALSE") = False Then
                AllFiles = "FALSE"
            End If
            MissingFiles = MissingFiles & "ffmpeg.exe" & ":"
        End If
        If File.Exists(Application.StartupPath & "\ffprobe.exe") Then

        Else
            If AllFiles.Contains("FALSE") = False Then
                AllFiles = "FALSE"
            End If
            MissingFiles = MissingFiles & "ffprobe.exe" & ":"
        End If
        If File.Exists(Application.StartupPath & "\ffplay.exe") Then

        Else
            If AllFiles.Contains("FALSE") = False Then
                AllFiles = "FALSE"
            End If
            MissingFiles = MissingFiles & "ffplay.exe" & ":"
        End If
        If File.Exists(Application.StartupPath & "\postproc-54.dll") Then

        Else
            If AllFiles.Contains("FALSE") = False Then
                AllFiles = "FALSE"
            End If
            MissingFiles = MissingFiles & "postproc-54.dll" & ":"
        End If
        If File.Exists(Application.StartupPath & "\swresample-2.dll") Then

        Else
            If AllFiles.Contains("FALSE") = False Then
                AllFiles = "FALSE"
            End If
            MissingFiles = MissingFiles & "swresample-2.dll" & ":"
        End If
        If File.Exists(Application.StartupPath & "\swscale-4.dll") Then

        Else
            If AllFiles.Contains("FALSE") = False Then
                AllFiles = "FALSE"
            End If
            MissingFiles = MissingFiles & "swscale-4.dll" & ":"
        End If
        If File.Exists(Application.StartupPath & "\youtube-dl.exe") Then

        Else
            If AllFiles.Contains("FALSE") = False Then
                AllFiles = "FALSE"
            End If
            MissingFiles = MissingFiles & "youtube-dl.exe" & ":"
        End If
        MissingFiles.TrimEnd(":")
        If AllFiles.Contains("FALSE") Then
            Dim reply2 As DialogResult = MessageBox.Show("Some files are missing. Would you like to download them?", "Fix Missing Files", MessageBoxButtons.YesNoCancel)
            If reply2 = DialogResult.Yes Then
                MissingFiles = MissingFiles & "END"
                Form3.ShowDialog()
            End If
        End If

        UpdateYTDLInfo.CreateNoWindow = True
        UpdateYTDLInfo.Arguments = "-U"
        UpdateYTDLInfo.UseShellExecute = False
        UpdateYTDLInfo.WindowStyle = ProcessWindowStyle.Hidden
        UpdateYTDLInfo.RedirectStandardOutput = True

        If File.Exists(Application.StartupPath & "\config.txt") Then
            CheckVersion = New StreamReader(Application.StartupPath & "\config.txt")
            Dim VersionContents As String
            VersionContents = CheckVersion.ReadToEnd()
            CheckVersion.Close()
            Dim output1() As String
            output1 = VersionContents.Split(":")
            CurrentVersion = output1(0)
            CurrentVDate = output1(1)
            If My.Computer.Network.Ping("google.com") Then
                UpdateYTDL.StartInfo = UpdateYTDLInfo
                AddHandler UpdateYTDL.OutputDataReceived, AddressOf UpdateOutputReader
                UpdateYTDL.Start()
                UpdateYTDL.BeginOutputReadLine()
                UpdateYTDL.WaitForExit()
                UpdateYTDL.Close()
            End If
            If My.Computer.Network.Ping("serverwebsite.ddns.net") Then
                If File.Exists(Application.StartupPath & "\remoteversion.txt") Then
                    File.Delete(Application.StartupPath & "\remoteversion.txt")
                End If
                My.Computer.Network.DownloadFile("http://serverwebsite.ddns.net:500/youtubedownload/remoteversion.txt", Application.StartupPath & "\remoteversion.txt")
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
        Dim ReadINI As StreamReader
        If File.Exists(Application.StartupPath & "\config.ini") Then
            ReadINI = New StreamReader(Application.StartupPath & "\config.ini")
            SettingsContents = ReadINI.ReadToEnd()
            ReadINI.Close()
            Dim output1() As String
            output1 = SettingsContents.Split(";")
            If output1(1).Contains("True") Then
                KeepThumbnailMP3 = True
            Else
                KeepThumbnailMP3 = False
            End If
            If output1(2).Contains("True") Then
                PostDownloadCmd = True
            Else
                PostDownloadCmd = False
            End If
            If output1(5).Contains("True") Then
                UseAutoCC = True
            Else
                UseAutoCC = False
            End If
            If output1(4).Contains("True") Then
                UseSubtitles = True
            Else
                UseSubtitles = False
            End If
            If output1(6).Contains("True") Then
                DownloadAllCC = True
            Else
                DownloadAllCC = False
            End If
            Dim output2() As String
            output2 = output1(8).Split("=")
            SaveLocation = output2(1)
        Else
            MessageBox.Show("Cannot load current settings as config.ini cannot be found" & vbCrLf & "or cannot be read due to insufficient permissions", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            KeepThumbnailMP3 = True
            PostDownloadCmd = False
            UseAutoCC = False
            UseSubtitles = False
            DownloadAllCC = False
        End If
        If File.Exists(Application.StartupPath & "\command.txt") Then
            ReadINI = New StreamReader(Application.StartupPath & "\command.txt")
            PDCommand = ReadINI.ReadToEnd()
            ReadINI.Close()
        Else
            MessageBox.Show("Cannot load 'Run program after downloading' setting as command.txt cannot be found" & vbCrLf & "or cannot be read due to insufficient permissions", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

        VidURL = TextBox1.Text
        If VidURL.Contains("youtube.com/") Then
            ComboBox1.Items.Clear()
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
            Audios = String.Empty
            Videos = String.Empty
            PicURL = String.Empty
            VidTitle = String.Empty
            CurrentAudFormat = String.Empty
            CurrentAudNumber = String.Empty
            CurrentAudSize = String.Empty
            CurrentProgress = String.Empty
            CurrentSpeed = String.Empty
            CurrentVidFormat = String.Empty
            CurrentVidNumber = String.Empty
            CurrentVidRes = String.Empty
            CurrentVidSize = String.Empty
            'firstVid = String.Empty
            HasSelectedQuality = ""
            SelectedAudioFormat = ""
            SelectedAudioNumber = ""
            SelectedAudioSize = ""
            SelectedVideoFormat = ""
            SelectedVideoNumber = ""
            SelectedVideoSize = ""
            FileNameToSave = ""
            If Me.PictureBox1.Image Is Nothing Then

            Else

                Me.PictureBox1.Image.Dispose()

            End If
            Label8.Text = "Getting Metadata..."
            PictureBox1.Image = Image.FromFile(Application.StartupPath & "\blank.jpg")
            If File.Exists(Application.StartupPath & "\thumbnail.jpg") Then
                File.Delete(Application.StartupPath & "\thumbnail.jpg")
            End If
            GetVidThumbnailInfo.Arguments = "--get-thumbnail " & VidURL
            GetVidThumbnailInfo.UseShellExecute = False
            GetVidThumbnailInfo.RedirectStandardOutput = True
            GetVidThumbnailInfo.CreateNoWindow = True
            GetVidThumbnailInfo.WindowStyle = ProcessWindowStyle.Hidden
            'GetVidThumbnailInfo.RedirectStandardError = True
            Dim temp1 As String
            GetVidThumbnail.StartInfo = GetVidThumbnailInfo
            GetVidThumbnail.Start()
            PicURL = GetVidThumbnail.StandardOutput.ReadToEnd()
            'temp1 = GetVidThumbnail.StandardError.ReadToEnd()
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
            ComboBox1.Items.Add("MP3 Only")
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
        If ComboBox1.SelectedItem = "MP3 Only" Then
            Dim TempAudioType As String
            Dim output1() As String
            output1 = Regex.Split(Audios, "NEXT")
            TempAudioType = output1(TrackBar1.Maximum)
            HasSelectedQuality = ""
            Dim TempAudioSize As String
            Dim output2() As String
            output2 = Regex.Split(TempAudioType, "TRIM")
            TempAudioSize = output2(2)
            Label4.Text = "Total File Size: " & TempAudioSize & " (AUDIO ONLY)"
            TrackBar1.Enabled = False
        Else
            TrackBar1.Enabled = True
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
            Else
                Label4.Text = "Select a video and audio quality"
            End If
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
            If ComboBox1.SelectedItem = "MP3 Only" Then
                'MsgBox("starting thread")
                AutoVideoDownloader = New Thread(AddressOf MP3Downloader)
                AutoVideoDownloader.Start()
                'MsgBox("started thread")
                DownloadRunning = "TRUE"
                GoTo EndStart
            Else
                AutoVideoDownloader = New Thread(AddressOf NormalDownloader)
                AutoVideoDownloader.Start()
                'MsgBox("Started thread")
                DownloadRunning = "TRUE"
                GoTo EndStart
            End If
        Else
            MsgBox("Please wait for the current download to complete")
        End If
EndStart:
    End Sub
    Public Sub MP3Downloader()
        'MsgBox("sub started")
        Dim command As String
        If KeepThumbnailMP3 = True Then
            command = "--extract-audio --audio-format mp3 --newline --embed-thumbnail " & VidURL
        Else
            command = "--extract-audio --audio-format mp3 --newline " & VidURL
        End If
        EditLogs = New StreamWriter(Application.StartupPath & "\tempfile.txt")
        EditLogs.WriteLine(command)
        EditLogs.Close()
        EditBAT = New StreamWriter(Application.StartupPath & "\MP3Download.bat")
        EditBAT.WriteLine("cd " & Chr(34) & Application.StartupPath & Chr(34))
        EditBAT.WriteLine("youtube-dl " & command)
        EditBAT.Close()
        MP3DownloadInfo.CreateNoWindow = True
        MP3DownloadInfo.UseShellExecute = False
        MP3DownloadInfo.WindowStyle = ProcessWindowStyle.Hidden
        MP3DownloadInfo.RedirectStandardOutput = True
        'NormalDownloadInfo.RedirectStandardError = True
        MP3Download.StartInfo = MP3DownloadInfo
        EditLogs = New StreamWriter(Application.StartupPath & "\MP3downloadlog.txt")
        AddHandler MP3Download.OutputDataReceived, AddressOf NewOutputReader
        MP3Download.Start()
        'MsgBox("started")
        MP3Download.BeginOutputReadLine()
        'MsgBox("reading output")
        MP3Download.WaitForExit()
        MP3Download.CancelOutputRead()
        MP3Download.Close()
        EditLogs.Close()
        Dim temp1 As String
        temp1 = VidTitle.Replace(":", "-")
        temp1 = temp1.Replace("/", "-")
        temp1 = temp1.Replace("\", "-")
        temp1 = temp1.Replace("*", "-")
        temp1 = temp1.Replace("?", "-")
        temp1 = temp1.Replace(Chr(34), "-")
        'temp1 = temp1.Replace("/", "-")
        temp1 = temp1.Replace("<", "-")
        temp1 = temp1.Replace(">", "-")
        temp1 = temp1.Replace("|", "-")
        'MsgBox(temp1)
        If File.Exists(Application.StartupPath & "\" & MP3NameToSave) Then
            If SaveLocation = "Documents" Then
                If File.Exists(My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\" & MP3NameToSave) Then
                    SaveFileDialog1.FileName = MP3NameToSave
                    If SaveFileDialog1.ShowDialog() = DialogResult.OK Then
                        File.Copy(Application.StartupPath & "\" & MP3NameToSave, SaveFileDialog1.FileName)
                        File.Delete(Application.StartupPath & "\" & MP3NameToSave)
                        If PostDownloadCmd = True Then
                            Process.Start("cmd.exe", "/c " & PDCommand)
                        End If
                        MsgBox("MP3 successfully saved")
                    End If
                Else
                    File.Copy(Application.StartupPath & "\" & MP3NameToSave, My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\" & MP3NameToSave)
                    File.Delete(Application.StartupPath & "\" & MP3NameToSave)
                    If PostDownloadCmd = True Then
                        Process.Start("cmd.exe", "/c " & PDCommand)
                    End If
                    MsgBox("MP3 downloaded successfully and saved in My Documents")
                End If
            Else
                If File.Exists(SaveLocation & "\" & MP3NameToSave) Then
                    SaveFileDialog1.FileName = MP3NameToSave
                    If SaveFileDialog1.ShowDialog() = DialogResult.OK Then
                        File.Copy(Application.StartupPath & "\" & MP3NameToSave, SaveFileDialog1.FileName)
                        File.Delete(Application.StartupPath & "\" & MP3NameToSave)
                        If PostDownloadCmd = True Then
                            Process.Start("cmd.exe", "/c " & PDCommand)
                        End If
                        Label8.Text = ""
                        ProgressBar1.Value = 0
                        Label7.Text = ""
                        MsgBox("MP3 successfully saved")
                    End If
                Else
                    File.Copy(Application.StartupPath & "\" & MP3NameToSave, SaveLocation & "\" & MP3NameToSave)
                    File.Delete(Application.StartupPath & "\" & MP3NameToSave)
                    If PostDownloadCmd = True Then
                        Process.Start("cmd.exe", "/c " & PDCommand)
                    End If
                    Label8.Text = ""
                    ProgressBar1.Value = 0
                    Label7.Text = ""
                    MsgBox("MP3 downloaded successfully and saved")
                End If
            End If
            Label8.Text = ""
            ProgressBar1.Value = 0
            Label7.Text = ""
        Else
            MsgBox("Error in downloading, please contact support@lightspeedmedia.tk")
            Label8.Text = ""
            ProgressBar1.Value = 0
            Label7.Text = ""
        End If
        firstVid = False
        DownloadRunning = "FALSE"
        AutoVideoDownloader.Abort()
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
        command = "-f " & SelectedVideoNumber & "+" & SelectedAudioNumber
        If UseSubtitles = True Then
            If UseAutoCC = True Then
                command = command & " --write-auto-sub --embed-subs"
            End If
        End If
        command = command & " " & VidURL & " --newline"
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
        'NormalDownloadInfo.RedirectStandardError = True
        NormalDownload.StartInfo = NormalDownloadInfo
        AddHandler NormalDownload.OutputDataReceived, AddressOf NewOutputReader
        NormalDownload.Start()
        ' If firstVid = True Then
        NormalDownload.BeginOutputReadLine()
        'End If
        EditLogs = New StreamWriter(Application.StartupPath & "\downloadlog.txt")
        'Dim OutputReader As StreamReader = NormalDownload.StandardOutput
        'Dim ErrorReader As StreamReader = NormalDownload.StandardError
        'ErrorRead(ErrorReader)
        'ReadOutput(OutputReader)
        'MsgBox("reading output")
        Dim output2 As String
        Dim error2 As String
        'error2 = NormalDownload.StandardError.ReadToEnd()
        'output2 = NormalDownload.StandardOutput.ReadToEnd()
        'MsgBox("OUTPUT:" & output2)
        'MsgBox("ERROR:" & error2)
        NormalDownload.WaitForExit()
        NormalDownload.CancelOutputRead()
        NormalDownload.Close()
        firstVid = False
        EditLogs.Close()
        'MsgBox(NormalDownload.ExitCode.ToString)
        Dim SaveLoc As String
        If SaveLocation = "Documents" Then
            SaveLoc = My.Computer.FileSystem.SpecialDirectories.MyDocuments
        Else
            SaveLoc = SaveLocation
        End If
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
                File.Copy(Application.StartupPath & "\" & FileNameToSave & ".mp4", SaveLoc & "\" & FileNameToSave & ".mp4")
            Else
                MsgBox("Error in downloading/converting the video, please contact support@serverwebsite.ddns.net")
            End If
        Else
            If SameSelectedFormat.Contains("mp4") Then
                If File.Exists(Application.StartupPath & "\" & FileNameToSave & ".mp4") Then
                    File.Copy(Application.StartupPath & "\" & FileNameToSave & ".mp4", SaveLoc & "\" & FileNameToSave & ".mp4")
                    File.Delete(Application.StartupPath & "\" & FileNameToSave & ".mp4")
                Else
                    MsgBox("Error in downloading the video, please contact support@serverwebsite.ddns.net")
                    GoTo EndDownloading
                End If
            ElseIf SameSelectedFormat.Contains("webm") Then
                If File.Exists(Application.StartupPath & "\" & FileNameToSave & ".webm") Then
                    File.Copy(Application.StartupPath & "\" & FileNameToSave & ".webm", SaveLoc & "\" & FileNameToSave & ".webm")
                    File.Delete(Application.StartupPath & "\" & FileNameToSave & ".webm")
                Else
                    MsgBox("Error in downloading the video, please contact support@serverwebsite.ddns.net")
                    GoTo EndDownloading
                End If
            End If
        End If
        Label8.Text = ""
        ProgressBar1.Value = 0
        Label7.Text = ""
        If PostDownloadCmd = True Then
            Process.Start("cmd.exe", "/c " & PDCommand)
        End If
        NeedToConvert = ""

        MsgBox("Download completed. Video saved in My Documents.")
EndDownloading:
        DownloadRunning = "FALSE"
        ProgressBar1.Value = 0
        Label7.Text = ""
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
                MsgBox("trimmed output, ")

                MsgBox(output1(0))
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
                MsgBox("File Name " & FileNameToSave)
                If NeedToConvert = "TRUE" Then

                Else

                End If
            End If
        End If
        ReadOutput(OutputReader)
    End Sub
    Private Async Sub ErrorRead(ErrorReader As StreamReader)
        Dim newerrorline As String = Await ErrorReader.ReadLineAsync()
        If newerrorline IsNot Nothing Then
            MsgBox(newerrorline)
        End If
        ErrorRead(ErrorReader)
    End Sub
    Private Sub UpdateOutputReader(sendingProcess As Object, output As DataReceivedEventArgs)
        If Not String.IsNullOrEmpty(output.Data) Then
            If output.Data.Contains("Updating to version ") Then
                Dim output1() As String
                output1 = Regex.Split(output.Data, " version ")
                MessageBox.Show("Please wait for a moment" & vbCrLf & "Updating downloader to version " & output1(1))
            End If
        End If
    End Sub
    Private Sub NewOutputReader(sendingProcess As Object, output As DataReceivedEventArgs)
        'MsgBox(output.Data)
        If Not String.IsNullOrEmpty(output.Data) Then
            'MsgBox(output.Data)
            EditLogs.WriteLine(output.Data)
            If output.Data.Contains("[ffmpeg] Destination: ") And ComboBox1.SelectedItem = "MP3 Only" Then
                Label8.Text = "Converting to MP3..."
                Dim output1() As String
                output1 = Regex.Split(output.Data, "ffmpeg] Destination: ")
                MP3NameToSave = output1(1)
            End If

            If output.Data.ToString().Contains("[download] Destination: ") And (ComboBox1.SelectedItem IsNot "MP3 Only") Then
                Dim output3() As String
                output3 = Regex.Split(output.Data, "download] Destination: ")
                Dim CutText As String
                If output.Data.ToString().Contains(".f" & SelectedVideoNumber & "." & SelectedVideoFormat) Then
                    CutText = ".f" & SelectedVideoNumber & "." & SelectedVideoFormat
                ElseIf output.Data.ToString().Contains(".f" & SelectedAudioNumber & "." & SelectedAudioFormat) Then
                    CutText = ".f" & SelectedAudioNumber & "." & SelectedAudioFormat
                End If
                Dim output4() As String
                output4 = Regex.Split(output3(1), CutText)
                FileNameToSave = output4(0)
                'MsgBox(FileNameToSave)
            End If
            If output.Data.ToString().Contains("[download]") = True And output.Data.ToString().Contains(" Destination: ") = False And output.Data.Contains(" 100% of ") = False Then
                    Dim TextToShow As String
                    Dim output5() As String
                    output5 = Regex.Split(output.Data.ToString(), "download] ")
                    TextToShow = output5(1)
                    TextToShow.TrimStart(Chr(32))
                Label7.Text = "Downloading: " & TextToShow
                Dim output6() As String
                    output6 = Regex.Split(TextToShow, "% of ")
                    CurrentProgress = output6(0)
                    Dim IntProgress As Integer
                    Dim DecProgress As Decimal = CurrentProgress
                    IntProgress = DecProgress * 10
                ProgressBar1.Value = IntProgress
            End If
            End If
        'Dim test() As String
        ' Dim count As Integer
        'count = 0
        ' Do
        '     ' MsgBox(output.Data.ToString())
        '  test(count) = output.Data.ToString()
        '   count = count + 1
        'Loop Until String.IsNullOrEmpty(output.Data)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Form2.ShowDialog()
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        AppSettings.ShowDialog()
        Dim ReadINI As StreamReader
        If File.Exists(Application.StartupPath & "\config.ini") Then
            ReadINI = New StreamReader(Application.StartupPath & "\config.ini")
            SettingsContents = ReadINI.ReadToEnd()
            ReadINI.Close()
            Dim output1() As String
            output1 = SettingsContents.Split(";")
            If output1(1).Contains("True") Then
                KeepThumbnailMP3 = True
            Else
                KeepThumbnailMP3 = False
            End If
            If output1(2).Contains("True") Then
                PostDownloadCmd = True
            Else
                PostDownloadCmd = False
            End If
            If output1(5).Contains("True") Then
                UseAutoCC = True
            Else
                UseAutoCC = False
            End If
            If output1(4).Contains("True") Then
                UseSubtitles = True
            Else
                UseSubtitles = False
            End If
            If output1(6).Contains("True") Then
                DownloadAllCC = True
            Else
                DownloadAllCC = False
            End If
            Dim output2() As String
            output2 = output1(8).Split("=")
            SaveLocation = output2(1)
        Else
            MessageBox.Show("Cannot load current settings as config.ini cannot be found" & vbCrLf & "or cannot be read due to insufficient permissions", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            KeepThumbnailMP3 = True
            PostDownloadCmd = False
        End If
        If File.Exists(Application.StartupPath & "\command.txt") Then
            ReadINI = New StreamReader(Application.StartupPath & "\command.txt")
            PDCommand = ReadINI.ReadToEnd()
            ReadINI.Close()
        Else
            MessageBox.Show("Cannot load 'Run program after downloading' setting as command.txt cannot be found" & vbCrLf & "or cannot be read due to insufficient permissions", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub
End Class
