' Probe Files Map
' Master Dir.
' - RE-TTSCat.dll
' - NAudio.dll
' - RE-TTSCat
' | - Cache
' | | - TTS*.mp3
' | - TTSCat.ini
'

Imports System.IO
Imports System.Threading
Imports System.Xml
Imports System.Xml.Serialization
Imports RE_TTSCat.Consts

Public Class Settings
    Public Class Conf
        ' 请自觉无视所有更新日期
        ' 嗯，我懒，懒得改了（

        ''' <summary>
        ''' 调试模式是否启用
        ''' </summary>
        ''' <returns></returns>
        Public Property DebugMode As Boolean
        ''' <summary>
        ''' 是否读出弹幕发送者
        ''' </summary>
        ''' <returns></returns>
        Public Property TTSDanmakuSender As Boolean
        ''' <summary>
        ''' 是否读出收到的礼物
        ''' </summary>
        ''' <returns></returns>
        Public Property TTSGiftsReceived As Boolean
        ''' <summary>
        ''' 自动清理缓存是否启用
        ''' </summary>
        ''' <returns></returns>
        Public Property AutoClearCache As Boolean
        ''' <summary>
        ''' 新增于 2017/04/25 17:34 - 是否启用 TTS 冷却
        ''' </summary>
        ''' <returns></returns>
        Public Property TTSDelayEnabled As Boolean
        ''' <summary>
        ''' 新增于 2017/04/25 17:34 - TTS 冷却值
        ''' </summary>
        ''' <returns></returns>
        Public Property TTSDelayValue As Long
        ''' <summary>
        ''' 新增于 2017/05/21 10:18 - 自定义礼物读出文本。
        ''' </summary>
        ''' <returns></returns>
        Public Property GiftsText As String
        ''' <summary>
        ''' 新增于 2017/05/21 10:18 - 自定义弹幕读出文本。
        ''' </summary>
        ''' <returns></returns>
        Public Property DanmakuText As String
        ''' <summary>
        ''' 新增于 2017/05/28 22:09 - 弹幕引擎 (0 = 毒瘤, 1 = .NET, 2 = Does not exist)
        ''' </summary>
        ''' <returns></returns>
        Public Property Engine As Integer
#Region "Status Reporting"
        ''' <summary>
        ''' 新增于 2017/05/29 23:30 - 是否启用状态报告。
        ''' </summary>
        ''' <returns></returns>
        Public Property StatusReport As Boolean
        ''' <summary>
        ''' 新增于 2017/05/29 23:30 - 状态报告时间间隔。
        ''' </summary>
        ''' <returns></returns>
        Public Property StatusReportInterval As Integer
        ''' <summary>
        ''' 新增于 2017/05/30 00:38 - 状态报告内容
        ''' </summary>
        ''' <returns></returns>
        Public Property StatusReportContent As String
        ''' <summary>
        ''' 新增于 2017/05/29 23:30 - 是否启用状态报告高级变量。
        ''' </summary>
        ''' <returns></returns>
        Public Property StatusReport_ResolveAdvVars As Boolean
#End Region
        ''' <summary>
        ''' 新增于 2017/06/24 20:50 - TTS 播放音量
        ''' </summary>
        ''' <returns></returns>
        Public Property TTSVolume As Integer
        ''' <summary>
        ''' 新增于 2017/07/08 00:08 - 缓存文件使用后立即删除
        ''' </summary>
        ''' <returns></returns>
        Public Property DoNotKeepCache As Boolean
        ''' <summary>
        ''' 新增于 2017/07/16 17:53 - 成功连接到房间后的读出内容
        ''' </summary>
        ''' <returns></returns>
        Public Property ConnectSuccessful As String
        ''' <summary>
        ''' 新增于 2017/07/16 17:53 - 下载失败后的重试次数，默认为 5
        ''' </summary>
        ''' <returns></returns>
        Public Property DLFailRetry As Short = 5
#Region "Proxy"
        ''' <summary>
        ''' 新增于 2017/07/28 17:01 - 代理服务器
        ''' </summary>
        ''' <returns></returns>
        Public Property ProxySettings_ProxyServer As String
        ''' <summary>
        ''' 新增于 2017/07/28 17:01 - 代理端口
        ''' </summary>
        ''' <returns></returns>
        Public Property ProxySettings_ProxyPort As Integer
        ''' <summary>
        ''' 新增于 2017/07/28 17:01 - 代理用户
        ''' </summary>
        ''' <returns></returns>
        Public Property ProxySettings_ProxyUser As String
        ''' <summary>
        ''' 新增于 2017/07/28 17:01 - 代理密码
        ''' </summary>
        ''' <returns></returns>
        Public Property ProxySettings_ProxyPassword As String
        ''' <summary>
        ''' 新增于 2017/07/28 17:01 - 是否使用 HTTPS
        ''' </summary>
        ''' <returns></returns>
        Public Property HTTPSPreference As Boolean
        ''' <summary>
        ''' 新增于 2017/07/28 17:01 - 是否使用 Google Global
        ''' </summary>
        ''' <returns></returns>
        Public Property UseGoogleGlobal As Boolean
