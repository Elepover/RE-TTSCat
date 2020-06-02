namespace Re_TTSCat
{
    public static partial class TTSPlayer
    {
        public class TTSEntry
        {
            public string Filename { get; set; }
            public bool DoNotDelete { get; set; }
            public TTSEntry(string filename = "", bool doNotDelete = false)
            {
                Filename = filename;
                DoNotDelete = doNotDelete;
            }
        }
    }
}
