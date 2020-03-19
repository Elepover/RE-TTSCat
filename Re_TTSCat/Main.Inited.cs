using BilibiliDM_PluginFramework;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public override async void Inited()
        {
            // to optimize startup time, we minimized the behaviors in Inited()
            // initialize bridge
            Data.Bridge.MainInstance = this;
            // mainly to initialize support library in case something bad happens
            Log("正在初始化");
            await Data.Conf.InitiateAsync();
            Log("初始化成功");
            base.Inited();
        }
    }
}
