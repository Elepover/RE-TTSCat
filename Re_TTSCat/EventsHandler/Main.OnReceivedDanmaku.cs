using BilibiliDM_PluginFramework;
using System;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public async void OnReceivedDanmaku(object sender, ReceivedDanmakuArgs e)
        {
            try
            {
                switch (e.Danmaku.MsgType)
                {
                    case MsgTypeEnum.Comment:
                        await CommentRoute(sender, e);
                        break;
                    case MsgTypeEnum.GiftSend:
                        await GiftRoute(sender, e);
                        break;
                    case MsgTypeEnum.GuardBuy:
                        await GuardBuyRoute(sender, e);
                        break;
                    case MsgTypeEnum.LiveStart:
                        await LiveStartRoute(sender, e);
                        break;
                    case MsgTypeEnum.LiveEnd:
                        await LiveEndRoute(sender, e);
                        break;
                    case MsgTypeEnum.Welcome:
                        await WelcomeRoute(sender, e);
                        break;
                    case MsgTypeEnum.WelcomeGuard:
                        await WelcomeGuardRoute(sender, e);
                        break;
                }
            }
            catch (Exception ex)
            {
                if (Data.Vars.CurrentConf.DebugMode)
                {
                    Data.Bridge.Log("消息处理出错: " + ex.Message);
                }
            }
        }
    }
}
