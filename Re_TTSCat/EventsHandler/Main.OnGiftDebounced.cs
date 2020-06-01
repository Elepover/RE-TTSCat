using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        private async void GiftDebouncedEvent(object sender, UserGift e)
        {
            ALog($"合并礼物: 来自 {e.User} ({e.UserId}) 的 {e.Qty} 个 {e.Gift}，直接路由到礼物路线...");
            var constructedArgs = new ReceivedDanmakuArgs();
            constructedArgs.Danmaku = new DanmakuModel();
            constructedArgs.Danmaku.GiftName = e.Gift;
            constructedArgs.Danmaku.GiftCount = e.Qty;
            constructedArgs.Danmaku.UserName = e.User;
            constructedArgs.Danmaku.UserID = e.UserId;
            await GiftRoute(null, constructedArgs);
        }
    }
}
