using BilibiliDM_PluginFramework;
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
            var loadWindow = new Windows.LoadingWindowLight
            {
                Left = Cursor.Position.X,
                Top = Cursor.Position.Y
            };
            try
            {
                loadWindow.Show();
                loadWindow.ProgressBar.Value = 30; Data.Conf.Delay(10);
                ALog("正在检查配置");
                await Data.Conf.InitiateAsync();
                loadWindow.ProgressBar.Value = 50; Data.Conf.Delay(10);
                ALog("配置初始化成功");
                ALog("正在启用播放器");
                TTSPlayer.Init();
                loadWindow.ProgressBar.Value = 80; Data.Conf.Delay(10);
                if (Data.Vars.CurrentConf.AutoUpdate)
                {
                    ALog("正在启动更新检查");
                    Thread updateChecker = new Thread(async () =>
                    {
                        ALog("正在检查更新");
                        try
                        {
                            var latestVersion = await KruinUpdates.Update.GetLatestUpdAsync();
                            var currentVersion = Data.Vars.currentVersion;
                            if (KruinUpdates.CheckIfLatest(latestVersion, currentVersion))
                            {
                                ALog("插件已为最新 (" + Data.Vars.currentVersion.ToString() + ")");
                            }
                            else
                            {
                                Log("发现更新: " + latestVersion.LatestVersion.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            Log("检查更新时发生错误: " + ex.Message);
                        }
                    });
                    updateChecker.Start();
                }
                loadWindow.ProgressBar.Value = 100; Data.Conf.Delay(10);
                loadWindow.Close();
                Log("启动成功");
                IsEnabled = true;
                base.Start();
            }
            catch (Exception ex)
            {
                try
                {
                    loadWindow.Close();
                }
                catch { }
                Log($"启动过程中出错: {ex.ToString()}");
                Windows.AsyncDialog.Open("启动失败，更多信息请查看日志（首页）。\n请在反馈错误时附加日志信息。\n\n如您在后期继续使用时遇到问题，请尝试重新启动弹幕姬。", "Re: TTSCat", MessageBoxIcon.Error);
                Log("启动失败");
            }
        }
    }
}
