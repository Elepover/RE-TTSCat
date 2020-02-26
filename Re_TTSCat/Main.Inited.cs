using BilibiliDM_PluginFramework;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public override async void Inited()
        {
            // to optimize startup time, we minimized the behaviors in Inited()
            Log("正在初始化");
            await Data.Conf.InitiateAsync();
            Log("初始化成功");
            base.Inited();
        }
    }
}
