using BilibiliDM_PluginFramework;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public static string ProcessInteract(DanmakuModel e, string template)
        {
            var rawDanmaku = Preprocess(template, e);
            return rawDanmaku.Replace("$USER", e.UserName);
        }
    }
}