using System.Threading;

namespace Re_TTSCat
{
    public static partial class TTSPlayer
    {
        public static void Init()
        {
            Data.Vars.Player = new Thread(() => PlayerThread())
            {
                IsBackground = true
            };
            Data.Vars.Player.Start();
        }
    }
}
