using System;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Re_TTSCat.Data
{
    public class Vars
    {
        public static readonly string apiBaidu = "https://fanyi.baidu.com/gettts?lan=zh&text=$TTSTEXT&spd=5&source=web";
        public static readonly string apiBaiduCantonese = "https://fanyi.baidu.com/gettts?lan=cte&text=$TTSTEXT&spd=5&source=web";
        public static readonly string dllFileName = Assembly.GetExecutingAssembly().Location;
        public static readonly string dllPath = (new FileInfo(dllFileName)).DirectoryName;
        public static readonly string confDir = Path.Combine(dllPath, "RE-TTSCat");
        public static readonly string cacheDir = Path.Combine(confDir, "Cache");
        public static readonly string confFileName = Path.Combine(confDir, "Config.json");
        public static readonly string audioLibFileName = Path.Combine(dllPath, "NAudio.dll");
        public static readonly Version currentVersion = new Version("3.0.5.223");

        public static Conf CurrentConf { get; set; } = new Conf();
        public static Thread MainBridge { get; set; }
        public static Thread Player { get; set; }
        public static uint RoomCount { get; set; }
        public static bool CallBridgeStop { get; set; } = false;
        public static bool CallPlayerStop { get; set; } = false;
    }
}
