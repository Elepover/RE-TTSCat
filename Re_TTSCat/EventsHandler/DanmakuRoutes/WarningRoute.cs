using BilibiliDM_PluginFramework;
using System.Threading.Tasks;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public async Task WarningRoute(object sender, ReceivedDanmakuArgs e)
        {
            await TTSPlayer.UnifiedPlay(Vars.CurrentConf.OnWarning, true);
        }
    }
}