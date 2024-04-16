namespace svp2lab_Converter
{
    public class Svp
    {
        public float BPM;
        public List<SvpNote> Notes;
        public float LengthSec;

        public Svp() { }

        public void FillPhoneme()
        {
            Notes.ForEach(x =>
            {
                if (string.IsNullOrWhiteSpace(x.Phonemes)) x.Phonemes = x.Lyric.Replace(".", "");
            });
            for (int i = 1; i < Notes.Count; i++)
            {
                if (Notes[i].Phonemes == "-")
                {
                    Notes[i].Phonemes = Notes[i - 1].Phonemes.Trim().Split(" ").Last();
                }
            }
        }
    }
    
    public class SvpNote
    {
        public long Onset;
        public float OnsetSec;
        public string Lyric;
        public string Phonemes;
        public float Tone;

        public SvpNote() { }
    }
}
