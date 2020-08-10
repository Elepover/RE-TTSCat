using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        private async void UpdateThread()
        {
            ALog("正在检查更新");
            try
            {
                var latestVersion = await KruinUpdates.Update.GetLatestUpdAsync();
                var currentVersion = Vars.CurrentVersion;
                if (KruinUpdates.CheckIfLatest(latestVersion, currentVersion))
                {
                    ALog("插件已为最新 (" + Vars.CurrentVersion.ToString() + ")");
                    Vars.UpdatePending = false;
                }
                else
                {
                    if (Vars.UpdatePending) return; // avoid duplicate updates
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
        }
    }
}