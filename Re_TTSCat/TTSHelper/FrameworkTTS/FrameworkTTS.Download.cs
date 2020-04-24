using System.IO;
using System.Speech.Synthesis;
using System.Threading;
using System.Windows.Threading;

namespace Re_TTSCat
{
    public static class FrameworkTTS
    {
        public static string Download(string content)
        {
            var fileName = Path.Combine(Data.Vars.CacheDir, Data.Conf.GetRandomFileName() + "MSFT.wav");
            var frame = new DispatcherFrame();
            var thread = new Thread(() => {
                var synth = new SpeechSynthesizer() { Rate = Data.Vars.CurrentConf.ReadSpeed } ;
                synth.SetOutputToWaveFile(fileName);
                Data.Bridge.ALog("(E1) 正在生成 TTS, 文件名: " + fileName);
                synth.Speak(content);
                synth.Dispose();
                frame.Continue = false;
            });
            thread.Start();
            Dispatcher.PushFrame(frame);
            return fileName;
        }
    }
}
