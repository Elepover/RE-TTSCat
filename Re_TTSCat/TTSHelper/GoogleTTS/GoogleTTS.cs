using Re_TTSCat.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

// source: https://github.com/Taybenberg/GoogleTTS/blob/master/csGoogleTTS/gTTS.cs, modified

namespace Re_TTSCat
{
    public class GoogleTTS
    {
        private const string TtsUrl = "https://translate.google.cn/translate_tts";
        private const string UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.17763";

        private const ushort MaxTokenLength = 175;

        private readonly List<byte[]> byteList = new List<byte[]>();

        private readonly string lang;

        public GoogleTTS(string text, string language)
        {
            lang = language;

            if (text.Length <= MaxTokenLength)
                DownloadByteArray(text);
            else
                Tokenizer(text);
        }

        private void DownloadByteArray(string text)
        {
            using (var webClient = new WebClient())
            {
                webClient.Headers.Add(HttpRequestHeader.Referer, "https://translate.google.cn/");
                webClient.Headers.Add("user-agent", UserAgent);
                webClient.QueryString.Add("ie", "UTF-8");
                webClient.QueryString.Add("client", "tw-ob");
                webClient.QueryString.Add("tl", lang);
                webClient.QueryString.Add("q", WebUtility.UrlEncode(text));
                webClient.UseDefaultCredentials = true;

                byteList.Add(webClient.DownloadData(TtsUrl));
            }
        }

        public void WriteFile(string path)
        {
            using (var fileStream = File.OpenWrite(path))
            {
                using (var binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(ToByteArray());

                    binaryWriter.Flush();
                    binaryWriter.Close();

                    fileStream.Close();
                }
            }
        }

        public byte[] ToByteArray()
        {
            byte[] arr = null;

            foreach (var b in byteList)
                if (arr == null)
                    arr = b;
                else
                    arr.Concat(b);

            return arr;
        }

        private void Tokenizer(string text)
        {
            foreach (var str0 in text.Split(new[] { '!', '?', '.', ';', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (str0.Length <= MaxTokenLength)
                    DownloadByteArray(str0);
                else
                    foreach (var str1 in str0.Split(new[] { ':', ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (str1.Length <= MaxTokenLength)
                            DownloadByteArray(str1);
                        else
                            foreach (var str2 in str1.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (str2.Length <= MaxTokenLength)
                                    DownloadByteArray(str2);
                                else
                                {
                                    var str3 = str2;

                                    do
                                    {
                                        DownloadByteArray(str3.Substring(0, MaxTokenLength));
                                        str3 = str3.Remove(0, MaxTokenLength);
                                    } while (str3.Length > MaxTokenLength);

                                    DownloadByteArray(str3);
                                }
                            }
                    }
            }
        }
        public static string Download(string content, string language)
        {
            var errorCount = 0;
Retry:
            try
            {
                var fileName = Path.Combine(Vars.cacheDir, Conf.GetRandomFileName() + "GOOG.mp3");
                if (Vars.CurrentConf.DebugMode) Bridge.Log("(E2) 正在下载 TTS, 文件名: " + fileName);
                var instance = new GoogleTTS(content, language);
                instance.WriteFile(fileName);
                return fileName;
            }
            catch (Exception ex)
            {
                if (Vars.CurrentConf.DebugMode) Bridge.Log("(E2) TTS 下载失败: " + ex.Message);
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
