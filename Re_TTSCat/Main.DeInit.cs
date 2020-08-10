using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public override void DeInit()
        {
            if ((Vars.ManagementWindow != null) && (!Vars.ManagementWindow.WindowDisposed)) Vars.ManagementWindow.Dispatcher.Invoke(() => { Vars.ManagementWindow.Close(); });
            Vars.CallPlayerStop = true;
            Vars.Player?.Interrupt();
            base.DeInit();
        }
    }
}
