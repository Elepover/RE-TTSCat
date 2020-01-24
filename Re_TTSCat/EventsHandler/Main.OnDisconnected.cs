using System;
using System.IO;
using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public async void OnDisconnected(object sender, DisconnectEvtArgs e)
        {
            if (!IsNAudioReady) return;
            if (e == null)
            {
                await TTSPlayer.UnifiedPlay(
                    Vars.CurrentConf.OnDisconnected.Replace(
                        "$ERROR", ""
                    )
                );
            }
            else
            {
                await TTSPlayer.UnifiedPlay(
                    Vars.CurrentConf.OnDisconnected.Replace(
                        "$ERROR", e.Error.Message
                    )
                );
            }
        }
    }
}
