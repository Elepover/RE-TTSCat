namespace Re_TTSCat.Data
{
    public partial class Conf
    {
        /// <summary>
        /// 是否启用自动更新
        /// </summary>
        public bool AutoUpdate { get; set; }
        /// <summary>
        /// 黑名单
        /// </summary>
        public string BlackList { get; set; }
        /// <summary>
        /// 白名单
        /// </summary>
        public string WhiteList { get; set; }
        /// <summary>
        /// 屏蔽模式
        /// (0 = 已关闭, 1 = 黑名单, 2 = 白名单)
        /// </summary>
        public byte BlockMode { get; set; }
        /// <summary>
        /// 礼物屏蔽模式
        /// (0 = 已关闭, 1 = 黑名单, 2 = 白名单)
        /// </summary>
        public byte GiftBlockMode { get; set; }
        /// <summary>
        /// 关键字屏蔽模式
        /// (0 = 已关闭, 1 = 黑名单, 2 = 白名单)
        /// </summary>
        public byte KeywordBlockMode { get; set; }
        /// <summary>
        /// true = 黑名单中为 UID
        /// false = 黑名单中为用户名
        /// </summary>
        public bool BlockUID { get; set; }
        /// <summary>
        /// 是否启用插件调试模式
        /// </summary>
        public bool DebugMode { get; set; }
        /// <summary>
        /// 下载失败的重试次数
        /// </summary>
        public byte DownloadFailRetryCount { get; set; }
        /// <summary>
        /// 是否在播放完成后立即删除缓存文件
        /// </summary>
        public bool DoNotKeepCache { get; set; }
        /// <summary>
        /// 要使用的弹幕引擎
        /// (0 = 毒瘤, 1 = .NET, 2 = Does not exist, 3 = 毒瘤 广东话, 4 = 有道, 5 = 用户自定义)
        /// </summary>
        public byte Engine { get; set; }
        /// <summary>
        /// 礼物黑名单
        /// </summary>
        public string GiftBlackList { get; set; }
        /// <summary>
        /// 礼物白名单
        /// </summary>
        public string GiftWhiteList { get; set; }
        /// <summary>
        /// 关键字黑名单
        /// </summary>
        public string KeywordBlackList { get; set; }
        /// <summary>
        /// 关键字白名单
        /// </summary>
        public string KeywordWhiteList { get; set; }
        /// <summary>
        /// 最小弹幕字数
        /// </summary>
        public int MinimumDanmakuLength { get; set; }
        /// <summary>
        /// 最大弹幕字数
        /// </summary>
        public int MaximumDanmakuLength { get; set; }
        /// <summary>
        /// 是否逐个朗读
        /// </summary>
        public bool ReadInQueue { get; set; }
        /// <summary>
        /// 弹幕朗读概率 (0~100)
        /// </summary>
        public int ReadPossibility { get; set; }
        /// <summary>
        /// 读出速度（仅适用于 .NET 框架引擎，范围 -10 ~ 10）
        /// </summary>
        public int ReadSpeed { get; set; }
        /// <summary>
        /// TTS 音量 (0~100, 调用时转换为 /100 后的 float)
        /// </summary>
        public int TTSVolume { get; set; }
        /// <summary>
        /// 是否适用 Google 国际服务器
        /// </summary>
        public bool UseGoogleGlobal { get; set; }
        /// <summary>
        /// 用户自定义 TTS 引擎地址，用 $TTSTEXT 表示读出内容
        /// </summary>
        public string CustomEngineURL { get; set; }
        /// <summary>
        /// 允许下载推送消息
        /// </summary>
        public bool AllowDownloadMessage { get; set; }
        /// <summary>
        /// 允许处理连接/断开事件
        /// </summary>
        public bool AllowConnectEvents { get; set; }
        /// <summary>
        /// 在断开连接时清空待朗读 TTS
        /// </summary>
        public bool ClearQueueAfterDisconnect { get; set; }
        /// <summary>
        /// 是否启用 HTTP 身份验证
        /// </summary>
        public bool HttpAuth { get; set; }
        /// <summary>
        /// HTTP 身份验证用户名
        /// </summary>
        public string HttpAuthUsername { get; set; }
        /// <summary>
        /// HTTP 身份验证密码
        /// </summary>
        public string HttpAuthPassword { get; set; }

        /// <summary>
        /// 在连接成功后读出的内容，留空以禁用
        /// </summary>
        public string OnConnected { get; set; }
        /// <summary>
        /// 在断线时读出的内容，留空以禁用
        /// </summary>
        public string OnDisconnected { get; set; }
        /// <summary>
        /// 自定义弹幕读出内容，留空以禁用
        /// </summary>
        public string OnDanmaku { get; set; }
        /// <summary>
        /// 自定义礼物读出内容，留空以禁用
        /// </summary>
        public string OnGift { get; set; }
        /// <summary>
        /// 购买船票文本，留空以禁用
        /// </summary>
        public string OnGuardBuy { get; set; }
        /// <summary>
        /// 直播开始文本，留空以禁用
        /// </summary>
        public string OnLiveStart { get; set; }
        /// <summary>
        /// 直播结束文本，留空以禁用
        /// </summary>
        public string OnLiveEnd { get; set; }
        /// <summary>
        /// 欢迎老爷文本，留空以禁用
        /// </summary>
        public string OnWelcome { get; set; }
        /// <summary>
        /// 欢迎船员文本，留空以禁用
        /// </summary>
        public string OnWelcomeGuard { get; set; }
    }
}
