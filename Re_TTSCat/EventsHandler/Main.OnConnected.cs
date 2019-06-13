using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public async void OnConnected(object sender, ConnectedEvtArgs e)
        {
            await TTSPlayer.UnifiedPlay(
                Vars.CurrentConf.OnConnected.Replace(
                    "$ROOM", e.roomid.ToString()
                )
            );
        }
    }
}
