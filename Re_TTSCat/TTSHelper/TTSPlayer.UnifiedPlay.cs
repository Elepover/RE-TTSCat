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
                Bridge.ALog("放弃: 内容为空");
                return;
            }
            Bridge.ALog("尝试朗读: " + content);
            if (!Conf.GetRandomBool(Vars.CurrentConf.ReadPossibility))
            {
                Bridge.ALog("放弃: 已随机丢弃");
                return;
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
                    fileName = await BaiduTTS.Download(content, true);
                    break;
                case 4:
                    fileName = await YoudaoTTS.Download(content);
                    break;
                case 5:
                    fileName = await CustomTTS.Download(content);
                    break;
            }
            if (fileName == null)
            {
                Bridge.ALog("下载失败，丢弃");
                return;
            }
            if (Vars.CurrentConf.ReadInQueue)
            {
                Bridge.ALog("正在添加下列文件到播放列表: " + fileName);
                readerList.Add(new AudioFileReader(fileName));
            }
            else
            {
                Bridge.ALog("正在直接播放: " + fileName);
                var thread = new Thread(() =>
                {
                    var waveOut = new WaveOutEvent();
                    var reader = new AudioFileReader(fileName);
                    waveOut.Init(reader);
                    waveOut.Volume = ((float)Vars.CurrentConf.TTSVolume) / 100;
                    Bridge.ALog("音量设置为: " + waveOut.Volume);
                    waveOut.Play();
                    Vars.TotalPlayed++;
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
