using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public static partial class BaiduTTS
    {
        public static partial class AiApi
        {
            public static async Task<string> Download(string content, SpeechPerson person)
            {
                var errorCount = 0;
            Retry:
                try
                {
                    var fileName = Path.Combine(Vars.CacheDir, Conf.GetRandomFileName() + "BDAI.mp3");
                    using (var downloader = new WebClient())
                    {
                        if (string.IsNullOrEmpty(Vars.ApiBaiduAiAccessToken))
                        {
                            Bridge.ALog("(E6) 正在获取 Access token...");
                            var rawJson =
                                await downloader.DownloadStringTaskAsync(
                                    $"https://openapi.baidu.com/oauth/2.0/token?grant_type=client_credentials&client_id={Vars.ApiBaiduAiAppKey}&client_secret={Vars.ApiBaiduAiSecretKey}"
                                    );
                            Vars.ApiBaiduAiAccessToken = JObject.Parse(rawJson)["access_token"].ToString();
                            Bridge.ALog("(E6) Token 获取成功，本次运行时或 30 天内有效");
                        }
                        Bridge.ALog("(E6) 正在下载 TTS, 文件名: " + fileName);
                        downloader.Headers.Add(HttpRequestHeader.AcceptEncoding, "identity;q=1, *;q=0");
                        downloader.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36");
                        var url = Vars.ApiBaiduAi
                            .Replace("$PERSON", ((int)person).ToString())
                            .Replace("$SPEED", ConvertSpeed(Vars.CurrentConf.ReadSpeed).ToString())
                            .Replace("$TOKEN", Vars.ApiBaiduAiAccessToken)
                            .Replace("$TTSTEXT", content);
                        await downloader.DownloadFileTaskAsync(url, fileName);
                        return fileName;
                    }
                }
                catch (Exception ex)
                {
                    Bridge.ALog("(E6) TTS 下载失败: " + ex.Message);
                    errorCount += 1;
                    if (errorCount <= Vars.CurrentConf.DownloadFailRetryCount)
                    {
                        goto Retry;
                    }
                    return null;
                }
            }
        }
    }
}
