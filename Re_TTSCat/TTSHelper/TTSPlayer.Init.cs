using Re_TTSCat.Data;
using System.Threading;

namespace Re_TTSCat
{
    public static partial class TTSPlayer
    {
        public static void Init()
        {
            Vars.Player = new Thread(() => PlayerThread())
            {
                IsBackground = true
            };
            Vars.Player.Start();
        }
    }
}
