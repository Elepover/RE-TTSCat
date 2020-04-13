using System.IO;
using System.Linq;

namespace Re_TTSCat.Data
{
    public partial class Conf
    {
        public static bool VerifyLibraryIntegrity()
        {
            if (!File.Exists(Vars.audioLibFileName)) return false;
            using (var stream = File.OpenRead(Vars.audioLibFileName))
            {
                var data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                if (data.SequenceEqual(Properties.Resources.NAudio))
                {
                    Bridge.ALog("支持库完整性检查通过");
                    return true;
                }
                Bridge.ALog("支持库完整性未通过");
                return false;
            }
        }
    }
}
