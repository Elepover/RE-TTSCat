using BilibiliDM_PluginFramework;
using System;
using System.IO;
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
                loadWindow.ProgressBar.Value = 20; Data.Conf.Delay(25);
                Log("正在检查文件");
                if (!File.Exists(Data.Vars.audioLibFileName))
                {
                    MessageBox.Show("未能找到 NAudio.dll 音频库文件。\n\n请您尝试将 NAudio.dll 与插件本体置于相同文件夹（即弹幕姬插件文件夹中）并重启弹幕姬。\n\n在您补回该文件前，插件将无法启动，但您仍可以打开管理窗口来修改配置。", "Re: TTSCat", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new FileNotFoundException("已侦测到音频库丢失");
                }
                Log("正在启用数据桥");
                RunBridge();
                loadWindow.ProgressBar.Value = 30; Data.Conf.Delay(25);
                Log("正在初始化配置");
                await Data.Conf.InitiateAsync();
                loadWindow.ProgressBar.Value = 50; Data.Conf.Delay(25);
                Log("配置初始化成功");
                Log("正在启用播放器");
                TTSPlayer.Init();
                loadWindow.ProgressBar.Value = 80; Data.Conf.Delay(25);
                if (Data.Vars.CurrentConf.AutoUpdate)
                {
                    Log("正在检查更新");
                    try
                    {
                        var latestVersion = await KruinUpdates.Update.GetLatestUpdAsync();
                        var currentVersion = Data.Vars.currentVersion;
                        if (KruinUpdates.CheckIfLatest(latestVersion, currentVersion))
                        {
                            Log("插件已为最新 (" + Data.Vars.currentVersion.ToString() + ")");
                        }
                        else
                        {
                            Log("发现更新: " + latestVersion.LatestVersion.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        Log("检查更新出错: " + ex.Message);
                    }

                }
                loadWindow.ProgressBar.Value = 100; Data.Conf.Delay(25);
                loadWindow.Close();
                Log("启动成功");
                base.Start();
            }
            catch (Exception ex)
            {
                try
                {
                    loadWindow.Close();
                }
                catch { }
                Log("启动过程中出错: " + ex.ToString());
                Windows.AsyncDialog.Open("启动失败，更多信息请查看日志。\n\n如您在后期继续使用时遇到问题，请尝试重新启动弹幕姬。", "Re: TTSCat", MessageBoxIcon.Error);
                Log("启动失败");
            }
        }
    }
}