#End Region
#Region ".NET Framework TTS"
        ''' <summary>
        ''' 新增于 2017/08/09 09:08 - NET 框架引擎语速
        ''' </summary>
        ''' <returns></returns>
        Public Property NETFramework_VoiceSpeed As String
#End Region
#Region "Blocking Settings"
        ''' <summary>
        ''' 新增于 2017/08/09 09:08 - 屏蔽模式 (0 = 已关闭, 1 = 黑名单, 2 = 白名单)
        ''' </summary>
        ''' <returns></returns>
        Public Property Block_Mode As String
        ''' <summary>
        ''' 新增于 2017/08/09 09:08 - 礼物屏蔽模式 (0 = 已关闭, 1 = 黑名单, 2 = 白名单)
        ''' </summary>
        ''' <returns></returns>
        Public Property GiftBlock_Mode As String
#End Region
        ''' <summary>
        ''' 常驻内存，黑名单
        ''' </summary>
        Public Blacklist As String
        ''' <summary>
        ''' 常驻内存，白名单
        ''' </summary>
        Public Whitelist As String
        ''' <summary>
        ''' 常驻内存，礼物黑名单
        ''' </summary>
        Public GiftBlacklist As String
        ''' <summary>
        ''' 常驻内存，礼物白名单
        ''' </summary>
        Public GiftWhitelist As String
        ''' <summary>
        ''' 队列读出
        ''' </summary>
        ''' <returns></returns>
        Public Property ReadInArray As Boolean
        ''' <summary>
        ''' 屏蔽类型 (0 = UID, 1 = 用户名)
        ''' </summary>
        ''' <returns></returns>
        Public Property BlockType As String
        ''' <summary>
        ''' 最少弹幕字数限制
        ''' </summary>
        ''' <returns></returns>
        Public Property MiniumDMLength As String
        ''' <summary>
        ''' 首次使用通知栏图标
        ''' </summary>
        ''' <returns></returns>
        Public Property FirstUseTrayIcon As Boolean
        ''' <summary>
        ''' 是否展示通知栏图标
        ''' </summary>
        ''' <returns></returns>
        Public Property ShowTrayIcon As Boolean
        ''' <summary>
        ''' 是否启用自动更新
        ''' </summary>
        ''' <returns></returns>
        Public Property AutoUpdEnabled As Boolean
        ''' <summary>
        ''' 是否忽略 TTS 音量。
        ''' </summary>
        ''' <returns></returns>
        Public Property IgnoreTTSVolume As Boolean

        Public Shared ReadOnly Property APIString As String
            Get
                If CurrentSettings.HTTPSPreference Then
                    Return "https://fanyi.baidu.com/gettts?lan=zh&text=$TTSTEXT&spd=5&source=web"
                Else
                    Return "http://fanyi.baidu.com/gettts?lan=zh&text=$TTSTEXT&spd=5&source=web"
                End If
            End Get
        End Property

        Sub New()
            DebugMode = False
            TTSDanmakuSender = True
            TTSGiftsReceived = True
            AutoClearCache = True
            TTSDelayEnabled = False
            TTSDelayValue = 5000
            DanmakuText = "$USER 说: $DM"
            GiftsText = "收到来自 $USER 的 $COUNT 个 $GIFT"
            Engine = 0
            StatusReport = False
            StatusReport_ResolveAdvVars = True
            StatusReportInterval = 60
            StatusReportContent = "当前在线人数: $ONLINE, 弹幕总数: $TOTALDM, 现在是 $YEAR 年 $MONTH 月 $DAY 日，$HOUR 时 $MINUTE 分 $SEC 秒，当前物理内存可用 $MEMAVAI GB，已用百分之 $MPERCENT，虚拟内存可用 $VMEM GB，已用百分之 $VPERCENT_VM。"
            TTSVolume = 100
            DoNotKeepCache = False
            ConnectSuccessful = "已成功连接至房间: %s"
            DLFailRetry = 5
            ProxySettings_ProxyServer = ""
            ProxySettings_ProxyPort = 0
            ProxySettings_ProxyUser = ""
            ProxySettings_ProxyPassword = ""
            HTTPSPreference = True
            UseGoogleGlobal = False
            NETFramework_VoiceSpeed = 0
            Block_Mode = 0
            GiftBlock_Mode = 0
            Blacklist = ""
            Whitelist = ""
            GiftBlacklist = ""
            GiftWhitelist = ""
            ReadInArray = True
            BlockType = 0
            MiniumDMLength = 1
            FirstUseTrayIcon = True
            ShowTrayIcon = True
            AutoUpdEnabled = True
            IgnoreTTSVolume = False
        End Sub
    End Class

    ''' <summary>
    ''' Initiate Settings and release TTSEngine.
    ''' </summary>
    Public Shared Sub Init()
        If Not Directory.Exists(ConfDir) Then
            Directory.CreateDirectory(ConfDir)
        End If
        CreateConf()
        If Not Directory.Exists(CacheDir) Then
            Directory.CreateDirectory(CacheDir)
        End If
    End Sub
    ''' <summary>
    ''' Initiates Settings File.
    ''' </summary>
    Public Shared Sub CreateConf()
        Try
            SaveConf(New Conf)
            ReadConf()
        Catch ex As Exception
        End Try
    End Sub
    ''' <summary>
    ''' Saves Settings File.
    ''' </summary>
    ''' <param name="obj">Object to serialize.</param>
    Public Shared Sub SaveConf(obj As Conf)
        Dim XSSubmit As XmlSerializer = New XmlSerializer(GetType(Conf))
        Dim XMLText = ""
        Using StringWriteParser = New StringWriter()
            Using XWriter As XmlWriter = XmlWriter.Create(StringWriteParser)
                XSSubmit.Serialize(XWriter, obj)
                XMLText = StringWriteParser.ToString()
            End Using
        End Using
        Dim Writer As New StreamWriter(ConfFile, False, System.Text.Encoding.UTF8)
        Writer.Write(XMLText)
        Writer.Close()
    End Sub
    ''' <summary>
    ''' Reads Settings File
    ''' </summary>
    Public Shared Sub ReadConf()
        Dim Serializer As XmlSerializer = New XmlSerializer(GetType(Conf))
        Using FStream As FileStream = New FileStream(ConfFile, FileMode.Open)
            CurrentSettings = CType(Serializer.Deserialize(FStream), Conf)
        End Using
    End Sub
End Class

Public Class Consts
    Public Shared ReadOnly Property DLLFile As String = Reflection.Assembly.GetExecutingAssembly.Location
    Public Shared ReadOnly Property DLLPath As String = (New FileInfo(DLLFile)).DirectoryName
    Public Shared ReadOnly Property ConfDir As String = Path.Combine(DLLPath, "RE-TTSCat")
    Public Shared ReadOnly Property CacheDir As String = Path.Combine(ConfDir, "Cache")
    Public Shared ReadOnly Property ConfFile As String = Path.Combine(ConfDir, "TTSCat.ini")
    Public Shared ReadOnly Property AudioLibFile As String = Path.Combine(DLLPath, "NAudio.dll")
    Public Shared Property CurrentSettings As Settings.Conf
    Public Shared Property DMJWindow As Window
End Class

Public Class Stats
    Public Shared Property EVENT_DanmakuReceived As Integer = 0
    Public Shared Property EVENT_Connected As Integer = 0
    Public Shared Property EVENT_RoomCountReceived As Integer = 0
    Public Shared Property SUCCEED_OnTTSDownloaded As Integer = 0
    Public Shared Property SUCCEED_OnTTSPlayed As Integer = 0
    Public Shared Property SUCCEED_OnMainBridge As Integer = 0
    Public Shared Property FAILURE_LatestError As Exception = Nothing
    Public Shared Property FAILURE_ErrorCounter As Integer = 0
    Public Shared Property FAILURE_DownloadFailed As Integer = 0
    Public Shared Property FAILURE_PlayFailed As Integer = 0
End Class

Public Class MainBridge
    Public Shared PendingLogs As List(Of Log) = New List(Of Log)
    Public Shared ReqStop As Boolean = False
    Public Shared ReqStart As Boolean = False
    Public Shared ReqAdmin As Boolean = False
    Public Shared ReqExit As Boolean = False
End Class

Public Class Log
    Sub New()
        LogContent = ""
        DebugOnly = False
    End Sub
    Public Property LogContent As String
    Public Property DebugOnly As Boolean

End Class

Public Module ModularSubs
    Public Sub L(Content As String, Optional Debug As Boolean = False)
        Dim Obj As New Log() With {.LogContent = Content, .DebugOnly = Debug}
        MainBridge.PendingLogs.Add(Obj)
    End Sub
    Public Sub Delay(interval As Single)
        Dim TimerCount As Single
        Dim Timer As New Stopwatch
        Timer.Start()
        interval = interval / 1000
        TimerCount = Timer.Elapsed.TotalSeconds + interval
        While TimerCount - Timer.Elapsed.TotalSeconds > 0
            Thread.Sleep(1)
        End While
    End Sub
End Module