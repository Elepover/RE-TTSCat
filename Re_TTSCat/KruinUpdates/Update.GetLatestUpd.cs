using System;
using System.Threading.Tasks;

namespace Re_TTSCat
{
    public partial class KruinUpdates
    {
        public partial class Update
        {
            public static async Task<Update> GetLatestUpdAsync()
            {
                string gottenResult;
                gottenResult = await HttpGet(new Uri("https://www.danmuji.org/api/v2/Re-TTSCat"));
                Newtonsoft.Json.Linq.JObject jsonObj = Newtonsoft.Json.Linq.JObject.Parse(gottenResult);
                Version latestVer = new Version(jsonObj["version"].ToString());
                string updDesc = jsonObj["update_desc"].ToString();
                DateTime updTime = DateTimeOffset.Parse(jsonObj["update_datetime"].ToString(), null).DateTime;
                string dlLink = jsonObj["dl_url"].ToString();
                return new Update(latestVer, updTime, updDesc, dlLink);
            }
        }
    }
}
