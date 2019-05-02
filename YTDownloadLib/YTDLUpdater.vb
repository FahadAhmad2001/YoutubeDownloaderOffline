Imports System.IO
Imports System.Text.RegularExpressions
Public Class YTDLUpdater
    Dim UpdateYT As Process = New Process()
    Dim UpdateYTInfo As ProcessStartInfo = New ProcessStartInfo(Directory.GetCurrentDirectory() & "\youtube-dl.exe")
    Public Event UpdatingDownloader(ByVal version As String)
    Public Sub UpdateYTDL()
        UpdateYTInfo.UseShellExecute = False
        UpdateYTInfo.WindowStyle = ProcessWindowStyle.Hidden
        UpdateYTInfo.CreateNoWindow = True
        UpdateYTInfo.RedirectStandardOutput = True
        UpdateYTInfo.Arguments = "-U"
        UpdateYT.StartInfo = UpdateYTInfo
        AddHandler UpdateYT.OutputDataReceived, AddressOf OutputData
        UpdateYT.Start()
        UpdateYT.BeginOutputReadLine()
        UpdateYT.WaitForExit()
        UpdateYT.Close()
    End Sub
    Private Sub OutputData(sendingProcess As Object, output As DataReceivedEventArgs)
        If Not String.IsNullOrEmpty(output.Data) Then
            If output.Data.Contains("Updating to version ") Then
                Dim output1() As String
                output1 = Regex.Split(output.Data, " version ")
                RaiseEvent UpdatingDownloader(output1(1))
                'MessageBox.Show("Please wait for a moment" & vbCrLf & "Updating downloader to version " & output1(1))
            End If
        End If
    End Sub
End Class
