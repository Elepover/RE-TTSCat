namespace Re_TTSCat.Data
{
    public partial class Bridge
    {
        public static void ALog(string content)
        {
            if (Vars.CurrentConf.DebugMode) PendingLogs.Add(content);
        }
    }
}
