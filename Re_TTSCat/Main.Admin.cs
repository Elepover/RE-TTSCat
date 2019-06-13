using System.Threading;
using BilibiliDM_PluginFramework;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public override void Admin()
        {
            var windowThread = new Thread(() =>
            {
                var window = new Windows.OptionsWindow();
                window.ShowDialog();
            });
            windowThread.SetApartmentState(ApartmentState.STA);
            windowThread.Start();
            base.Admin();
        }
    }
}
