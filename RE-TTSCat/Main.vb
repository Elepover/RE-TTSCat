Imports System.ComponentModel
Imports System.IO
Imports System.Threading
Imports BilibiliDM_PluginFramework

Public Class Main
    Inherits DMPlugin
    Public Sub New()
        PluginAuth = "Elepover"
        PluginName = "RE-TTSCat"
        PluginCont = "elepover@outlook.com"
        PluginVer = "2.0.0.1"
        PluginDesc = "TTSDanmaku 重写版（读弹幕姬）"
    End Sub
#Region "MainBridge"
    Private MainBridgeSynchronizer As Thread
    Private Sub MainBridgeSynchronize()
        While True
            ' Priority: ReqStart > ReqStop > ReqAdmin > ReqExit > Logs
            If MainBridge.ReqStart Then
                MainBridge.ReqStart = False
                Start()
            End If
            If MainBridge.ReqStop Then
                MainBridge.ReqStop = False
                [Stop]()
            End If
            If MainBridge.ReqAdmin Then
                MainBridge.ReqAdmin = False
                Admin()
            End If
            If MainBridge.ReqExit Then
                Consts.DMJWindow.Close()
            End If

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

            Threading.Thread.Sleep(450)
        End While
    End Sub
#End Region

    Public Overrides Sub Start()
        MyBase.Start()
    End Sub

    Public Overrides Sub [Stop]()
        MyBase.Stop()
    End Sub

    Public Overrides Sub Admin()
        MyBase.Admin()
    End Sub

    Public Overrides Sub Inited()
        MyBase.Inited()
    End Sub

    Public Overrides Sub DeInit()
        MyBase.DeInit()
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
