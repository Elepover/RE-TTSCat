using BilibiliDM_PluginFramework;
using System;
using System.IO;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public async void OnReceivedDanmaku(object sender, ReceivedDanmakuArgs e)
        {
            if (!IsNAudioReady) return;
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
    }
}
