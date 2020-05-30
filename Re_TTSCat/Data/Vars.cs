using Re_TTSCat.Windows;
using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Re_TTSCat.Data
{
    public sealed class Vars
    {
        public static readonly string ApiBaidu = "https://fanyi.baidu.com/gettts?lan=zh&text=$TTSTEXT&spd=5&source=web";
        public static readonly string ApiBaiduCantonese = "https://fanyi.baidu.com/gettts?lan=cte&text=$TTSTEXT&spd=5&source=web";
        public static readonly string ApiYoudao = "http://tts.youdao.com/fanyivoice?word=$TTSTEXT&le=zh&keyfrom=speaker-target";
        public static readonly string ApiBaiduAi = "https://tsn.baidu.com/text2audio?tex=$TTSTEXT&lan=zh&per=$PERSON&cuid=1234567JAVA&ctp=1&tok=$TOKEN";
        public static readonly string ApiBaiduAiAppKey = "4E1BG9lTnlSeIf1NQFlrSq6h"; // thank, ref: https://github.com/Baidu-AIP/speech-demo/blob/master/rest-api-tts/java/src/com/baidu/speech/restapi/ttsdemo/TtsMain.java
        public static readonly string ApiBaiduAiSecretKey = "544ca4657ba8002e3dea3ac2f5fdd241";
        public static readonly string AppDllFileName = Assembly.GetExecutingAssembly().Location;
        public static readonly string AppDllFilePath = (new FileInfo(AppDllFileName)).DirectoryName;
        public static readonly string ConfDir = Path.Combine(AppDllFilePath, "RE-TTSCat");
        public static readonly string DefaultCacheDir = Path.Combine(ConfDir, "Cache");
        public static readonly string CacheDirTemp = Path.Combine(Path.GetTempPath(), "Re-TTSCat TTS Cache");
        public static readonly string ConfFileName = Path.Combine(ConfDir, "Config.json");
        public static readonly string AudioLibraryFileName = Path.Combine(ConfDir, "NAudio.dll");
        public static readonly Version CurrentVersion = new Version("3.7.65.530");
        public static readonly string ManagementWindowDefaultTitle = "Re: TTSCat - 插件管理";

        public static Conf CurrentConf = new Conf();
        public static Thread Player;
        public static uint RoomCount;
        public static uint TotalPlayed = 0;
        public static bool CallPlayerStop = false;
        public static bool HangWhenCrash = false;
        public static OptionsWindow ManagementWindow;
        public static string CacheDir => CurrentConf?.SaveCacheInTempDir == false ? DefaultCacheDir : CacheDirTemp;
        public static string ApiBaiduAiAccessToken = string.Empty;
    }
}
