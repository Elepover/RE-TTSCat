Imports System.Windows

Public Class Window_StatusReport
    Public Sub New()

        ' 此调用是设计器所必需的。
        InitializeComponent()

        ' 在 InitializeComponent() 调用之后添加任何初始化。

    End Sub

    Private Sub Window_StatusReport_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        Icon = ConvertToImageSource(My.Resources.avatar, 64, 64)
        LoadToControl()
        TextBox_VarsHelp.Text = "状态报告 - 变量帮助。

以下变量将在读出时被自动替换为对应内容。
详细范例请在弹幕姬插件仓库 -> Re: TTSCat 中查看。

$ONLINE 直播间在线人数。

$HOUR 系统时间，小时。
$MINUTE 系统时间，分钟。
$SEC 系统时间，秒。
$YEAR 系统时间，年。
$MONTH 系统时间，月。
$DAY 系统时间，日。

$MEMAVAI 可用系统内存（GB）。
$MPERCENT 系统内存占用百分比。
$VMEM 可用虚拟内存（GB）。
$VPERCENT_VM 虚拟内存占用百分比。"

    End Sub

    Private Sub Button_Save_Click(sender As Object, e As RoutedEventArgs) Handles Button_Save.Click
        '保存
        Settings.Init()
        Consts.CurrentSettings.StatusReport_ResolveAdvVars = CheckBox_EnableAdvVars.IsChecked
        Consts.CurrentSettings.StatusReport = CheckBox_EnableStatusReport.IsChecked
        Try
            If Not ((CInt(NumericUpDown_Interval.Text) > 3600) Or (CInt(NumericUpDown_Interval.Text) < 45)) Then
                Consts.CurrentSettings.StatusReportInterval = NumericUpDown_Interval.Text
            End If
        Catch ex As Exception
        End Try
        Consts.CurrentSettings.StatusReportContent = TextBox_ReportText.Text

        Try
            Settings.SaveConf(Consts.CurrentSettings)
        Catch ex As Exception
            Stats.FAILURE_ErrorCounter += 1
            Stats.FAILURE_LatestError = ex
        End Try
        LoadToControl()
        Me.Close()
    End Sub

    Private Sub LoadToControl()
        CheckBox_EnableAdvVars.IsChecked = Consts.CurrentSettings.StatusReport_ResolveAdvVars
        CheckBox_EnableStatusReport.IsChecked = Consts.CurrentSettings.StatusReport
        NumericUpDown_Interval.Text = Consts.CurrentSettings.StatusReportInterval
        TextBox_ReportText.Text = Consts.CurrentSettings.StatusReportContent
    End Sub

    Private Sub Button_Cancel_Click(sender As Object, e As RoutedEventArgs) Handles Button_Cancel.Click
        Me.Close()
    End Sub
End Class
