using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Re_TTSCat.Data
{
    public partial class VoiceReplyRule : INotifyPropertyChanged
    {
        public bool Matches(string danmaku)
        {
            if (!IsTurnedOn) return false;
            bool status;
            switch ((MatchMode)MatchingMode)
            {
                case MatchMode.Contains:
                    status = danmaku.Contains(Keyword);
                    break;
                case MatchMode.Regex:
                    try
                    {
                        status = Regex.IsMatch(danmaku, Keyword, RegexOptions.None);
                    }
                    catch (Exception ex)
                    {
                        Bridge.ALog($"正则表达式匹配失败: {ex.Message}");
                        status = false;
                    }
                    break;
                case MatchMode.Exact:
                    status = danmaku == Keyword;
                    break;
                default:
                    // how can you come here?
                    status = false;
                    break;
            }
            return status;
        }
    }
}
