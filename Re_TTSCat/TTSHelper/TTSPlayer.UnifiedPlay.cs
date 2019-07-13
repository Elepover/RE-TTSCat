using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public static partial class TTSPlayer
    {
        /// <summary>
        /// Play something, other options are defined by current configurations
        /// </summary>
        /// <param name="content"></param>
        public static async Task UnifiedPlay(string content)
        {
            if (content.Replace(" ", "") == "")
            {
                if (Vars.CurrentConf.DebugMode)
                {
                    Bridge.Log("放弃: 内容为空");
                }
                return;
            }
            if (Vars.CurrentConf.DebugMode)
            {
                Bridge.Log("尝试朗读: " + content);
            }
            if (!Conf.GetRandomBool(Vars.CurrentConf.ReadPossibility))
            {
                if (Vars.CurrentConf.DebugMode)
                {
                    Bridge.Log("放弃: 已随机丢弃");
                    return;
                }
            }
            string fileName;
            switch (Vars.CurrentConf.Engine)
            {
                default:
                    fileName = await BaiduTTS.Download(content);
                    break;
                case 1:
                    fileName = FrameworkTTS.Download(content);
                    break;
                case 2:
                    fileName = GoogleTTS.Download(content, "zh-CN");
                    break;
                case 3:
                    fileName = await BaiduTTS.DownloadCantonese(content);
                    break;
            }
            if (fileName == null)
            {
                Bridge.Log("下载失败，丢弃");
                return;
            }
            if (Vars.CurrentConf.ReadInQueue)
            {
                if (Vars.CurrentConf.DebugMode)
                {
                    Bridge.Log("正在添加下列文件到播放列表: " + fileName);
                }
                readerList.Add(new AudioFileReader(fileName));
            }
            else
            {
                if (Vars.CurrentConf.DebugMode)
                {
                    Bridge.Log("正在直接播放: " + fileName);
                }
                var thread = new Thread(() =>
                {
                    var waveOut = new WaveOutEvent();
                    var reader = new AudioFileReader(fileName);
                    waveOut.Init(reader);
                    waveOut.Volume = ((float)Vars.CurrentConf.TTSVolume) / 100;
                    if (Vars.CurrentConf.DebugMode)
                    {
                        Bridge.Log("音量设置为: " + waveOut.Volume);
                    }
                    waveOut.Play();
                    while (waveOut.PlaybackState != PlaybackState.Stopped) { Thread.Sleep(50); }
                    reader.Dispose();
                    waveOut.Dispose();
                    if (Vars.CurrentConf.DoNotKeepCache)
                    {
                        File.Delete(fileName);
                    }
                });
                thread.Start();
            }
            
        }
    }
}
