namespace Re_TTSCat.Data
{
    public partial class Conf
    {
        public static bool CheckKeywordEligibility(string content)
        {
            switch (Vars.CurrentConf.KeywordBlockMode)
            {
                default:
                    return true;
                case 1:
                    foreach (string keyword in Vars.CurrentConf.KeywordBlackList.Split('\n'))
                    {
                        if (content.Contains(keyword)) { return false; }
                    }
                    return true;
                case 2:
                    foreach (string keyword in Vars.CurrentConf.KeywordWhiteList.Split('\n'))
                    {
                        if (content.Contains(keyword)) { return true; }
                    }
                    return false;
            }
        }
    }
}
