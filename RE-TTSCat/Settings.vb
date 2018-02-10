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
Imports System.Net
Imports System.Threading
Imports System.Windows.Threading
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
            TTSDelayEnabled = False
            TTSDelayValue = 5000
            DanmakuText = "$USER 说: $DM"
            GiftsText = "收到来自 $USER 的 $COUNT 个 $GIFT"
            Engine = 0
            StatusReport = False
            StatusReport_ResolveAdvVars = True
            StatusReportInterval = 60
            StatusReportContent = "当前人气值: $ONLINE, 现在是 $YEAR 年 $MONTH 月 $DAY 日，$HOUR 时 $MINUTE 分 $SEC 秒，当前物理内存可用 $MEMAVAI GB，已用百分之 $MPERCENT，虚拟内存可用 $VMEM GB，已用百分之 $VPERCENT_VM。"
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
            CreateConf()
        End If
        If Not Directory.Exists(CacheDir) Then
            Directory.CreateDirectory(CacheDir)
        End If
        Try
            ReadConf()
        Catch ex As Exception
            CreateConf()
            ReadConf()
        End Try
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
        Dim fileStream As IO.FileStream = New IO.FileStream(ConfFile, IO.FileMode.Create)
        Dim serializer As New Runtime.Serialization.DataContractSerializer(GetType(Conf))
        serializer.WriteObject(fileStream, obj)
        fileStream.Close()
    End Sub
    ''' <summary>
    ''' Reads Settings File
    ''' </summary>
    Public Shared Sub ReadConf()
        Dim fileStream As FileStream = New FileStream(ConfFile, FileMode.Open)
        Dim gottenResult As Conf = New Runtime.Serialization.DataContractSerializer(GetType(Conf)).ReadObject(fileStream)
        fileStream.Close()
        CurrentSettings = gottenResult
    End Sub
End Class

Public Class Consts
    Public Shared ReadOnly Property DLLFile As String = Reflection.Assembly.GetExecutingAssembly.Location
    Public Shared ReadOnly Property DLLPath As String = (New FileInfo(DLLFile)).DirectoryName
    Public Shared ReadOnly Property ConfDir As String = Path.Combine(DLLPath, "RE-TTSCat")
    Public Shared ReadOnly Property CacheDir As String = Path.Combine(ConfDir, "Cache")
    Public Shared ReadOnly Property ConfFile As String = Path.Combine(ConfDir, "TTSCat.xml")
    Public Shared ReadOnly Property AudioLibFile As String = Path.Combine(DLLPath, "NAudio.dll")
    Public Shared Property StartedOnce As Boolean = False
    Public Shared Property CurrentSettings As Settings.Conf
    Public Shared WithEvents DMJWindow As Window
    Public Shared Property PluginEnabled As Boolean = False

    Private Shared Sub DMJWindow_Loaded(sender As Object, e As RoutedEventArgs) Handles DMJWindow.Loaded
        DMJWindow.Activate()
    End Sub
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
    Public Shared Property Started As Boolean = False
    Public Shared Property StartError As Boolean = False
    Public Shared Property PluginInstance As Main
End Class

Public Class Log
    Sub New()
        LogContent = ""
        DebugOnly = False
    End Sub
    Public Property LogContent As String
    Public Property DebugOnly As Boolean
End Class

Public Class KruinUpdates

    Public Class Update
        ''' <summary>
        ''' 产生一个新的 Update 对象。
        ''' </summary>
        ''' <param name="latestVer">最新版本</param>
        ''' <param name="updTime">更新日期</param>
        ''' <param name="updDesc">更新描述</param>
        Sub New(latestVer As Version, updTime As Date, updDesc As String, dlLink As String)
            pLatestVersion = latestVer
            pUpdateTime = updTime
            pUpdateDescription = updDesc
            pDLLink = dlLink
        End Sub

        ''' <summary>
        ''' 获得的最新版本
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property LatestVersion As Version
            Get
                Return pLatestVersion
            End Get
        End Property
        ''' <summary>
        ''' 最新版本更新日期
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property UpdateTime As Date
            Get
                Return pUpdateTime
            End Get
        End Property
        ''' <summary>
        ''' 更新描述
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property UpdateDescription As String
            Get
                Return pUpdateDescription
            End Get
        End Property

        ''' <summary>
        ''' 更新下载链接
        ''' </summary>
        ''' <returns></returns>
        Public ReadOnly Property DLURL As String
            Get
                Return pDLLink
            End Get
        End Property

        Private Property pLatestVersion As Version
        Private Property pUpdateTime As Date
        Private Property pUpdateDescription As String
        Private Property pDLLink As String
    End Class

    ''' <summary>
    ''' 获取最新的插件版本
    ''' </summary>
    ''' <returns></returns>
    Public Shared Function GetLatestUpd() As Update
        Dim gottenResult As String
        gottenResult = HttpGet(New Uri("https://www.danmuji.org/api/v2/TTSDanmaku"))
        Dim jsonObj As Newtonsoft.Json.Linq.JObject = Newtonsoft.Json.Linq.JObject.Parse(gottenResult)
        Dim latestVer As Version = New Version(jsonObj("version").ToString())
        Dim updDesc As String = jsonObj("update_desc").ToString()
        Dim updTime As Date = DateTimeOffset.Parse(jsonObj("update_datetime"), Nothing).DateTime
        Dim dlLink As String = jsonObj("dl_url").ToString()

        Return New Update(latestVer, updTime, updDesc, dlLink)
    End Function

    ''' <summary>
    ''' 等同于 DownloadString 吧，mmp
    ''' </summary>
    ''' <param name="uri">请求 URI</param>
    ''' <returns></returns>
    Public Shared Function HttpGet(uri As Uri) As String
        Dim request As HttpWebRequest = DirectCast(WebRequest.Create(uri), HttpWebRequest)
        request.UserAgent = "KruinUpdates/" & New Main().PluginVer & " (TTSDanmaku;)"
        Using response As HttpWebResponse = DirectCast(request.GetResponse(), HttpWebResponse)
            Using stream As IO.Stream = response.GetResponseStream()
                Using reader As New IO.StreamReader(stream)
                    Dim text = reader.ReadToEnd()
                    Return text
                End Using
            End Using
        End Using
    End Function

    ''' <summary>
    ''' 插件是不是最新版?
    ''' </summary>
    ''' <param name="currentVer">当前版本</param>
    ''' <returns></returns>
    Public Shared Function CheckIfLatest(currentVer As Version) As Boolean
        Dim upd As Update = GetLatestUpd()
        Return CheckIfLatest(upd, currentVer)
    End Function

    ''' <summary>
    ''' 插件是不是最新版?
    ''' </summary>
    ''' <param name="upd">检查到的最新版本</param>
    ''' <param name="currentVer">当前版本</param>
    ''' <returns></returns>
    Public Shared Function CheckIfLatest(upd As Update, currentVer As Version) As Boolean
        If upd.LatestVersion > currentVer Then
            Return False
        Else
            Return True
        End If
    End Function
End Class

Public Module ModularSubs
    Public Sub L(Content As String, Optional Debug As Boolean = False)
        Dim Obj As New Log() With {.LogContent = Content, .DebugOnly = Debug}
        MainBridge.PendingLogs.Add(Obj)
    End Sub
    ''' <summary>
    ''' Wait a minute... emmm...
    ''' via Stack Overflow.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Delay(ByVal Time As Double)
        Dim Frame = New DispatcherFrame()
        Dim Thr As New Thread(CType((Sub()
                                         Thread.Sleep(Time)
                                         Frame.[Continue] = False
                                     End Sub), ThreadStart))
        Thr.Start()
        Dispatcher.PushFrame(Frame)
    End Sub
    ''' <summary>
    ''' Converts an object into System.Windows.Media.ImageSource object.
    ''' </summary>
    ''' <param name="source">Source object to convert.</param>
    ''' <returns>Converted ImageSource object.</returns>
    ''' <remarks></remarks>
    Public Function ConvertToImageSource(source As Object, Width As Integer, Height As Integer) As ImageSource
        Dim result As ImageSource = Interop.Imaging.CreateBitmapSourceFromHBitmap(
             source.GetHbitmap(),
             IntPtr.Zero,
             Int32Rect.Empty,
             BitmapSizeOptions.FromWidthAndHeight(Width, Height))
        Return result
    End Function
End Module