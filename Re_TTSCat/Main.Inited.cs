using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;
using System;
using System.IO;
using System.Threading;
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
                var frame = new DispatcherFrame();
                var thread = new Thread(() =>
                {
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
                    Log(totalDeleted == 0 ? "无需清理缓存" : $"已清理 {totalDeleted} 个缓存文件");
                    frame.Continue = false;
                });
                thread.Start();
                Dispatcher.PushFrame(frame);
            }
        }
    }
}
