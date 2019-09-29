using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public static partial class CustomTTS
    {
        public static async Task<string> Download(string content)
        {
            var errorCount = 0;
Retry:
            try
            {
                var fileName = Path.Combine(Vars.cacheDir, Conf.GetRandomFileName() + "USER.mp3");
                if (Vars.CurrentConf.DebugMode)
                {
                    Bridge.Log("(E5) 正在下载 TTS, 文件名: " + fileName);
                }
                var downloader = new WebClient();
                downloader.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36");
                await downloader.DownloadFileTaskAsync(Vars.CurrentConf.CustomEngineURL.Replace("$TTSTEXT", content), fileName);
                downloader.Dispose();
                return fileName;
            }
            catch (Exception ex)
            {
                if (Vars.CurrentConf.DebugMode)
                {
                    Bridge.Log("(E5) TTS 下载失败: " + ex.Message);
                }
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
