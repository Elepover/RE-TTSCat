using Re_TTSCat.Data;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace Re_TTSCat
{
    public static class YoudaoTTS
    {
        public static async Task<string> Download(string content)
        {
            {
                var errorCount = 0;
            Retry:
                try
                {
                    var fileName = Path.Combine(Vars.CacheDir, Conf.GetRandomFileName() + "YODO.mp3");
                    Bridge.ALog("(E4) 正在下载 TTS, 文件名: " + fileName);
                    using (var downloader = new WebClient())
                    {
                        downloader.Headers.Add(HttpRequestHeader.AcceptEncoding, "identity;q=1, *;q=0");
                        downloader.Headers.Add(HttpRequestHeader.Referer, "http://fanyi.youdao.com/");
                        downloader.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36");
                        await downloader.DownloadFileTaskAsync(Vars.ApiYoudao.Replace("$TTSTEXT", content),
                                                               fileName);
                        return fileName;
                    }
                }
                catch (Exception ex)
                {
                    Bridge.ALog("(E4) TTS 下载失败: " + ex.Message);
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