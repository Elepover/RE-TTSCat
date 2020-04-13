using BilibiliDM_PluginFramework;

namespace Re_TTSCat.Data
{
    public partial class Conf
    {
        public static bool CheckDanmakuLength(string content)
        {
            if (!((content.Length <= Vars.CurrentConf.MaximumDanmakuLength) && (content.Length >= Vars.CurrentConf.MinimumDanmakuLength)))
            {
                Bridge.ALog($"忽略: 弹幕字数 ({content.Length}) 不符合要求");
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