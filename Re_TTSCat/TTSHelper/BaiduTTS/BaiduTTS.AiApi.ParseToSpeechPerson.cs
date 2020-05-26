namespace Re_TTSCat
{
    public static partial class BaiduTTS
    {
        public static partial class AiApi
        {
            public static SpeechPerson ParseToSpeechPerson(int index)
            {
                switch (index)
                {
                    case 0: return SpeechPerson.DuXiaoMei;
                    case 1: return SpeechPerson.DuXiaoYu;
                    case 2: return SpeechPerson.DuXiaoYao;
                    case 3: return SpeechPerson.DuYaYa;
                    case 4: return SpeechPerson.DuXiaoJiao;
                    case 5: return SpeechPerson.DuMiDuo;
                    case 6: return SpeechPerson.DuBoWen;
                    case 7: return SpeechPerson.DuXiaoTong;
                    case 8: return SpeechPerson.DuXiaoMeng;
                    default: return SpeechPerson.DuXiaoMei;
                }
            }
        }
    }
}
