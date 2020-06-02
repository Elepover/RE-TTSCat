using Re_TTSCat.Data;
using System;
using System.Threading.Tasks;

namespace Re_TTSCat
{
    public static partial class TTSPlayer
    {
        public static async Task PlayVoiceReply(string danmaku, string user)
        {
            if (!Vars.CurrentConf.EnableVoiceReply) return;
            // danmaku blocking rules have been processed, just process what's left for us
            // go through all rules to see if there's a match
            // (this is the master cycle)
            foreach (var rule in Vars.CurrentConf.VoiceReplyRules)
            {
                if (rule.Matches(danmaku))
                {
                    if ((VoiceReplyRule.ReplyMode)rule.ReplyingMode != VoiceReplyRule.ReplyMode.VoiceGeneration)
                    {
                        // play specific file:
                        try
                        {
                            if (Vars.CurrentConf.InstantVoiceReply)
                            {
                                Play(rule.ReplyContent, false, true);
                            }
                            else
                            {
                                // add the file to queue
                                fileList.Add(new TTSEntry(rule.ReplyContent, true));
                            }
                        }
                        catch (Exception ex)
                        {
                            Bridge.ALog($"无法读出语音答复: {ex.Message}");
                        }
                    }
                    else
                    {
                        // play by voice generation (and by default)
                        await UnifiedPlay(
                            rule.ReplyContent.Replace("$USER", user),
                            true,
                            Vars.CurrentConf.InstantVoiceReply
                        );
                    }
                }
            }
        }
    }
}
