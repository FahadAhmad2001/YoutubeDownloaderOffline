Imports System.IO
Public Class AppSettings
    Dim INIContents As String
    Dim EmbedVidThumbnail As Boolean
    Dim UseSubtitles As Boolean
    Dim UseCommand As Boolean
    Dim CommandString As String
    Dim UseAutoCC As Boolean
    Dim DownloadAllCC As Boolean
    Dim CommandContents As String
    Dim SaveLocation As String
    Private Sub AppSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim ReadINI As StreamReader
        CheckBox5.Enabled = False
        ComboBox1.Enabled = False
        If File.Exists(Application.StartupPath & "\config.ini") Then
            ReadINI = New StreamReader(Application.StartupPath & "\config.ini")
            INIContents = ReadINI.ReadToEnd()
            ReadINI.Close()
            Dim output1() As String
            output1 = INIContents.Split(";")
            If output1(1).Contains("True") Then
                EmbedVidThumbnail = True
                CheckBox1.CheckState = CheckState.Checked
            Else
                EmbedVidThumbnail = False
                CheckBox1.CheckState = CheckState.Unchecked
            End If
            If output1(2).Contains("True") Then
                UseCommand = True
                TextBox1.Enabled = True
                CheckBox3.CheckState = CheckState.Checked
            Else
                UseCommand = False
                TextBox1.Enabled = False
                CheckBox3.CheckState = CheckState.Unchecked
            End If
            If output1(4).Contains("True") Then
                UseSubtitles = True
                CheckBox6.CheckState = CheckState.Checked
                CheckBox2.Enabled = True
                CheckBox4.Enabled = True
            Else
                UseSubtitles = False
                CheckBox6.CheckState = CheckState.Unchecked
                CheckBox2.Enabled = False
                CheckBox4.Enabled = False
            End If
            If output1(5).Contains("True") Then
                If CheckBox2.Enabled = True Then
                    CheckBox2.CheckState = CheckState.Checked
                Else
                    CheckBox2.Enabled = True
                    CheckBox2.CheckState = CheckState.Checked
                    CheckBox2.Enabled = False
                End If
                UseAutoCC = True
            Else
                If CheckBox2.Enabled = True Then
                    CheckBox2.CheckState = CheckState.Unchecked
                Else
                    CheckBox2.Enabled = True
                    CheckBox2.CheckState = CheckState.Unchecked
                    CheckBox2.Enabled = False
                End If
                UseAutoCC = False
            End If
            If output1(6).Contains("True") Then
                If CheckBox4.Enabled = True Then
                    CheckBox4.CheckState = CheckState.Checked
                Else
                    CheckBox4.Enabled = True
                    CheckBox4.CheckState = CheckState.Checked
                    CheckBox4.Enabled = False
                End If
                DownloadAllCC = True
            Else
                If CheckBox4.Enabled = True Then
                    CheckBox4.CheckState = CheckState.Unchecked
                Else
                    CheckBox4.Enabled = True
                    CheckBox4.CheckState = CheckState.Unchecked
                    CheckBox4.Enabled = False
                End If
                DownloadAllCC = False
            End If
            Dim output2() As String
            output2 = output1(8).Split("=")
            SaveLocation = output2(1)
            If SaveLocation = "Documents" Then
                Label4.Text = "Files saved in : My Documents"
            Else
                Label4.Text = "Files saved in: " & SaveLocation
            End If
        Else
                MessageBox.Show("Cannot load current settings as config.ini cannot be found" & vbCrLf & "or cannot be read due to insufficient permissions", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            TextBox1.Enabled = False
            CheckBox2.Enabled = False
            CheckBox4.Enabled = False
            CheckBox5.Enabled = False
        End If
        If File.Exists(Application.StartupPath & "\command.txt") Then
            ReadINI = New StreamReader(Application.StartupPath & "\command.txt")
            CommandContents = ReadINI.ReadToEnd()
            ReadINI.Close()
            If TextBox1.Enabled = True Then
                TextBox1.Text = CommandContents
            Else
                TextBox1.Enabled = True
                TextBox1.Text = CommandContents
                TextBox1.Enabled = False
            End If
        Else
            MessageBox.Show("Cannot load 'Run program after downloading' setting as command.txt cannot be found" & vbCrLf & "or cannot be read due to insufficient permissions", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Dim NewINIContents As String
        NewINIContents = "GenSettings:;" & vbCrLf
        If EmbedVidThumbnail = True Then
            NewINIContents = NewINIContents & vbCrLf & "UseVidThumbnail=True;"
        Else
            NewINIContents = NewINIContents & vbCrLf & "UseVidThumbnail=False;"
        End If
        If UseCommand = True Then
            NewINIContents = NewINIContents & vbCrLf & "UsePostDownloadCmd=True;" & vbCrLf & vbCrLf
            Dim WriteCmd As StreamWriter
            If File.Exists(Application.StartupPath & "\command.txt") Then
                File.Delete(Application.StartupPath & "\command.txt")
            End If
            WriteCmd = New StreamWriter(Application.StartupPath & "\command.txt")
            WriteCmd.Write(TextBox1.Text)
            WriteCmd.Close()
        Else
            NewINIContents = NewINIContents & vbCrLf & "UsePostDownloadCmd=False;" & vbCrLf & vbCrLf
        End If
        NewINIContents = NewINIContents & "Subtitles;" & vbCrLf & vbCrLf
        If UseSubtitles = True Then
            NewINIContents = NewINIContents & "UseSubtitles=True;" & vbCrLf
        Else
            NewINIContents = NewINIContents & "UseSubtitles=False;" & vbCrLf
        End If
        If UseAutoCC = True Then
            NewINIContents = NewINIContents & "UseAutoCC=True;" & vbCrLf
        Else
            NewINIContents = NewINIContents & "UseAutoCC=False;" & vbCrLf
        End If
        If DownloadAllCC = True Then
            NewINIContents = NewINIContents & "DownloadAllCC=True;"
        Else
            NewINIContents = NewINIContents & "DownloadAllCC=False;"
        End If
        NewINIContents = NewINIContents & vbCrLf & vbCrLf & "SaveLocation;" & vbCrLf & vbCrLf & "SaveLocation=" & SaveLocation & ";"

        If File.Exists(Application.StartupPath & "\config.ini") Then
            File.Delete(Application.StartupPath & "\config.ini")
        End If
        Dim WriteSettings As StreamWriter
        WriteSettings = New StreamWriter(Application.StartupPath & "\config.ini")
        WriteSettings.Write(NewINIContents)
        WriteSettings.Close()
        Me.Close()
    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            EmbedVidThumbnail = True
        Else
            EmbedVidThumbnail = False
        End If
    End Sub

    Private Sub CheckBox3_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox3.CheckedChanged
        If CheckBox3.Checked = True Then
            UseCommand = True
            TextBox1.Enabled = True
        Else
            UseCommand = False
            TextBox1.Enabled = False
        End If
    End Sub

    Private Sub CheckBox6_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox6.CheckedChanged
        If CheckBox6.Checked = True Then
            CheckBox2.Enabled = True
            CheckBox4.Enabled = True
            'CheckBox5.Enabled = True
            UseSubtitles = True
        Else
            CheckBox2.Enabled = False
            CheckBox4.Enabled = False
            'CheckBox5.Enabled = False
            UseSubtitles = False
        End If
    End Sub

    Private Sub CheckBox2_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox2.CheckedChanged
        If CheckBox2.Checked Then
            UseAutoCC = True
        Else
            UseAutoCC = False
        End If
    End Sub

    Private Sub CheckBox4_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox4.CheckedChanged
        If CheckBox4.Checked = True Then
            DownloadAllCC = True
        Else
            DownloadAllCC = False
        End If
    End Sub

    Private Sub Label2_Click(sender As Object, e As EventArgs) Handles Label2.Click

    End Sub

    Private Sub Label4_Click(sender As Object, e As EventArgs) Handles Label4.Click

    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        If FolderBrowserDialog1.ShowDialog() = DialogResult.OK Then
            SaveLocation = FolderBrowserDialog1.SelectedPath
            Label4.Text = "Files saved in: " + SaveLocation
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        SaveLocation = "Documents"
        Label4.Text = "Files saved in: My Documents"
    End Sub
End Class