using System.ComponentModel;

namespace Re_TTSCat.Data
{
    public partial class VoiceReplyRule : INotifyPropertyChanged
    {
        public enum MatchSource
        {
            DanmakuContent = 0,
            DanmakuUser = 1,
            GiftName = 2,
            GiftUser = 3
        }

        public enum MatchMode
        {
            Contains = 0,
            Regex = 1,
            Exact = 2,
            Wildcard = 3
        }

        public enum ReplyMode
        {
            VoiceGeneration = 0,
            PrerecordedMessage = 1
        }

        public void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsTurnedOn { get => _isTurnedOn; set { _isTurnedOn = value; NotifyPropertyChanged("IsTurnedOn"); } }
        public string Keyword { get => _keyword; set { _keyword = value; NotifyPropertyChanged("Keyword"); } }
        public int MatchingSource { get => _matchingSource; set { _matchingSource = value; NotifyPropertyChanged("MatchingSource"); } }
        public int MatchingMode { get => _matchingMode; set { _matchingMode = value; NotifyPropertyChanged("MatchingMode"); } }
        public int ReplyingMode { get => _replyingMode; set { _replyingMode = value; NotifyPropertyChanged("ReplyingMode"); } }
        public string ReplyContent { get => _replyContent; set { _replyContent = value; NotifyPropertyChanged("ReplyContent"); } }
        private bool _isTurnedOn;
        private string _keyword;
        private int _matchingSource;
        private int _matchingMode;
        private int _replyingMode;
        private string _replyContent;

        public VoiceReplyRule()
        {
            _isTurnedOn = true;
            _keyword = string.Empty;
            _matchingSource = (int)MatchSource.DanmakuContent;
            _matchingMode = (int)MatchMode.Contains;
            _replyingMode = (int)ReplyMode.VoiceGeneration;
            _replyContent = string.Empty;
        }

        public bool IsVariablesGood()
        {
            switch ((MatchSource)MatchingSource)
            {
                case MatchSource.DanmakuContent:
                    return !(ReplyContent.Contains("$GIFT") || ReplyContent.Contains("$COUNT"));
                case MatchSource.DanmakuUser:
                    return !(ReplyContent.Contains("$GIFT") || ReplyContent.Contains("$COUNT"));
                case MatchSource.GiftName:
                    return !ReplyContent.Contains("$DM");
                case MatchSource.GiftUser:
                    return !ReplyContent.Contains("$DM");
                default: return false;
            }
        }
    }
}
