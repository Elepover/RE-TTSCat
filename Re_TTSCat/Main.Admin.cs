using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public override void Admin()
        {
            if (Keyboard.IsKeyDown(Key.LeftShift))
            {
                Log("正在启动配置重置...");
                var worker = new Thread(async () =>
                {
                    if (MessageBox.Show("您按下了 Shift 键，重置配置吗？", "Re: TTSCat", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
                    Log("重置配置...");
                    try
                    {
                        await Conf.InitiateAsync().ConfigureAwait(false);
                        Vars.CurrentConf = new Conf();
                        await Conf.SaveAsync();
                        Log("重置成功!");
                        MessageBox.Show("重置成功！", "Re: TTSCat", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        Log($"重置过程中出错: {ex.Message}");
                    }
                });
                worker.Start();
                return;
            }
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
