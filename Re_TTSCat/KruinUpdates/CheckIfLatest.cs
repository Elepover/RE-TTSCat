using System;
using System.Threading.Tasks;

namespace Re_TTSCat
{
    public partial class KruinUpdates
    {
        public static async Task<bool> CheckIfLatestAsync(Version currentVersion)
        {
            Update upd = await Update.GetLatestUpdAsync();
            return CheckIfLatest(upd, currentVersion);
        }
        public static bool CheckIfLatest(Update upd, Version currentVer)
        {
            if (upd.LatestVersion > currentVer)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
