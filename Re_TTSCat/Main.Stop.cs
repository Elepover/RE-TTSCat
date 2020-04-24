using BilibiliDM_PluginFramework;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Threading;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public override void Stop()
        {
            Log("正在尝试停用插件");
            var loadWindow = new Windows.LoadingWindowLight
            {
                Left = Cursor.Position.X,
                Top = Cursor.Position.Y
            };
            loadWindow.Show(); Data.Conf.Delay(25);
            loadWindow.ProgressBar.Value = 30; Data.Conf.Delay(25);

            var frame = new DispatcherFrame();
            var thread = new Thread(() => {
                ALog("正在等待播放器停止");
                Data.Vars.CallPlayerStop = true;
                while (Data.Vars.Player.IsAlive) Thread.Sleep(100);
                frame.Continue = false;
            });
            thread.Start();
            Dispatcher.PushFrame(frame);
            loadWindow.ProgressBar.Value = 100; Data.Conf.Delay(25);
            loadWindow.Close();

            Log("插件已停用");
            IsEnabled = false;
            base.Stop();
        }
    }
}
