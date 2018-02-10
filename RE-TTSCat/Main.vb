Imports System.IO
Imports System.Threading
Imports System.Windows.Threading
Imports BilibiliDM_PluginFramework

Public Class Main
    Inherits DMPlugin
    Public Sub New()
        PluginAuth = "Elepover"
        PluginName = "Re: TTSCat"
        PluginCont = "elepover@outlook.com"
        PluginVer = "2.0.0.1"
        PluginDesc = "TTSDanmaku 重写版（读弹幕姬）"
    End Sub
#Region "MainBridge"
    Private MainBridgeSynchronizer As Thread
    Private SRThread As Thread
    Private UserCountLatest As Integer = 0
    Private Sub MainBridgeSynchronize()
        While True
            Try
                ' Priority: ReqStart > ReqStop > ReqAdmin > ReqExit > Logs
                If MainBridge.PendingLogs.Count > 0 Then
                    While Not MainBridge.PendingLogs.Count = 0
                        Dim CLog As Log = MainBridge.PendingLogs(0)
                        If CLog.DebugOnly Then
                            If Consts.CurrentSettings.DebugMode Then
                                Log(CLog.LogContent)
                            End If
                        Else
                            Log(CLog.LogContent)
                        End If
                        MainBridge.PendingLogs.RemoveAt(0)
                    End While
                End If
            Catch ex As Exception
                MessageBox.Show(ex.ToString, "Re: TTSCat - MainBridge")
            End Try
            Stats.SUCCEED_OnMainBridge += 1
            Thread.Sleep(100)
        End While
    End Sub
#End Region
    Public Sub ThrStatusReport()
        Do Until 233 = 2333
            '注意检测插件是否启用
            L("状态报告已开始计时: " & Consts.CurrentSettings.StatusReportInterval, True)
            Thread.Sleep(Consts.CurrentSettings.StatusReportInterval * 1000)
            L("计时达到，检查环境。", True)
            If Consts.CurrentSettings.StatusReport = False Then GoTo DLoop
            If Consts.PluginEnabled = False Then GoTo DLoop
            Dim content As String = Consts.CurrentSettings.StatusReportContent
            If Consts.CurrentSettings.StatusReport_ResolveAdvVars Then
                content = content.Replace("$ONLINE", UserCountLatest).Replace("$HOUR", Now.Hour).Replace("$MINUTE", Now.Minute).Replace("$SEC", Now.Second).Replace("$YEAR", Now.Year).Replace("$MONTH", Now.Month).Replace("$DAY", Now.Day).Replace("$MEMAVAI", Math.Round(((New Devices.ComputerInfo).AvailablePhysicalMemory / 1024 / 1024 / 1024), 2)).Replace("$MPERCENT", Math.Round((((New Devices.ComputerInfo).TotalPhysicalMemory - (New Devices.ComputerInfo).AvailablePhysicalMemory) / (New Devices.ComputerInfo).TotalPhysicalMemory) * 100, 2)).Replace("$VMEM", Math.Round(((New Devices.ComputerInfo).AvailableVirtualMemory / 1024 / 1024 / 1024 / 1024), 2)).Replace("$VPERCENT_VM", Math.Round((((New Devices.ComputerInfo).TotalVirtualMemory - (New Devices.ComputerInfo).AvailableVirtualMemory) / (New Devices.ComputerInfo).TotalVirtualMemory) * 100, 2))
            Else
                content = content.Replace("$ONLINE", UserCountLatest)
            End If
#Disable Warning BC42358 ' 在调用完成之前，会继续执行当前方法，原因是此调用不处于等待状态
            TTSPlay.DLPlayTTS(content)
