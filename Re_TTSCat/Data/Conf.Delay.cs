using System;
using System.Threading;
using System.Windows.Threading;

namespace Re_TTSCat.Data
{
    public partial class Conf
    {
        public static void Delay(int interval)
        {
            var frame = new DispatcherFrame();
            var worker = new Thread(() => {
                Thread.Sleep(interval);
                frame.Continue = false;
            });
            worker.Start();
            Dispatcher.PushFrame(frame);
        }
    }
}
