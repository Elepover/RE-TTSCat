using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public void OnReceivedRoomCount(object sender, ReceivedRoomCountArgs e)
        {
            Vars.RoomCount = e.UserCount;
            Bridge.ALog("OnReceivedRoomCount: " + e.UserCount);
        }
    }
}
