using BilibiliDM_PluginFramework;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Re_TTSCat.Data
{
    public partial class VoiceReplyRule : INotifyPropertyChanged
    {
        public bool Matches(DanmakuModel dm)
        {
            try
            {
                if (!IsTurnedOn) return false;
                bool status;
                string content;
                switch ((MatchSource)MatchingSource)
                {
                    case MatchSource.DanmakuContent:
                        if (!(dm.MsgType == MsgTypeEnum.Comment || dm.MsgType != MsgTypeEnum.SuperChat)) return false;
                        content = dm.CommentText;
                        break;
                    case MatchSource.DanmakuUser:
                        if (!(dm.MsgType == MsgTypeEnum.Comment || dm.MsgType != MsgTypeEnum.SuperChat)) return false;
                        content = dm.UserName;
                        break;
                    case MatchSource.GiftName:
                        if (dm.MsgType != MsgTypeEnum.GiftSend) return false;
                        content = dm.GiftName;
                        break;
                    case MatchSource.GiftUser:
                        if (dm.MsgType != MsgTypeEnum.GiftSend) return false;
                        content = dm.UserName;
                        break;
                    default: return false;
                }
                switch ((MatchMode)MatchingMode)
                {
                    case MatchMode.Contains:
                        status = content.Contains(Keyword);
                        break;
                    case MatchMode.Regex:
                        try
                        {
                            status = Regex.IsMatch(content, Keyword, RegexOptions.None);
                        }
                        catch (Exception ex)
                        {
                            Bridge.ALog($"正则表达式匹配失败: {ex.Message}");
                            status = false;
                        }
                        break;
                    case MatchMode.Exact:
                        status = content == Keyword;
                        break;
                    case MatchMode.Wildcard:
                        status = Conf.CheckWildcard(Keyword, content);
                        break;
                    default:
                        // how can you come here?
                        status = false;
                        break;
                }
                return status;
            }
            catch (Exception ex)
            {
                Bridge.ALog($"规则匹配失败: {ex.Message}, 关键字: {Keyword}");
                return false;
            }
        }
    }
}
