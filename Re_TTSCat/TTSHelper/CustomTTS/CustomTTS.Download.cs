// ** CUSTOM ENGINE IS IN ALPHA, STABILITY NOT GUARANTEED**

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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
                var fileName = Path.Combine(Vars.CacheDir, Conf.GetRandomFileName() + "USER.mp3");
                Bridge.ALog($"(E5) 正在下载 TTS, 文件名: {fileName}, 方法: {Vars.CurrentConf.ReqType}");
                if (Vars.CurrentConf.ReqType == RequestType.ApplicationXWwwFormUrlencoded || Vars.CurrentConf.ReqType == RequestType.TextPlain)
                {
                    Bridge.ALog("POST 模式: text/plain / application/x-www-form-urlencoded");
                    // 配置 Headers
                    var uploader = WebRequest.CreateHttp(Vars.CurrentConf.CustomEngineURL);
                    uploader.Method = "POST";
                    if (Vars.CurrentConf.HttpAuth)
                    {
                        uploader.Credentials = new NetworkCredential(Vars.CurrentConf.HttpAuthUsername, Vars.CurrentConf.HttpAuthPassword);
                        uploader.PreAuthenticate = true;
                    }
                    foreach (var header in Vars.CurrentConf.Headers)
                    {
                        Bridge.ALog($"添加 Header: {header.Name}, 值 {header.Value}");
                        uploader.Headers.Add(header.Name, header.Value);
                    }
                    uploader.ContentType = Vars.CurrentConf.ReqType == RequestType.ApplicationXWwwFormUrlencoded ? "application/x-www-form-urlencoded" : "text/plain";

                    // 准备数据
                    var data = Encoding.UTF8.GetBytes(Vars.CurrentConf.PostData);
                    var dataStream = uploader.GetRequestStream();
                    dataStream.Write(data, 0, data.Length);
                    dataStream.Close();
                    uploader.ContentLength = data.Length;

                    // 等响应
                    using (var res = uploader.GetResponse())
                    {
                        // 写数据
                        using (var writer = new StreamWriter(File.OpenWrite(fileName)) { AutoFlush = true })
                        {
                            byte[] responseData = new byte[res.ContentLength];
                            res.GetResponseStream().Read(responseData, 0, responseData.Length);
                            writer.Write(responseData);
                        }
                    }
                }
                else if (Vars.CurrentConf.ReqType == RequestType.MultipartFormData)
                {
                    using (var httpClient = new HttpClient())
                    {
                        var form = new MultipartFormDataContent();
                        // 配置 Headers
                        if (Vars.CurrentConf.HttpAuth)
                        {
                            var authArray = Encoding.UTF8.GetBytes($"{Vars.CurrentConf.HttpAuthUsername}:{Vars.CurrentConf.HttpAuthPassword}");
                            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authArray));
                        }
                        foreach (var header in Vars.CurrentConf.Headers)
                        {
                            Bridge.ALog($"添加 Header: {header.Name}, 值 {header.Value}");
                            httpClient.DefaultRequestHeaders.Add(header.Name, header.Value);
                        }
                        // 转换数据
                        var postItems = JsonConvert.DeserializeObject<List<Header>>(Vars.CurrentConf.PostData);
                        foreach (var item in postItems)
                        {
                            var data = Convert.FromBase64String(item.Value);
                            form.Add(new ByteArrayContent(data, 0, data.Length), item.Name);
                        }
                        // 抓结果
                        using (var res = await httpClient.PostAsync(Vars.CurrentConf.CustomEngineURL, form))
                        {
                            res.EnsureSuccessStatusCode();
                            // 写数据
                            using (var writer = new StreamWriter(File.OpenWrite(fileName)) { AutoFlush = true })
                            {
                                var responseData = await res.Content.ReadAsByteArrayAsync();
                                writer.Write(responseData);
                            }
                        }
                    }
                        
                }
                else
                {
                    Bridge.ALog("GET 模式");
                    using (var downloader = new WebClient())
                    {
                        downloader.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36");
                        foreach (var header in Vars.CurrentConf.Headers)
                        {
                            Bridge.ALog($"添加 Header: {header.Name}, 值 {header.Value}");
                            downloader.Headers.Add(header.Name, header.Value);
                        }
                        if (Vars.CurrentConf.HttpAuth) downloader.Credentials = new NetworkCredential(Vars.CurrentConf.HttpAuthUsername, Vars.CurrentConf.HttpAuthPassword);
                        await downloader.DownloadFileTaskAsync(Vars.CurrentConf.CustomEngineURL.Replace("$TTSTEXT", content), fileName);
                    }
                }
                return fileName;
            }
            catch (Exception ex)
            {
                Bridge.ALog($"(E5) TTS 下载失败: {ex.Message}");
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
