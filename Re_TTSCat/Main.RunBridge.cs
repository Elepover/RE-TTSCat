using System.Threading;

namespace Re_TTSCat
{
    public partial class Main
    {
        public void RunBridge()
        {
            Data.Vars.MainBridge = new Thread(() => BridgeSyncer())
            {
                IsBackground = true
            };
            Data.Vars.MainBridge.Start();
        }
    }
}
