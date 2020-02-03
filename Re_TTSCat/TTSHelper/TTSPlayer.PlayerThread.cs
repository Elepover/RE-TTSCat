using System.IO;
using System.Threading;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public static partial class TTSPlayer
    {
        public static void PlayerThread()
        {
            Bridge.Log("播放器已启动");
            while (!Vars.CallPlayerStop)
            {
                if (readerList.Count != 0)
                {
                    if (Vars.CurrentConf.DebugMode)
                    {
                        Bridge.Log("启动播放，剩余数目:" + readerList.Count);
                    }
                    var fileName = readerList[0].FileName;
                    Play(readerList[0]);
                    Vars.TotalPlayed++;
                    if (Vars.CurrentConf.DoNotKeepCache)
                    {
                        File.Delete(fileName);
                    }
                    if (readerList.Count > 0) readerList.RemoveAt(0);
                }
                Thread.Sleep(100);
            }
            Bridge.Log("播放器已停止");
            Vars.CallPlayerStop = false;
        }
    }
}
