﻿using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using NAudio.Wave;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public static partial class BaiduTTS
    {
        public static async Task<string> Download(string content, bool cantonese = false)
        {
            var errorCount = 0;
Retry:
            try
            {
                var fileName = Path.Combine(Vars.CacheDir, Conf.GetRandomFileName() + "BIDU.mp3");
                Bridge.ALog("(E0) 正在下载 TTS, 文件名: " + fileName);
                using (var downloader = new WebClient())
                {
                    string url;
                    downloader.Headers.Add(HttpRequestHeader.AcceptEncoding, "identity;q=1, *;q=0");
                    downloader.Headers.Add(HttpRequestHeader.Referer, "https://fanyi.baidu.com/");
                    downloader.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36");
                    if (cantonese)
                    {
                        url = Vars.ApiBaiduCantonese.Replace("$TTSTEXT", content);
                    }
                    else
                    {
                        url = Vars.ApiBaidu.Replace("$TTSTEXT", content);
                    }
                    await downloader.DownloadFileTaskAsync(url, fileName);
                    // validate if file is playable
                    using (var reader = new AudioFileReader(fileName)) { }
                    return fileName;
                }
            }
            catch (Exception ex)
            {
                Bridge.ALog("(E0) TTS 下载失败: " + ex.Message);
                errorCount += 1;
                Vars.TotalFails++;
                if (errorCount <= Vars.CurrentConf.DownloadFailRetryCount)
                {
                    goto Retry;
                }
                return null;
            }
        }
    }
}
