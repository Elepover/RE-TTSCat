using System;

namespace Re_TTSCat
{
    public partial class KruinUpdates
    {
        public partial class Update
        {
            public Update(Version latestVer, DateTime updTime, string updDesc, string dlLink)
            {
                _latestVersion = latestVer;
                _updateTime = updTime;
                _updateDescription = updDesc;
                _downloadLink = dlLink;
            }

            public Version LatestVersion => _latestVersion;
            public DateTime UpdateTime => _updateTime;
            public string UpdateDescription => _updateDescription;
            public string DownloadLink => _downloadLink;
            private Version _latestVersion { get; }
            private DateTime _updateTime { get; }
            private string _updateDescription { get; }
            private string _downloadLink { get; }
        }
    }
}
