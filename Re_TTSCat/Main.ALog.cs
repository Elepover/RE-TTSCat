using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public void ALog(string log)
        {
            if (Vars.CurrentConf.DebugMode) Log(log);
        }
    }
}
