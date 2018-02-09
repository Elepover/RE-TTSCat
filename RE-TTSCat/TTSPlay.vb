Imports System.Threading
Imports NAudio.Wave

Public Class TTSPlay
    Public Shared WithEvents WaveOutDevice As WaveOutEvent
    ''' <summary>
    ''' A collection of filenames of Pending TTS files.
    ''' </summary>
    Public Shared PendingTTSes As New List(Of String)
    ''' <summary>
    ''' Download TTS and Play! Returning whether succeed or not.
    ''' </summary>
    ''' <param name="Content"></param>
    Public Shared Async Function DLPlayTTS(Content As String, Optional TestMode As Boolean = False) As Task(Of Boolean)
        Try
            'If testmode, then play directly (no sound)
            If TestMode Then
                Try
                    PlayTTS(Await DownloadTTS("这是一个中文语音合成的例子。", 0, 5), True, True)
                    Return True
                Catch ex As Exception
                    Return False
                End Try
            End If
            'If not playing in row, play directly.
            If Not Consts.CurrentSettings.ReadInArray Then
                PlayTTS(Await DownloadTTS(Content, Consts.CurrentSettings.Engine, Consts.CurrentSettings.DLFailRetry),, True)
                Return True
            Else
                'If playing in row, check extra stuff.
                'If already empty, add to list and start playing.
                If PendingTTSes.Count = 0 Then
                    PendingTTSes.Add(Await DownloadTTS(Content, Consts.CurrentSettings.Engine, Consts.CurrentSettings.DLFailRetry))
                    PlayTTS(PendingTTSes(0))
                    Return True
                Else 'If Not, just add to list.
                    PendingTTSes.Add(Await DownloadTTS(Content, Consts.CurrentSettings.Engine, Consts.CurrentSettings.DLFailRetry))
                    Return True
                End If
            End If
        Catch ex As Exception
            Stats.FAILURE_ErrorCounter += 1
            Stats.FAILURE_LatestError = ex
            Return False
        End Try
    End Function

    Private Shared Sub CALLBACK_PlaybackStopped()
        'Remove in playlist.
        PendingTTSes.RemoveAt(0)
        'Check if playlist is empty.
        If Not PendingTTSes.Count = 0 Then
            PlayTTS(PendingTTSes(0))
        End If
    End Sub

    ''' <summary>
    ''' Plays TTS with provided filename.
    ''' </summary>
    ''' <param name="AudioFile"></param>
    Private Shared Sub PlayTTS(AudioFile As String, Optional Silent As Boolean = False, Optional NoCallBack As Boolean = False)
        Using FileReader = New AudioFileReader(AudioFile)
            WaveOutDevice = New WaveOutEvent()
            WaveOutDevice.Init(FileReader)
            If Not Silent Then
                WaveOutDevice.Play()
            End If
            While (Not WaveOutDevice.PlaybackState = PlaybackState.Stopped)
                Delay(500)
            End While
            WaveOutDevice.Dispose()
            If Not NoCallBack Then CALLBACK_PlaybackStopped()
        End Using
    End Sub
    ''' <summary>
    ''' Downloads/Generates TTS file following selected source and returns filename.
    ''' </summary>
    ''' <param name="Text">Text to speak.</param>
    ''' <param name="Engine">Engine Selection.</param>
    ''' <param name="Retry">Retry Option.</param>
    ''' <returns>Filename.</returns>
#Disable Warning BC42356 ' 此异步方法缺少 "Await" 运算符，因此将以同步方式运行
    Private Shared Async Function DownloadTTS(Text As String, Optional Engine As Short = 0, Optional Retry As Integer = 5) As Task(Of String)
#Enable Warning BC42356 ' 此异步方法缺少 "Await" 运算符，因此将以同步方式运行
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
        Retry = Consts.CurrentSettings.DLFailRetry

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
                Return FullFileName
            Case 1
                ' Generate TTS
                Try
                    Dim Outputter As New Speech.Synthesis.SpeechSynthesizer()
                    Outputter.SetOutputToWaveFile(FullFileName.Replace(".mp3", ".wav"))
                    Outputter.Speak(Text)
                    Return FullFileName
                Catch ex As Exception
                    Stats.FAILURE_ErrorCounter += 1
                    Stats.FAILURE_LatestError = ex
                    L(".NET 框架错误: " & ex.Message)
                    Return "$FAIL"
                End Try
            Case 2
Retrieval_GG:
                Try
                    If UseProxy Then
                        Google.TTS.TTSHelper.GerarArquivo(FullFileName, Text, Google.TTS.TTSLang.Chinese, Consts.CurrentSettings.UseGoogleGlobal, True, Proxy)
                    Else
                        Google.TTS.TTSHelper.GerarArquivo(FullFileName, Text, Google.TTS.TTSLang.Chinese, Consts.CurrentSettings.UseGoogleGlobal)
                    End If
                Catch ex As Exception '这块的代码和上面一模一样
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
                Return FullFileName
            Case Else
                Return "$FAIL"
        End Select
    End Function
End Class
