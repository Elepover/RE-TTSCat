using System.IO;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading;
using System.Web;
using System.Windows.Threading;

namespace Re_TTSCat
{
    public static class FrameworkTTS
    {
        public static string Download(string content)
        {
            var fileName = Path.Combine(Data.Vars.CacheDir, Data.Conf.GetRandomFileName() + "MSFT.wav");
            var frame = new DispatcherFrame();
            var thread = new Thread(() =>
            {
                using (var synth = new SpeechSynthesizer()
                {
                    Rate = Data.Vars.CurrentConf.ReadSpeed
                })
                {
                    synth.SetOutputToWaveFile(fileName);
                    Data.Bridge.ALog("(E1) 正在生成 TTS, 文件名: " + fileName);
                    var voices = synth.GetInstalledVoices().Where(x => x.Enabled);
                    var targetVoice = voices.FirstOrDefault(x => x.VoiceInfo.Name == Data.Vars.CurrentConf.VoiceName);
                    if (targetVoice == default) Data.Bridge.ALog($"(E1) 错误：选中的语音包 {Data.Vars.CurrentConf.VoiceName} 不可用，将忽略语音选择");
                    else synth.SelectVoice(targetVoice.VoiceInfo.Name);
                    synth.Speak(HttpUtility.UrlDecode(content));
                }
                frame.Continue = false;
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            Dispatcher.PushFrame(frame);
            return fileName;
        }
    }
}
