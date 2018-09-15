Imports System.Text.RegularExpressions
Imports System.Net
Imports System.IO
Imports System.Threading
Public Class Form3
    Dim AllMissingFiles As String
    Dim MissingFiles() As String
    Dim WithEvents FilesDownloader As New WebClient
    Dim CurrentLabel As String
    Dim DownloadRunning As Boolean
    Dim CurrentFile As String
    Dim TempFiles() As String = {"s", "s"}
    Private Sub Form3_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AllMissingFiles = Form1.MissingFiles
        MissingFiles = AllMissingFiles.Split(":")
        AddHandler FilesDownloader.DownloadProgressChanged, AddressOf DownloadProgressChanged
        AddHandler FilesDownloader.DownloadFileCompleted, AddressOf DownloadCompleted
        DownloadRunning = False
    End Sub

    Private Sub Form3_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        '    For Each item As String In MissingFiles
        '   If String.IsNullOrEmpty(item) = False Then
        '  'MsgBox(item)
        ' DownloadRunning = False
        '     CurrentLabel = "Downloading: " & item
        '    Label1.Text = CurrentLabel
        '   Dim DownloadURL As Uri = New Uri("http://serverwebsite.ddns.net:500/youtubedownload/files/" & item)
        '
        '     FilesDownloader.DownloadFileAsync(DownloadURL, Application.StartupPath & "\" & item)
        '    'MsgBox("Download started")
        '   DownloadRunning = True
        'CheckForCompletion:
        '       FilesDownloader.
        '      End If
        '     Next
        MissingFilesDownloader()

    End Sub
    Public Sub MissingFilesDownloader()
        CurrentFile = MissingFiles(0)
        If String.IsNullOrEmpty(CurrentFile) = False Then
            If CurrentFile.Contains("END") = False Then
                CurrentLabel = "Downloading: " & CurrentFile
                Dim DownloadURL As Uri = New Uri("http://lightspeedmedia.tk:500/youtubedownload/files/" & CurrentFile)
                FilesDownloader.DownloadFileAsync(DownloadURL, Application.StartupPath & "\" & CurrentFile)
            Else
                Me.Close()
            End If
        Else
                'Me.Close()
            End If
    End Sub
    Public Sub DownloadProgressChanged(sender As Object, e As DownloadProgressChangedEventArgs)
        ProgressBar1.Value = e.ProgressPercentage
        Label1.Text = CurrentLabel & " - " & e.ProgressPercentage & "%"
    End Sub
    Public Sub DownloadCompleted()
        Dim count As Integer
        count = 0
        Array.Clear(TempFiles, 0, TempFiles.Length)
        Array.Copy(MissingFiles, TempFiles, MissingFiles.Rank)
        'Array.Clear(MissingFiles, 0, 1)
        For Each item As String In MissingFiles
            If item = CurrentFile Then
                count = count - 1
                'Array.Clear(MissingFiles, count, 1)
            Else
                MissingFiles(count) = item
            End If
            count = count + 1
        Next
        If MissingFiles.Rank = 0 Then
            Me.Close()
        End If
        MissingFilesDownloader()
    End Sub

    Private Sub Label1_Click(sender As Object, e As EventArgs) Handles Label1.Click

    End Sub
End Class