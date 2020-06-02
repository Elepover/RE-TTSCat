using Re_TTSCat.Data;
using System.Collections.Generic;

namespace Re_TTSCat
{
    public static partial class TTSPlayer
    {
        public static List<TTSEntry> fileList = new List<TTSEntry>();
        public static float Volume { get => ((float)Vars.CurrentConf.TTSVolume) / 100; }
    }
}
