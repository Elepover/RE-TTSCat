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
            if (Vars.CurrentConf.FullyAutomaticUpdate && Vars.CurrentConf.AutoUpdate)
            {
                ALog("正在启动更新检查");
                Thread updateChecker = new Thread(() => UpdateThread());
                updateChecker.Start();
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
            if (Vars.CurrentConf.AutoBaiduFallback && Vars.CurrentConf.Engine == 6 && string.IsNullOrWhiteSpace(Vars.CurrentConf.BaiduApiKey))
            {
                Log("您正在使用百度高级版引擎且未配置公钥/私钥，已自动回落至百度引擎");
                Vars.CurrentConf.Engine = 0;
                await Conf.SaveAsync();
            }
            if (Vars.CurrentConf?.AutoStartOnLoad == true) Start();
            if (!Vars.SystemSpeechAvailable)
            {
                Log("技术信息：");
                Log(Vars.SpeechUnavailableString);
                Log("警告：无法初始化 .NET 框架引擎！您可能正在使用修改版 Windows.");
            }
        }
    }
}
