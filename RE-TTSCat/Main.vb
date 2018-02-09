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
            Thread.Sleep(450)
        End While
    End Sub
#End Region

    Public Overrides Sub Start()
        Log("启动插件中...")
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
        Delay(500)
        WindowObj.TextBox_Status.Text = "正在准备设置..."

        'Detect NAudio
        If Not File.Exists(Consts.AudioLibFile) Then
            MessageBox.Show("警告: NAudio 播放库文件丢失，请从弹幕姬插件仓库中插件页面下载 NAudio 播放库，并与插件置于同一文件夹后再试一次。" & vbCrLf & vbCrLf & "已取消插件启动。" & vbCrLf & vbCrLf & "您关闭弹幕姬时将可能会遇到一个错误，当您补回 NAudio 播放库后，问题即可解决。" & vbCrLf & vbCrLf & "您在补回 NAudio 播放库后，可直接启动插件。", "Re: TTSCat", vbOKOnly, MessageBoxImage.Warning)
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
        MainBridgeSynchronizer.Abort()
        While MainBridgeSynchronizer.IsAlive
            Delay(500)
        End While
        MyBase.Stop()
        Consts.PluginEnabled = False
    End Sub

    Public Overrides Sub Admin()
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
        Consts.TrayWindow = New Form_TrayKeeper
        Consts.TrayWindow.Show() 'Tray requires configurations and MainBridge. So we need to start it later than conf. system.
    End Sub

    Public Overrides Sub DeInit()
        On Error Resume Next
        MyBase.DeInit()
        If Consts.StartedOnce Then Consts.TrayWindow.NotifyIcon_Default.Visible = False
        Process.GetCurrentProcess.Kill()
    End Sub

    Private Sub Main_Connected(sender As Object, e As ConnectedEvtArgs) Handles Me.Connected
        Stats.EVENT_Connected += 1
    End Sub

    Private Sub Main_ReceivedDanmaku(sender As Object, e As ReceivedDanmakuArgs) Handles Me.ReceivedDanmaku
        Stats.EVENT_DanmakuReceived += 1
    End Sub

    Private Sub Main_ReceivedRoomCount(sender As Object, e As ReceivedRoomCountArgs) Handles Me.ReceivedRoomCount
        Stats.EVENT_RoomCountReceived += 1
    End Sub
End Class
