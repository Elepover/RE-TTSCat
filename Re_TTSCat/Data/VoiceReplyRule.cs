using System.ComponentModel;

namespace Re_TTSCat.Data
{
    public partial class VoiceReplyRule : INotifyPropertyChanged
    {
        public enum MatchMode
        {
            Contains = 0,
            Regex = 1,
            Exact = 2
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
        public int MatchingMode { get => _matchingMode; set { _matchingMode = value; NotifyPropertyChanged("MatchingMode"); } }
        public int ReplyingMode { get => _replyingMode; set { _replyingMode = value; NotifyPropertyChanged("ReplyingMode"); } }
        public string ReplyContent { get => _replyContent; set { _replyContent = value; NotifyPropertyChanged("ReplyContent"); } }
        private bool _isTurnedOn;
        private string _keyword;
        private int _matchingMode;
        private int _replyingMode;
        private string _replyContent;

        public VoiceReplyRule()
        {
            _isTurnedOn = true;
            _keyword = string.Empty;
            _matchingMode = (int)MatchMode.Contains;
            _replyingMode = (int)ReplyMode.VoiceGeneration;
            _replyContent = string.Empty;
        }
    }
}
