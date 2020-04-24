using BilibiliDM_PluginFramework;

namespace Re_TTSCat.Data
{
    public partial class Conf
    {
        public static bool CheckDanmakuLength(string content)
        {
            var len = new System.Globalization.StringInfo(content).LengthInTextElements;
            if (!((len <= Vars.CurrentConf.MaximumDanmakuLength) && (len >= Vars.CurrentConf.MinimumDanmakuLength)))
            {
                Bridge.ALog($"忽略: 弹幕字数 ({len}) 不符合要求");
                return false;
            }
            return true;
        }
        public static bool CheckDanmakuLength(ReceivedDanmakuArgs e)
        {
            return CheckDanmakuLength(e.Danmaku.CommentText);
        }
    }
}