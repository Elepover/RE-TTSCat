using System.Threading;
using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public override void Admin()
        {
            if ((Vars.ManagementWindow == null) || ((Vars.ManagementWindow.WindowDisposed)))
            {
                var windowThread = new Thread(() =>
                {
                    Vars.ManagementWindow = new Windows.OptionsWindow();
                    Vars.ManagementWindow.ShowDialog();
                });
                windowThread.SetApartmentState(ApartmentState.STA);
                windowThread.Start();
            }
            else
            {
                Vars.ManagementWindow.Dispatcher.Invoke(() => { Vars.ManagementWindow.Activate(); });
            }
            base.Admin();
        }
    }
}
