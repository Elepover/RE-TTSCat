using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public override async void Inited()
        {
            // to optimize startup time, we minimized the behaviors in Inited()
            // initialize bridge
            Bridge.MainInstance = this;
            // mainly to initialize support library in case something bad happens
            Log("正在初始化");
            await Conf.InitiateAsync();
            Log("初始化成功");
            base.Inited();
            // finish cache cleanup
            if (Vars.CurrentConf.ClearCacheOnStartup)
            {
                var thread = new Thread(() =>
                {
                    var sw = new Stopwatch();
                    sw.Start();
                    int totalDeleted = 0;
                    foreach (var file in Directory.GetFiles(Vars.CacheDir))
                    {
                        try
                        {
                            File.Delete(file);
                            totalDeleted++;
                        }
                        catch (Exception ex)
                        {
                            ALog($"无法删除日志文件 {Path.GetFileName(file)}, 跳过: {ex.Message}");
                        }
                    }
                    Log(totalDeleted == 0 ? "无需清理缓存" : $"已清理 {totalDeleted} 个缓存文件, 用时 {Math.Round(sw.Elapsed.TotalMilliseconds, 2)}ms");
                    sw.Stop();
                });
                thread.Start();
            }
            try
            {
                if (Vars.CurrentConf?.OverrideToLogsTabOnStartup == true)
                {
                    var window = System.Windows.Application.Current.MainWindow;
                    var tabControl = (TabControl)window.FindName("TabControl");
                    tabControl.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                ALog($"无法切换到首页: {ex.Message}");
            }
            if (Vars.CurrentConf?.AutoStartOnLoad == true) Start();
        }
    }
}
