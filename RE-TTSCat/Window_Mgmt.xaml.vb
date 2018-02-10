Imports System.IO
Imports System.Threading
Imports NAudio.Wave
Imports RE_TTSCat.Consts

Public Class Window_Mgmt
    Public Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub
    Private Sub LoadToControl() Handles Me.Loaded, Button_Reload.Click
        Settings.ReadConf()
        'Tab 1
        CheckBox_Options_PluginEnabled.IsChecked = PluginEnabled
        CheckBox_Options_ReadDanmakuSender.IsChecked = CurrentSettings.TTSDanmakuSender
        CheckBox_Options_ReadGifts.IsChecked = CurrentSettings.TTSGiftsReceived
        CheckBox_Options_ReadTTSInArray.IsChecked = CurrentSettings.ReadInArray
        CheckBox_AutoUpdate.IsChecked = CurrentSettings.AutoUpdEnabled
        'Textboxes
        TextBox_CustomDMContent.Text = CurrentSettings.DanmakuText
        TextBox_CustomGiftContent.Text = CurrentSettings.GiftsText
        TextBox_CustomConnected.Text = CurrentSettings.ConnectSuccessful

        'Tab 2
        ComboBox_Engine.SelectedIndex = CurrentSettings.Engine
        NumericUpDown_Volume.Value = CurrentSettings.TTSVolume
        NumericUpDown_RetryCount.Value = CurrentSettings.DLFailRetry
        NumericUpDown_SpeechSpeed.Value = CurrentSettings.NETFramework_VoiceSpeed
        'Col 2
        Slider_DMLengthLimit.Value = CurrentSettings.MiniumDMLength
        CheckBox_IgnoreTTSVolume.IsChecked = CurrentSettings.IgnoreTTSVolume
        CheckBox_DebugMode.IsChecked = CurrentSettings.DebugMode

        'Tab 3
        ComboBox_Blockmode.SelectedIndex = CurrentSettings.Block_Mode
        TextBox_Blacklist.Text = CurrentSettings.Blacklist
        TextBox_Whitelist.Text = CurrentSettings.Whitelist
        ComboBox_GiftBlockMode.SelectedIndex = CurrentSettings.GiftBlock_Mode
        TextBox_GiftBlacklist.Text = CurrentSettings.GiftBlacklist
        TextBox_GiftWhitelist.Text = CurrentSettings.GiftWhitelist
        TrackBar_BlockType.Value = CurrentSettings.BlockType

        Try
            'Tab 4
            TextBox_Stats.Clear()
            TextBox_Stats.AppendText("收到弹幕总数: " & Stats.EVENT_DanmakuReceived & vbCrLf)
            TextBox_Stats.AppendText("连接成功次数: " & Stats.EVENT_Connected & vbCrLf)
            TextBox_Stats.AppendText("人气值更新次数: " & Stats.EVENT_RoomCountReceived & vbCrLf)
            TextBox_Stats.AppendText("TTS 下载成功次数: " & Stats.SUCCEED_OnTTSDownloaded & vbCrLf)
            TextBox_Stats.AppendText("TTS 播放成功次数: " & Stats.SUCCEED_OnTTSPlayed & vbCrLf)
            TextBox_Stats.AppendText("MainBridge 处理成功次数: " & Stats.SUCCEED_OnMainBridge & vbCrLf)
            TextBox_Stats.AppendText("总计出错次数: " & Stats.FAILURE_ErrorCounter & vbCrLf)
            TextBox_Stats.AppendText("下载失败次数: " & Stats.FAILURE_DownloadFailed & vbCrLf)
            TextBox_Stats.AppendText("播放失败次数: " & Stats.FAILURE_PlayFailed & vbCrLf)
            TextBox_Stats.AppendText("队列中 TTS 数量: " & TTSPlay.PendingTTSes.Count & vbCrLf)
            'Needs some process.
            Dim ErrStr As String = ""
            Try
                If Stats.FAILURE_LatestError.Equals(Nothing) Then
                    ErrStr = "好像还没出错过..."
                Else
                    ErrStr = Stats.FAILURE_LatestError.ToString()
                End If
            Catch ex As Exception When ex.GetType() = GetType(NullReferenceException)
                ErrStr = "好像还没出错过..."
            End Try
            TextBox_Stats.AppendText("最后一次引发的错误: " & ErrStr & vbCrLf)
        Catch ex As Exception
            TextBox_Stats.AppendText("统计出错: " & ex.ToString & vbCrLf)
        End Try

        'Tab 5
        If CurrentSettings.DebugMode Then
            TabItem_Debug.Visibility = Visibility.Visible
        Else
            TabItem_Debug.Visibility = Visibility.Hidden
        End If

        'Tab 6
        Label_AboutTitle.Text = "Re: TTSCat " & Edition & " v" & MainBridge.PluginInstance.PluginVer
        TextBox_Debug.Clear()
        TextBox_Debug.AppendText("---------- OS Environment ----------" & vbCrLf)
        TextBox_Debug.AppendText("Operating System: " & (New Devices.ComputerInfo).OSFullName & vbCrLf)
        TextBox_Debug.AppendText("OS Version: " & (New Devices.ComputerInfo).OSVersion & vbCrLf)
        TextBox_Debug.AppendText("---------- Plugin Environment ----------" & vbCrLf)
        TextBox_Debug.AppendText("Plugin version: " & MainBridge.PluginInstance.PluginVer & vbCrLf)
        TextBox_Debug.AppendText("Plugin edition: " & Edition & vbCrLf)
        TextBox_Debug.AppendText("Plugin executable: " & DLLFile & vbCrLf)
        TextBox_Debug.AppendText("Plugin configuration directory: " & ConfDir & vbCrLf)
        TextBox_Debug.AppendText("Plugins directory: " & DLLPath & vbCrLf)
    End Sub
    Private Sub SaveConf()
        CurrentSettings.TTSDanmakuSender = CheckBox_Options_ReadDanmakuSender.IsChecked
        CurrentSettings.TTSGiftsReceived = CheckBox_Options_ReadGifts.IsChecked
        CurrentSettings.ReadInArray = CheckBox_Options_ReadTTSInArray.IsChecked
        CurrentSettings.AutoUpdEnabled = CheckBox_AutoUpdate.IsChecked
        'Textboxes
        CurrentSettings.DanmakuText = TextBox_CustomDMContent.Text
        CurrentSettings.GiftsText = TextBox_CustomGiftContent.Text
        CurrentSettings.ConnectSuccessful = TextBox_CustomConnected.Text

        'Tab 2
        CurrentSettings.Engine = ComboBox_Engine.SelectedIndex
        CurrentSettings.TTSVolume = Math.Round(NumericUpDown_Volume.Value)
        CurrentSettings.DLFailRetry = Math.Round(NumericUpDown_RetryCount.Value)
        CurrentSettings.NETFramework_VoiceSpeed = Math.Round(NumericUpDown_SpeechSpeed.Value)
        'Col 2
        CurrentSettings.MiniumDMLength = Math.Round(Slider_DMLengthLimit.Value)
        CurrentSettings.IgnoreTTSVolume = CheckBox_IgnoreTTSVolume.IsChecked
        CurrentSettings.DebugMode = CheckBox_DebugMode.IsChecked

        'Tab 3
        CurrentSettings.Block_Mode = ComboBox_Blockmode.SelectedIndex
        CurrentSettings.Blacklist = TextBox_Blacklist.Text
        CurrentSettings.Whitelist = TextBox_Whitelist.Text
        CurrentSettings.GiftBlock_Mode = ComboBox_GiftBlockMode.SelectedIndex
        CurrentSettings.GiftBlacklist = TextBox_GiftBlacklist.Text
        CurrentSettings.GiftWhitelist = TextBox_GiftWhitelist.Text
        CurrentSettings.BlockType = Math.Round(TrackBar_BlockType.Value)

        Try
            Settings.SaveConf(CurrentSettings)
        Catch ex As Exception
            MessageBox.Show("保存失败，如无法自行排错，请联系开发者: " & ex.ToString)
        End Try
    End Sub

    Private Sub Button_Donate_Click(sender As Object, e As RoutedEventArgs) Handles Button_Donate.Click
        Process.Start("https://daily.elepover.com/donate/")
    End Sub

    Private Sub Button_CheckUpd_Click(sender As Object, e As RoutedEventArgs) Handles Button_CheckUpd.Click
        Dim UpdThr As New Threading.Thread(CType(Sub()
                                                     Dim UpdWindow As New Window_Updates
                                                     UpdWindow.ShowDialog()
                                                 End Sub, Threading.ThreadStart))
        UpdThr.SetApartmentState(Threading.ApartmentState.STA)
        UpdThr.Start()
    End Sub

    Private Sub Button_StatusReport_Click(sender As Object, e As RoutedEventArgs) Handles Button_StatusReport.Click
        Dim SRThr As New Threading.Thread(CType(Sub()
                                                    Dim SRWindow As New Window_StatusReport
                                                    SRWindow.ShowDialog()
                                                End Sub, Threading.ThreadStart))
        SRThr.SetApartmentState(Threading.ApartmentState.STA)
        SRThr.Start()
    End Sub

    Private Sub Button_ProxySettings_Click(sender As Object, e As RoutedEventArgs) Handles Button_ProxySettings.Click
        Dim PXThr As New Threading.Thread(CType(Sub()
                                                    Dim PXWindow As New Window_ProxySettings
                                                    PXWindow.ShowDialog()
                                                End Sub, Threading.ThreadStart))
        PXThr.SetApartmentState(Threading.ApartmentState.STA)
        PXThr.Start()
    End Sub

    Private Sub Button_Apply_Click(sender As Object, e As RoutedEventArgs) Handles Button_Apply.Click
        SaveConf()
        LoadToControl()
    End Sub

    Private Sub Button_OK_Click(sender As Object, e As RoutedEventArgs) Handles Button_OK.Click
        SaveConf()
        Me.Close()
    End Sub

    Private Sub Button_Cancel_Click(sender As Object, e As RoutedEventArgs) Handles Button_Cancel.Click
        Me.Close()
    End Sub

    Private Sub Button_About_Click(sender As Object, e As RoutedEventArgs) Handles Button_About.Click
        Process.Start("https://www.danmuji.cn/plugins/Re_TTSCat")
    End Sub

    Private Sub Button_Suggestions_Click(sender As Object, e As RoutedEventArgs) Handles Button_Suggestions.Click
        Process.Start("https://daily.elepover.com/comments/")
    End Sub

    Private Sub SliderHandler(sender As Object, e As RoutedPropertyChangedEventArgs(Of Double)) Handles NumericUpDown_Volume.ValueChanged, NumericUpDown_RetryCount.ValueChanged, NumericUpDown_SpeechSpeed.ValueChanged, Slider_DMLengthLimit.ValueChanged
        Try
            TextBlock_DMLengthLimit.Text = Math.Round(Slider_DMLengthLimit.Value)
            TextBlock_RetryCount.Text = Math.Round(NumericUpDown_RetryCount.Value)
            TextBlock_SpeechSpeed.Text = Math.Round(NumericUpDown_SpeechSpeed.Value)
            TextBlock_Volume.Text = Math.Round(NumericUpDown_Volume.Value)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub Window_Mgmt_IsVisibleChanged(sender As Object, e As DependencyPropertyChangedEventArgs) Handles Me.IsVisibleChanged
        Icon = ConvertToImageSource(My.Resources.avatar, 64, 64)
    End Sub

    Private Sub Button_CheckConnectivity_Click(sender As Object, e As RoutedEventArgs) Handles Button_CheckConnectivity.Click
        Try
            Dim PInstance As New Net.NetworkInformation.Ping
            PInstance.Send("fanyi.baidu.com", 5000)
            PInstance.Send("translate.google.cn", 5000)
            MessageBox.Show("OK", "DEBUG", vbOKOnly, MessageBoxImage.Information)
        Catch ex As Exception
            MessageBox.Show(ex.ToString, "DEBUG", vbOKOnly, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub Button_TestGo_Click(sender As Object, e As RoutedEventArgs) Handles Button_TestGo.Click
        Try
#Disable Warning BC42358 ' 在调用完成之前，会继续执行当前方法，原因是此调用不处于等待状态
            TTSPlay.DLPlayTTS(TextBox_TTSTest.Text)
#Enable Warning BC42358 ' 在调用完成之前，会继续执行当前方法，原因是此调用不处于等待状态
        Catch ex As Exception
            MessageBox.Show(ex.ToString, "DEBUG", vbOKOnly, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub Button_DeleteCache_Click(sender As Object, e As RoutedEventArgs) Handles Button_DeleteCache.Click
        Try
            For Each FL As FileInfo In (New DirectoryInfo(Consts.CacheDir)).GetFiles()
                Dim TSize As Long = FL.Length
                File.Delete(FL.FullName)
            Next
            MessageBox.Show("OK", "DEBUG", vbOKOnly, MessageBoxImage.Information)
        Catch ex As Exception
            MessageBox.Show(ex.ToString, "DEBUG", vbOKOnly, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub Button_ManuallyCrash_Click(sender As Object, e As RoutedEventArgs) Handles Button_ManuallyCrash.Click
        Throw New NullReferenceException
    End Sub

    Private Sub Button_PlayAudio_Click(sender As Object, e As RoutedEventArgs) Handles Button_PlayAudio.Click
        Try
            Dim Filename As String = InputBox("Filename: ", "DEBUG")
            Using audioFile = New AudioFileReader(Filename)
                Using outputDevice = New WaveOutEvent()
                    outputDevice.Init(audioFile)
                    outputDevice.Play()
                    While outputDevice.PlaybackState = PlaybackState.Playing
                        Thread.Sleep(1000)
                    End While
                End Using
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.ToString, "DEBUG", vbOKOnly, MessageBoxImage.Error)
        End Try
    End Sub

    Private Sub Button_StartDebugger_Click(sender As Object, e As RoutedEventArgs) Handles Button_StartDebugger.Click
        Debugger.Launch()
    End Sub
End Class
