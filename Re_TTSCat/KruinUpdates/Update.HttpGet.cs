using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class KruinUpdates
    {
        public partial class Update
        {
            private static async Task<string> HttpGet(Uri uri)
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Referer = "https://www.danmuji.org/";
                request.UserAgent = "KruinUpdates/" + Vars.CurrentVersion.ToString() + " (Re-TTSCat;)";
                using (HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync())
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            var text = reader.ReadToEnd();
                            return text;
                        }
                    }
                }
            }
        }
    }
}