#Enable Warning BC42358 ' 在调用完成之前，会继续执行当前方法，原因是此调用不处于等待状态
DLoop:
        Loop
    End Sub
    Public Overrides Sub Start()
        Log("启动插件中...")
        If MainBridge.StartError Then
            Log("拒绝启动: 插件激活过程中出现问题。")
            Exit Sub
        End If
        MyBase.Start()
        Try
            Settings.Init()
        Catch ex As Exception
            Log("启动失败: " & ex.Message)
            Exit Sub
        End Try
        Consts.PluginEnabled = True
        Log("启动成功！")
    End Sub

    Public Async Sub ThrStartPlugin()
        Dim WindowObj As New Window_Loading
        WindowObj.Show()
        'Delay(500)
        WindowObj.TextBox_Status.Text = "正在准备设置..."

        'Detect NAudio
        If Not File.Exists(Consts.AudioLibFile) Then
            MessageBox.Show("警告: NAudio 播放库文件丢失，请从弹幕姬插件仓库中插件页面下载 NAudio 播放库，并与插件置于同一文件夹后再试一次。" & vbCrLf & vbCrLf & "已取消插件激活。" & vbCrLf & vbCrLf & "您关闭弹幕姬时将可能会遇到一个错误，当您补回 NAudio 播放库后，重新启动弹幕姬，问题即可解决。", "Re: TTSCat", vbOKOnly, MessageBoxImage.Warning)
            MainBridge.StartError = True
            MainBridge.Started = True
            Exit Sub
        End If

        Dim Frame = New DispatcherFrame()
        Dim Thr As New Thread(CType((Sub()
                                         Settings.Init()
                                         Frame.[Continue] = False
                                     End Sub), ThreadStart))
        Thr.Start()
        Dispatcher.PushFrame(Frame)

        Delay(100)
        WindowObj.TextBox_Status.Text = "正在测试网络..."
        If Await TTSPlay.DLPlayTTS("这是一个中文语音合成的例子。", True) = False Then
            MessageBox.Show("网络测试失败，可能会遇到一些玄学问题。", "Re: TTSCat", vbOKOnly, MessageBoxImage.Warning)
        End If

        Delay(100)
        WindowObj.TextBox_Status.Text = "正在清理缓存..."
        Dim CacheCleared As Integer = 0
        Dim CacheSize As Single = 0
        Dim Frame_Cache = New DispatcherFrame()
        Dim Thr_Cache As New Thread(CType((Sub()
                                               Try
                                                   For Each FL As FileInfo In (New DirectoryInfo(Consts.CacheDir)).GetFiles()
                                                       Dim TSize As Long = FL.Length
                                                       File.Delete(FL.FullName)
                                                       CacheCleared += 1
                                                       CacheSize += TSize 'Only adds length when succeed.
                                                   Next
                                               Catch ex As Exception
                                               End Try
                                               Frame_Cache.[Continue] = False
                                           End Sub), ThreadStart))
        Thr_Cache.Start()
        Dispatcher.PushFrame(Frame_Cache)

        CacheSize = CacheSize / 1024 / 1024
        L("已清理 " & CacheCleared & " 个缓存文件，释放了 " & CacheSize & " MiB 的存储空间。")

        MainBridge.Started = True
        WindowObj.TextBox_Status.Text = "完成！"
        Delay(100)
        WindowObj.Close()
    End Sub

    Public Overrides Sub [Stop]()
        Log("停止插件中...")
        If MainBridge.StartError Then
            Log("拒绝操作: 插件激活过程中出现问题。")
        End If
        MyBase.Stop()
        Consts.PluginEnabled = False
    End Sub

    Public Overrides Sub Admin()
        If MainBridge.StartError Then
            Log("拒绝操作: 插件激活过程中出现问题。")
            Exit Sub
        End If
        Dim ThrADM As New Thread(CType((Sub()
                                            Try
                                                Dim TmpWindow As New Window_Mgmt
                                                TmpWindow.ShowDialog()
                                            Catch ex As Exception
                                            End Try
                                        End Sub), ThreadStart))
        ThrADM.SetApartmentState(ApartmentState.STA)
        ThrADM.Start()
        MyBase.Admin()
    End Sub

    Public Overrides Sub Inited()
        MyBase.Inited()
        'Initialize All Stuff
        MainBridge.Started = False
        MainBridge.StartError = False
        Consts.StartedOnce = True
        Consts.DMJWindow = Application.Current.MainWindow
        MainBridge.PluginInstance = Me
        Net.ServicePointManager.SecurityProtocol = Net.SecurityProtocolType.Tls12
        'Start thread
        Dim LoadThr As New Thread(AddressOf ThrStartPlugin)
        LoadThr.SetApartmentState(ApartmentState.STA)
        LoadThr.Start()
        While (Not MainBridge.Started)
            Delay(250)
        End While
        ' Post-initialization
        If MainBridge.StartError Then
            Exit Sub
        End If
        MainBridgeSynchronizer = New Thread(AddressOf MainBridgeSynchronize)
        MainBridgeSynchronizer.SetApartmentState(ApartmentState.STA)
        MainBridgeSynchronizer.Start()
        SRThread = New Thread(AddressOf ThrStatusReport)
        SRThread.SetApartmentState(ApartmentState.STA)
        SRThread.Start()
        Consts.TrayWindow = New Form_TrayKeeper
        Consts.TrayWindow.Show() 'Tray requires configurations and MainBridge. So we need to start it later than conf. system.
    End Sub

    Public Overrides Sub DeInit()
        On Error Resume Next
        MyBase.DeInit()
        If (Consts.StartedOnce And (Not MainBridge.StartError)) Then Consts.TrayWindow.NotifyIcon_Default.Visible = False
        Process.GetCurrentProcess.Kill()
    End Sub
