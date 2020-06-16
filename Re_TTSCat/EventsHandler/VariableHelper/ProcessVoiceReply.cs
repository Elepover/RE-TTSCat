using BilibiliDM_PluginFramework;
using Re_TTSCat.Data;

namespace Re_TTSCat
{
    public partial class Main : DMPlugin
    {
        public static string ProcessVoiceReply(DanmakuModel e, VoiceReplyRule rule)
        {
            // there's no need to check if the types match, we've already checked for it
            switch ((VoiceReplyRule.MatchSource)rule.MatchingSource)
            {
                case VoiceReplyRule.MatchSource.DanmakuContent:
                    // available: $USER (including $$ and $!), $DM
                    return ProcessDanmaku(e, rule.ReplyContent);
                case VoiceReplyRule.MatchSource.DanmakuUser:
                    // available: $USER (including $$ and $!), $DM
                    return ProcessDanmaku(e, rule.ReplyContent);
                case VoiceReplyRule.MatchSource.GiftName:
                    // available: $USER (including $$ and $!), $GIFT and $COUNT
                    return ProcessGift(e, rule.ReplyContent);
                case VoiceReplyRule.MatchSource.GiftUser:
                    // available: $USER (including $$ and $!), $GIFT and $COUNT
                    return ProcessGift(e, rule.ReplyContent);
                default: return rule.ReplyContent;
            }
        }
    }
}
