using System.Threading;

namespace Re_TTSCat
{
    public partial class Main
    {
        public void BridgeSyncer()
        {
            ALog("数据桥已启动");
            while (true)
            {
                while (Data.Bridge.PendingLogs.Count == 0)
                {
                    if (Data.Vars.CallBridgeStop) { goto Exit; }
                    Thread.Sleep(250);
                }
                while (Data.Bridge.PendingLogs.Count != 0)
                {
                    Log(Data.Bridge.PendingLogs[0]);
                    Data.Bridge.PendingLogs.RemoveAt(0);
                }
            }
Exit:
            Data.Vars.CallBridgeStop = false;
            ALog("数据桥已停止");
        }
    }
}
