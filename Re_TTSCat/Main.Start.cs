using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public override async void Start()
        {
            Log("插件启动中");
            var loadWindow = new Windows.LoadingWindowLight();
            try
            {
                loadWindow.IsOpen = true;
                loadWindow.ProgressBar.Value = 30; Conf.Delay(10);
                ALog("正在检查配置");
                await Conf.InitiateAsync();
                loadWindow.ProgressBar.Value = 60; Conf.Delay(10);
                ALog("配置初始化成功");
                ALog("正在启用播放器");
                TTSPlayer.Init();
                loadWindow.ProgressBar.Value = 100; Conf.Delay(10);
                loadWindow.IsOpen = false;
                Log("启动成功");
                IsEnabled = true;
                if ((Vars.ManagementWindow != null) && !Vars.ManagementWindow.WindowDisposed)
                    Vars.ManagementWindow.Dispatcher.Invoke(() => { Vars.ManagementWindow.CheckBox_IsPluginActive.IsChecked = true; });
                base.Start();
            }
            catch (Exception ex)
            {
                loadWindow.IsOpen = false;
                Log($"启动过程中出错: {ex}");
                Windows.AsyncDialog.Open("启动失败，更多信息请查看日志（首页）。\n请在反馈错误时附加日志信息。\n\n如您在后期继续使用时遇到问题，请尝试重新启动弹幕姬。", "Re: TTSCat", MessageBoxIcon.Error);
                Log("启动失败");
            }
        }
    }
}
