using System.IO;
using System.Threading;
using System.Windows.Threading;
using NAudio.Wave;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public static partial class TTSPlayer
    {
        public static void Play(string filename, bool wait = true)
        {
            var frame = new DispatcherFrame();
            var thread = new Thread(() =>
            {
                using (var reader = new AudioFileReader(filename))
                {
                    using (var waveOut = new WaveOutEvent())
                    {
                        waveOut.Init(reader);
                        waveOut.Volume = ((float)Vars.CurrentConf.TTSVolume) / 100;
                        Bridge.ALog($"音量设置为: {waveOut.Volume}");
                        waveOut.Play();
                        Vars.TotalPlayed++;
                        if (!wait)
                            frame.Continue = false;
                        while (waveOut.PlaybackState != PlaybackState.Stopped) { Thread.Sleep(50); }
                        if (Vars.CurrentConf.DoNotKeepCache)
                            File.Delete(filename);
                    }
                    frame.Continue = false;
                }
            });
            thread.Start();
            Dispatcher.PushFrame(frame);
        }
    }
}
