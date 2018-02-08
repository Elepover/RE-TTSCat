Imports System.Threading
Imports NAudio.Wave

Public Class TTSPlay
    Public Shared WaveOutDevice As WaveOutEvent

    ''' <summary>
    ''' Plays TTS with provided filename.
    ''' </summary>
    ''' <param name="AudioFile"></param>
    Public Sub PlayTTS(AudioFile As String)
        Using FileReader = New AudioFileReader(AudioFile)
            Using WaveOutDevice = New WaveOutEvent()
                WaveOutDevice.Init(FileReader)
                WaveOutDevice.Play()
            End Using
        End Using
    End Sub
    ''' <summary>
    ''' Downloads/Generates TTS file following selected source and returns filename.
    ''' </summary>
    ''' <param name="Text">Text to speak.</param>
    ''' <param name="Engine">Engine Selection.</param>
    ''' <param name="Retry">Retry Option.</param>
    ''' <returns>Filename.</returns>
    Public Function DownloadTTS(Text As String, Optional Engine As Short = 0, Optional Retry As Integer = 5) As String
        Dim DLTimer As New Stopwatch()
        DLTimer.Start()
        Dim PartialFilename As String = $"TTS{(New Random()).Next(10000000, 49999999)}{(New Random()).Next(49999999, 99999999)}.mp3"
        Dim FullFileName As String = IO.Path.Combine(Consts.CacheDir, PartialFilename)

        ' Proxy
#Disable Warning BC40000 'mmp
        Dim Proxy As Net.WebProxy = Net.WebProxy.GetDefaultProxy
#Enable Warning BC40000
        Dim UseProxy As Boolean = True
        If Consts.CurrentSettings.ProxySettings_ProxyServer = "" Then
            UseProxy = False
        End If
        If UseProxy Then
            Proxy = New System.Net.WebProxy(Consts.CurrentSettings.ProxySettings_ProxyServer, Consts.CurrentSettings.ProxySettings_ProxyPort)
        End If

        ' Retrieval
        Dim RetryCount As Integer = 0
        Retry = Consts.CurrentSettings?.DLFailRetry

        Select Case Engine
            Case 0
Retrieval:
                Try
                    Dim DLclient As New Net.WebClient()
                    If UseProxy Then
                        DLclient.Proxy = Proxy
                    End If
                    DLclient.DownloadFile(Settings.Conf.APIString.Replace("$TTSTEXT", Text), FullFileName)
                Catch ex As Exception
                    Stats.FAILURE_LatestError = ex
                    Stats.FAILURE_ErrorCounter += 1
                    'Retry
                    If Retry = 0 Then
                        Stats.FAILURE_DownloadFailed += 1
                        L("下载失败，丢弃。（未启用自动重下）", True)
                        Return "$FAIL"
                    End If
                    If RetryCount < Retry Then
                        RetryCount += 1
                        L("下载失败: " & ex.Message & "；将在 1 秒后执行第 " & RetryCount & " 次重试。", True)
                        Stats.FAILURE_DownloadFailed += 1
                        Delay(1000)
                        GoTo Retrieval
                    End If
                    If RetryCount = Retry Then
                        RetryCount += 1
                        L("下载失败: " & ex.Message & "；即将在 1 秒后执行最后一次重试。", True)
                        Stats.FAILURE_DownloadFailed += 1
                        Delay(1000)
                        GoTo Retrieval
                    End If
                    'Over 5 times
                    L("在重试 " & Retry & " 次以后，TTS 下载失败: " & ex.Message)
                    Return "$FAIL"
                End Try
                DLTimer.Stop()
                L("下载成功，耗时 " & DLTimer.ElapsedMilliseconds & "ms.", True)
            Case 1

            Case 2

        End Select
    End Function
End Class
