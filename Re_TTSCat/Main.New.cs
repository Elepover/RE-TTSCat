using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;
using System;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public static bool IsNAudioReady = false;
        public static bool IsEnabled = false;
        public Main()
        {
            PluginAuth = "Elepover";
            PluginCont = "elepover@outlook.com";
            PluginDesc = "直接读出你收到的弹幕和礼物！";
            PluginName = "Re: TTSCat";
            PluginVer = Data.Vars.CurrentVersion.ToString();
            Connected += OnConnected;
            Disconnected += OnDisconnected;
            ReceivedRoomCount += OnReceivedRoomCount;
            ReceivedDanmaku += OnReceivedDanmaku;
            AppDomain.CurrentDomain.UnhandledException += GlobalErrorHandler;
            Vars.Debouncer.GiftDebouncedEvent += GiftDebouncedEvent;
        }
    }
}
