Public Class Form_TrayKeeper
    Private Sub Form_TrayKeeper_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Visible = False
        NotifyIcon_Default.Visible = Consts.CurrentSettings.ShowTrayIcon
        If NotifyIcon_Default.Visible And Consts.CurrentSettings.FirstUseTrayIcon Then
            Consts.CurrentSettings.FirstUseTrayIcon = False
            Try
                Settings.SaveConf(Consts.CurrentSettings)
            Catch ex As Exception
                L("设置保存失败：" & ex.Message)
            End Try
            NotifyIcon_Default.ShowBalloonTip(1000, "Re: TTSCat", "现在可以直接在任务栏通知区域管理插件啦！", Windows.Forms.ToolTipIcon.Info)
        End If
    End Sub

    Private Sub 启动插件ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 启动插件ToolStripMenuItem.Click
        MainBridge.PluginInstance.Start()
    End Sub

    Private Sub 停用插件ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 停用插件ToolStripMenuItem.Click
        MainBridge.PluginInstance.Stop()
    End Sub

    Private Sub 管理插件ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 管理插件ToolStripMenuItem.Click
        MainBridge.PluginInstance.Admin()
    End Sub

    Private Sub 强制关闭弹幕姬ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 强制关闭弹幕姬ToolStripMenuItem.Click
        NotifyIcon_Default.Visible = False
        Process.GetCurrentProcess.Kill()
    End Sub

    Private Sub 优雅关闭弹幕姬ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 优雅关闭弹幕姬ToolStripMenuItem.Click
        Consts.DMJWindow.Close()
    End Sub

    Private Sub NotifyIcon_Default_MouseDoubleClick(sender As Object, e As Forms.MouseEventArgs) Handles NotifyIcon_Default.MouseDoubleClick
        显示隐藏弹幕姬窗口ToolStripMenuItem.PerformClick()
    End Sub

    Private Sub 显示隐藏弹幕姬窗口ToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 显示隐藏弹幕姬窗口ToolStripMenuItem.Click
        If Consts.DMJWindow.IsVisible Then
            Consts.DMJWindow.Visibility = Visibility.Hidden
        Else
            Consts.DMJWindow.Visibility = Visibility.Visible
        End If
    End Sub
End Class