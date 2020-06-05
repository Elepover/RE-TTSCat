using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
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
                loadWindow.ProgressBar.Value = 80; Conf.Delay(10);
                if (Vars.CurrentConf.AutoUpdate)
                {
                    ALog("正在启动更新检查");
                    Thread updateChecker = new Thread(async () =>
                    {
                        ALog("正在检查更新");
                        try
                        {
                            var latestVersion = await KruinUpdates.Update.GetLatestUpdAsync();
                            var currentVersion = Vars.CurrentVersion;
                            if (KruinUpdates.CheckIfLatest(latestVersion, currentVersion))
                            {
                                ALog("插件已为最新 (" + Vars.CurrentVersion.ToString() + ")");
                            }
                            else
                            {
                                Log($"发现更新: {latestVersion.LatestVersion}, 正在下载...");
                                try
                                {
                                    ALog("正在下载更新...");
                                    using (var downloader = new WebClient())
                                    {
                                        await downloader.DownloadFileTaskAsync("https://www.danmuji.org" + latestVersion.DownloadLink, Vars.DownloadUpdateFilename);
                                    }

                                    ALog("正在备份...");
                                    var backupFilename = Path.Combine(Vars.ConfDir, $"Re_TTSCat_v{Vars.CurrentVersion}.dll");
                                    if (File.Exists(backupFilename)) File.Delete(backupFilename);
                                    File.Move(Vars.AppDllFileName, backupFilename);

                                    ALog("正在解压...");
                                    using (var zip = ZipFile.OpenRead(Vars.DownloadUpdateFilename))
                                    {
                                        foreach (var entry in zip.Entries)
                                        {
                                            entry.ExtractToFile(Path.Combine(Vars.AppDllFilePath, entry.FullName), true);
                                        }
                                    }

                                    ALog("正在清理...");
                                    if (File.Exists(Vars.DownloadUpdateFilename)) File.Delete(Vars.DownloadUpdateFilename);

                                    Log("更新成功！重启弹幕姬即可生效");
                                    Vars.UpdatePending = true;
                                }
                                catch (Exception ex)
                                {
                                    ALog($"更新失败: {ex}");
                                    Log("更新失败");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log("检查更新时发生错误: " + ex.Message);
                        }
                    });
                    updateChecker.Start();
                }
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
