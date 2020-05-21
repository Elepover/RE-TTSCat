using System;
using System.IO;
using System.Threading;
using NAudio.Wave;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public static partial class TTSPlayer
    {
        public static void PlayerThread()
        {
            Bridge.ALog("播放器已启动");
            while (!Vars.CallPlayerStop)
            {
                if (fileList.Count != 0)
                {
                    Bridge.ALog("启动播放，剩余数目:" + fileList.Count);
                    var fileName = fileList[0];
                    try
                    {
                        using (var reader = new AudioFileReader(fileList[0]))
                            Play(reader);
                        Vars.TotalPlayed++;
                        if (Vars.CurrentConf.DoNotKeepCache)
                            File.Delete(fileName);
                    }
                    catch (Exception ex)
                    {
                        Bridge.ALog($"无法读取文件 {Path.GetFileName(fileName)}, 放弃: {ex.Message}");
                    }
                    if (fileList.Count > 0) fileList.RemoveAt(0);
                }
                Thread.Sleep(100);
            }
            Bridge.ALog("播放器已停止");
            Vars.CallPlayerStop = false;
        }
    }
}
