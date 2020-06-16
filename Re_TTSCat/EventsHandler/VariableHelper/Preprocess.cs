using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        /// <summary>
        /// 将 $$, $!, $VIP 和 $GUARD 预处理
        /// </summary>
        /// <param name="format">原始模板</param>
        /// <param name="e">原始事件</param>
        /// <returns></returns>
        public static string Preprocess(string format, DanmakuModel e)
        {
            string guardText;
            switch (e.UserGuardLevel)
            {
                default: guardText = Vars.CurrentConf.CustomGuardLevel0; break;
                case 1: guardText = Vars.CurrentConf.CustomGuardLevel1; break;
                case 2: guardText = Vars.CurrentConf.CustomGuardLevel2; break;
                case 3: guardText = Vars.CurrentConf.CustomGuardLevel3; break;
            }
            format = format
                .Replace("$$", "$GUARD$")
                .Replace("$!", "$VIP$");
            if (e.isVIP) format = format.Replace("$VIP", Vars.CurrentConf.CustomVIP);
            else format = format.Replace("$VIP", "");
            return format.Replace("$GUARD", guardText);
        }

        public static string Preprocess(string format, ReceivedDanmakuArgs e) => Preprocess(format, e.Danmaku);
    }
}