using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public static class BaiduTTS
    {
        public static async Task<string> Download(string content)
        {
            var errorCount = 0;
Retry:
            try
            {
                var fileName = Path.Combine(Vars.cacheDir, Conf.GetRandomFileName() + "BIDU.mp3");
                if (Vars.CurrentConf.DebugMode)
                {
                    Bridge.Log("(E0) 正在下载 TTS, 文件名: " + fileName);
                }
                var downloader = new WebClient();
                await downloader.DownloadFileTaskAsync(Vars.apiBaidu.Replace("$TTSTEXT", content),
                                                       fileName);
                downloader.Dispose();
                return fileName;
            }
            catch (Exception ex)
            {
                if (Vars.CurrentConf.DebugMode)
                {
                    Bridge.Log("(E0) TTS 下载失败: " + ex.Message);
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
