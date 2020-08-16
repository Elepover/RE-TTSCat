using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;
using System.Threading.Tasks;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public async Task InteractRoute(object sender, ReceivedDanmakuArgs e)
        {
            // check user eligibility
            if (!Conf.CheckUserEligibility(e)) return;
            Bridge.ALog("规则检查通过，正在朗读用户交互事件");

            string result;

            switch (e.Danmaku.InteractType)
            {
                case InteractTypeEnum.Enter:
                    result = ProcessInteract(e.Danmaku, Vars.CurrentConf.OnInteractEnter);
                    break;
                case InteractTypeEnum.Follow:
                    result = ProcessInteract(e.Danmaku, Vars.CurrentConf.OnInteractFollow);
                    break;
                case InteractTypeEnum.MutualFollow:
                    result = ProcessInteract(e.Danmaku, Vars.CurrentConf.OnInteractMutualFollow);
                    break;
                case InteractTypeEnum.Share:
                    result = ProcessInteract(e.Danmaku, Vars.CurrentConf.OnInteractShare);
                    break;
                case InteractTypeEnum.SpecialFollow:
                    result = ProcessInteract(e.Danmaku, Vars.CurrentConf.OnInteractSpecialFollow);
                    break;
                default: return;
            }

            await TTSPlayer.UnifiedPlay(result);
        }
    }
}