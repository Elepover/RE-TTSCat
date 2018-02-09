<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form_TrayKeeper
    Inherits System.Windows.Forms.Form

    'Form 重写 Dispose，以清理组件列表。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows 窗体设计器所必需的
    Private components As System.ComponentModel.IContainer

    '注意: 以下过程是 Windows 窗体设计器所必需的
    '可以使用 Windows 窗体设计器修改它。  
    '不要使用代码编辑器修改它。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form_TrayKeeper))
        Me.NotifyIcon_Default = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.ContextMenuStrip_Default = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.启动插件ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.停用插件ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.管理插件ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.优雅关闭弹幕姬ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.强制关闭弹幕姬ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.显示隐藏弹幕姬窗口ToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextMenuStrip_Default.SuspendLayout()
        Me.SuspendLayout()
        '
        'NotifyIcon_Default
        '
        Me.NotifyIcon_Default.ContextMenuStrip = Me.ContextMenuStrip_Default
        Me.NotifyIcon_Default.Icon = CType(resources.GetObject("NotifyIcon_Default.Icon"), System.Drawing.Icon)
        Me.NotifyIcon_Default.Text = "Re: TTSCat"
        '
        'ContextMenuStrip_Default
        '
        Me.ContextMenuStrip_Default.BackColor = System.Drawing.Color.White
        Me.ContextMenuStrip_Default.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.显示隐藏弹幕姬窗口ToolStripMenuItem, Me.启动插件ToolStripMenuItem, Me.停用插件ToolStripMenuItem, Me.管理插件ToolStripMenuItem, Me.优雅关闭弹幕姬ToolStripMenuItem, Me.强制关闭弹幕姬ToolStripMenuItem})
        Me.ContextMenuStrip_Default.Name = "ContextMenuStrip_Default"
        Me.ContextMenuStrip_Default.Size = New System.Drawing.Size(199, 158)
        '
        '启动插件ToolStripMenuItem
        '
        Me.启动插件ToolStripMenuItem.Image = CType(resources.GetObject("启动插件ToolStripMenuItem.Image"), System.Drawing.Image)
        Me.启动插件ToolStripMenuItem.Name = "启动插件ToolStripMenuItem"
        Me.启动插件ToolStripMenuItem.Size = New System.Drawing.Size(198, 22)
        Me.启动插件ToolStripMenuItem.Text = "启动插件"
        '
        '停用插件ToolStripMenuItem
        '
        Me.停用插件ToolStripMenuItem.Image = CType(resources.GetObject("停用插件ToolStripMenuItem.Image"), System.Drawing.Image)
        Me.停用插件ToolStripMenuItem.Name = "停用插件ToolStripMenuItem"
        Me.停用插件ToolStripMenuItem.Size = New System.Drawing.Size(160, 22)
        Me.停用插件ToolStripMenuItem.Text = "停用插件"
        '
        '管理插件ToolStripMenuItem
        '
        Me.管理插件ToolStripMenuItem.Font = New System.Drawing.Font("Microsoft YaHei UI", 9.0!)
        Me.管理插件ToolStripMenuItem.Image = CType(resources.GetObject("管理插件ToolStripMenuItem.Image"), System.Drawing.Image)
        Me.管理插件ToolStripMenuItem.Name = "管理插件ToolStripMenuItem"
        Me.管理插件ToolStripMenuItem.Size = New System.Drawing.Size(198, 22)
        Me.管理插件ToolStripMenuItem.Text = "管理插件"
        '
        '优雅关闭弹幕姬ToolStripMenuItem
        '
        Me.优雅关闭弹幕姬ToolStripMenuItem.Image = CType(resources.GetObject("优雅关闭弹幕姬ToolStripMenuItem.Image"), System.Drawing.Image)
        Me.优雅关闭弹幕姬ToolStripMenuItem.Name = "优雅关闭弹幕姬ToolStripMenuItem"
        Me.优雅关闭弹幕姬ToolStripMenuItem.Size = New System.Drawing.Size(160, 22)
        Me.优雅关闭弹幕姬ToolStripMenuItem.Text = "优雅关闭弹幕姬"
        '
        '强制关闭弹幕姬ToolStripMenuItem
        '
        Me.强制关闭弹幕姬ToolStripMenuItem.Image = CType(resources.GetObject("强制关闭弹幕姬ToolStripMenuItem.Image"), System.Drawing.Image)
        Me.强制关闭弹幕姬ToolStripMenuItem.Name = "强制关闭弹幕姬ToolStripMenuItem"
        Me.强制关闭弹幕姬ToolStripMenuItem.Size = New System.Drawing.Size(160, 22)
        Me.强制关闭弹幕姬ToolStripMenuItem.Text = "强制关闭弹幕姬"
        '
        '显示隐藏弹幕姬窗口ToolStripMenuItem
        '
        Me.显示隐藏弹幕姬窗口ToolStripMenuItem.Font = New System.Drawing.Font("Microsoft YaHei UI", 9.0!, System.Drawing.FontStyle.Bold)
        Me.显示隐藏弹幕姬窗口ToolStripMenuItem.Image = CType(resources.GetObject("显示隐藏弹幕姬窗口ToolStripMenuItem.Image"), System.Drawing.Image)
        Me.显示隐藏弹幕姬窗口ToolStripMenuItem.Name = "显示隐藏弹幕姬窗口ToolStripMenuItem"
        Me.显示隐藏弹幕姬窗口ToolStripMenuItem.Size = New System.Drawing.Size(198, 22)
        Me.显示隐藏弹幕姬窗口ToolStripMenuItem.Text = "显示 / 隐藏弹幕姬窗口"
        '
        'Form_TrayKeeper
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.White
        Me.ClientSize = New System.Drawing.Size(548, 294)
        Me.Font = New System.Drawing.Font("微软雅黑", 8.25!)
        Me.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.MinimumSize = New System.Drawing.Size(156, 39)
        Me.Name = "Form_TrayKeeper"
        Me.ShowIcon = False
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "TTSCat Tray"
        Me.WindowState = System.Windows.Forms.FormWindowState.Minimized
        Me.ContextMenuStrip_Default.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents ContextMenuStrip_Default As Forms.ContextMenuStrip
    Friend WithEvents 启动插件ToolStripMenuItem As Forms.ToolStripMenuItem
    Friend WithEvents 停用插件ToolStripMenuItem As Forms.ToolStripMenuItem
    Friend WithEvents 管理插件ToolStripMenuItem As Forms.ToolStripMenuItem
    Friend WithEvents 强制关闭弹幕姬ToolStripMenuItem As Forms.ToolStripMenuItem
    Public WithEvents NotifyIcon_Default As Forms.NotifyIcon
    Friend WithEvents 优雅关闭弹幕姬ToolStripMenuItem As Forms.ToolStripMenuItem
    Friend WithEvents 显示隐藏弹幕姬窗口ToolStripMenuItem As Forms.ToolStripMenuItem
End Class
