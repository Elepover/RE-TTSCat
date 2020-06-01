using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;

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
                case MsgTypeEnum.SuperChat:
                    await SuperChatRoute(sender, e);
                    break;
                case MsgTypeEnum.GiftSend:
                    if (Vars.CurrentConf.GiftsThrottle)
                    {
                        ALog($"礼物合并已启用，正在合并礼物: 来自 {e.Danmaku.UserName} ({e.Danmaku.UserID}) 的 {e.Danmaku.GiftCount} 个 {e.Danmaku.GiftName}");
                        Vars.Debouncer.Add(new UserGift
                        (
                            e.Danmaku.UserName,
                            e.Danmaku.UserID,
                            e.Danmaku.GiftName,
                            e.Danmaku.GiftCount
                        ));
                    }
                    else
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
