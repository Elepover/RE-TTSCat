using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public string ProcessGift(ReceivedDanmakuArgs e)
        {
            var rawDanmaku = Preprocess(Vars.CurrentConf.OnGift, e);
            return rawDanmaku
                .Replace("$GIFT", e.Danmaku.GiftName)
                .Replace("$COUNT", e.Danmaku.GiftCount.ToString())
                .Replace("$USER", e.Danmaku.UserName);
        }
    }
}