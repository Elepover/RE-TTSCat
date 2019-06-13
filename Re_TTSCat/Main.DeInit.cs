using BilibiliDM_PluginFramework;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public override void DeInit()
        {
            Stop();
            base.DeInit();
        }
    }
}