#Disable Warning BC42358
    Private Sub Main_Connected(sender As Object, e As ConnectedEvtArgs) Handles Me.Connected
        Stats.EVENT_Connected += 1
        L("连接成功，获取到的房间号: " & e?.roomid, True)
        If Consts.PluginEnabled Then
            If Consts.CurrentSettings.ConnectSuccessful = "" Then
                TTSPlay.DLPlayTTS("已成功连接至房间: " & e.roomid)
            Else
                TTSPlay.DLPlayTTS(Consts.CurrentSettings.ConnectSuccessful.Replace("%s", e.roomid))
            End If
        End If
    End Sub
#Enable Warning BC42358
    Private Async Sub Main_ReceivedDanmaku(sender As Object, e As ReceivedDanmakuArgs) Handles Me.ReceivedDanmaku
        Stats.EVENT_DanmakuReceived += 1
        'Start checking eligibility.
        'UID / Username
        Select Case Consts.CurrentSettings.BlockType
            Case 0
                L("UID 屏蔽模式", True)
                If Not TTSPlay.CheckEligibility(e.Danmaku.UserID, e.Danmaku.GiftName, e.Danmaku.MsgType) Then
                    L("用户 " & e.Danmaku.UserName & " (" & e.Danmaku.UserID & ")不符合条件，退出。", True)
                    Exit Sub
                End If
            Case 1
                If Not TTSPlay.CheckEligibility(e.Danmaku.UserName, e.Danmaku.GiftName, e.Danmaku.MsgType) Then
                    L("用户 " & e.Danmaku.UserName & " (" & e.Danmaku.UserID & ")不符合条件，退出。", True)
                    Exit Sub
                End If
        End Select
        'Get string.
        Dim PlayStr As String = ""
        L("PlayType: " & e.Danmaku.MsgType.ToString(), True)
        Select Case e.Danmaku.MsgType
            Case MsgTypeEnum.Comment
                If Consts.CurrentSettings.TTSDanmakuSender Then
                    PlayStr = Consts.CurrentSettings.DanmakuText.Replace("$USER", e.Danmaku.UserName).Replace("$DM", e.Danmaku.CommentText)
                Else
                    PlayStr = Consts.CurrentSettings.DanmakuText.Replace("$USER", "").Replace("$DM", e.Danmaku.CommentText)
                End If
            Case MsgTypeEnum.GiftSend 'Gift options has already been proceed in CheckEligibility() function.
                If Consts.CurrentSettings.TTSDanmakuSender Then
                    PlayStr = Consts.CurrentSettings.GiftsText.Replace("$USER", e.Danmaku.UserName).Replace("$COUNT", e.Danmaku.GiftCount).Replace("$GIFT", e.Danmaku.GiftName)
                Else
                    PlayStr = Consts.CurrentSettings.GiftsText.Replace("$USER", "").Replace("$COUNT", e.Danmaku.GiftCount).Replace("$GIFT", e.Danmaku.GiftName)
                End If
            Case MsgTypeEnum.GiftTop
            Case MsgTypeEnum.GuardBuy
            Case MsgTypeEnum.LiveEnd
                PlayStr = "直播结束，当前房间号: " & e.Danmaku.roomID
            Case MsgTypeEnum.LiveStart
                PlayStr = "直播开始，当前房间号: " & e.Danmaku.roomID
            Case MsgTypeEnum.Welcome
            Case MsgTypeEnum.WelcomeGuard
        End Select
        L("播放内容: " & PlayStr, True)

        Await TTSPlay.DLPlayTTS(PlayStr) 'Start playing!
    End Sub

    Private Sub Main_ReceivedRoomCount(sender As Object, e As ReceivedRoomCountArgs) Handles Me.ReceivedRoomCount
        UserCountLatest = e.UserCount
        Stats.EVENT_RoomCountReceived += 1
    End Sub
End Class
