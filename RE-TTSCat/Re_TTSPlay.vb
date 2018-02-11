' Separated as two different modules.
' Mod. 1: TTSPlay
' Mod. 2: TTSDownload
' Two modules only have communications over 'List PendingTTSes(Of String)'
'
' Mod. 1: Check list and play.
' Mod. 2: Download and add to list.
'
' Both modules are required to check if 'ReadInArray' is enabled.

Imports NAudio.Wave

Public Class Re_TTSPlay
    Public Shared TTSPlayer As Threading.Thread
    Public Shared PendingTTSes As New List(Of String)
    ''' <summary>
    ''' Check if Worker has started.
    ''' </summary>
    ''' <returns></returns>
    Private Shared ReadOnly Property WorkerStarted As Boolean
        Get
            Try
                Return TTSPlayer.IsAlive
            Catch ex As Exception When (ex.GetType = GetType(NullReferenceException))
                Return False
            End Try
        End Get
    End Property
    Public Shared Sub StartWorker()
        If Not WorkerStarted Then
            TTSPlayer = New Threading.Thread(AddressOf ThrPlayTTS)
            TTSPlayer.Start()
        End If
    End Sub
    Private Shared Sub ThrPlayTTS()
        'Enter cycle
        'ONLY PLAYS WHEN PLUGIN IS ACTIVE.
        While (True)
            If Consts.PluginEnabled = False Then
                Delay(1990)
                GoTo ExitCycle
            End If
            While (Not PendingTTSes.Count = 0)
                Try
                    PlayTTS(PendingTTSes(0))
                    Stats.SUCCEED_OnTTSPlayed += 1
                Catch ex As Exception
                    Stats.FAILURE_ErrorCounter += 1
                    Stats.FAILURE_LatestError = ex
                Finally
                    PendingTTSes.RemoveAt(0)
                End Try
            End While
ExitCycle:
            Delay(100)
        End While
    End Sub
    ''' <summary>
    ''' Play TTS file.
    ''' </summary>
    ''' <param name="Filename">File to play.</param>
    Private Shared Sub PlayTTS(Filename As String)
        'Play and wait for done.
        Using FileReader = New AudioFileReader(Filename)
            Using WaveOutDevice = New WaveOutEvent()
                WaveOutDevice.Init(FileReader)
                WaveOutDevice.Play()
                While (Not WaveOutDevice.PlaybackState = PlaybackState.Stopped)
                    Delay(500)
                End While
            End Using
        End Using
    End Sub
    ''' <summary>
    ''' Download TTS And Add To List.
    ''' </summary>
    ''' <param name="Text">TTS Text.</param>
    ''' <param name="Engine">TTS Engine.</param>
    ''' <param name="Retry">Retrival Count.</param>
    Public Shared Sub DownloadTTS(Text As String, PlayInArray As Boolean, Optional Engine As Short = 0, Optional Retry As Integer = 5)
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
                        'EXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXIT
                        Exit Sub
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
                    'EXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXIT
                    Exit Sub
                End Try
                DLTimer.Stop()
                L("下载成功，耗时 " & DLTimer.ElapsedMilliseconds & "ms.", True)
                Stats.SUCCEED_OnTTSDownloaded += 1
                If Not PlayInArray Then
                    PlayTTS(FullFileName)
                Else
                    PendingTTSes.Add(FullFileName)
                End If
                Exit Sub
            Case 1
                ' Generate TTS
                Try
                    Dim Outputter As New Speech.Synthesis.SpeechSynthesizer()
                    Outputter.SetOutputToWaveFile(FullFileName.Replace(".mp3", ".wav"))
                    Outputter.SpeakAsync(Text)
                    Delay(3000)
                    Stats.SUCCEED_OnTTSDownloaded += 1
                    If Not PlayInArray Then
                        PlayTTS(FullFileName)
                    Else
                        PendingTTSes.Add(FullFileName)
                    End If
                    Exit Sub
                Catch ex As Exception
                    Stats.FAILURE_ErrorCounter += 1
                    Stats.FAILURE_LatestError = ex
                    L(".NET 框架错误: " & ex.Message)
                    'EXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXIT
                    Exit Sub
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
                        'EXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXIT
                        Exit Sub
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
                    'EXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXIT
                    Exit Sub
                End Try
                DLTimer.Stop()
                L("下载成功，耗时 " & DLTimer.ElapsedMilliseconds & "ms.", True)
                Stats.SUCCEED_OnTTSDownloaded += 1
                If Not PlayInArray Then
                    PlayTTS(FullFileName)
                Else
                    PendingTTSes.Add(FullFileName)
                End If
                Exit Sub
            Case Else
                L("退出: 无效引擎。", True)
                'EXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXIT
                Exit Sub
        End Select
    End Sub
    ''' <summary>
    ''' Check eligibility of a message. If any 'type' is wrong, returns true.
    ''' ONLY CAN BE USED WHEN SETTINGS SYSTEM IS READY!
    ''' </summary>
    ''' <param name="Sender">Message sender.</param>
    ''' <param name="Content">Message content.</param>
    ''' <param name="Type">Message type.</param>
    ''' <returns></returns>
    Public Shared Function CheckEligibility(Sender As String, Content As String, Type As BilibiliDM_PluginFramework.MsgTypeEnum) As Boolean
        Select Case Type
            Case BilibiliDM_PluginFramework.MsgTypeEnum.Comment
                'Detect user block type.
                Select Case Consts.CurrentSettings.Block_Mode
                    Case 0 'Disabled
                        Return True
                    Case 1 'Blacklist
                        For Each TmpStr As String In Consts.CurrentSettings.Blacklist.Replace(vbCr, "").Split({vbLf}, StringSplitOptions.RemoveEmptyEntries)
                            If TmpStr = Sender Then Return False
                        Next
                        Return True
                    Case 2 'Whitelist
                        'Just opposite to blacklist mode.
                        For Each TmpStr As String In Consts.CurrentSettings.Whitelist.Replace(vbCr, "").Split({vbLf}, StringSplitOptions.RemoveEmptyEntries)
                            If TmpStr = Sender Then Return True
                        Next
                        Return False
                    Case Else 'Not happens
                        Return True
                End Select
            Case BilibiliDM_PluginFramework.MsgTypeEnum.GiftSend
                'Check if gifts tts is enabled.
                If Not Consts.CurrentSettings.TTSGiftsReceived Then
                    Return False
                End If
                'Recursively check if sender is eligible or not.
                If CheckEligibility(Sender, "", BilibiliDM_PluginFramework.MsgTypeEnum.Comment) Then
                    Select Case Consts.CurrentSettings.GiftBlock_Mode
                        Case 0 'Disable
                            Return True
                        Case 1 'Blacklist
                            For Each TmpStr As String In Consts.CurrentSettings.GiftBlacklist.Replace(vbCr, "").Split({vbLf}, StringSplitOptions.RemoveEmptyEntries)
                                If TmpStr = Content Then Return False
                            Next
                            Return True
                        Case 2 'Whitelist
                            For Each TmpStr As String In Consts.CurrentSettings.GiftWhitelist.Replace(vbCr, "").Split({vbLf}, StringSplitOptions.RemoveEmptyEntries)
                                If TmpStr = Content Then Return True
                            Next
                            Return False
                        Case Else 'Not happens.
                            Return True
                    End Select
                Else
                    Return False
                End If
            Case Else
                Return True
        End Select
    End Function
End Class
