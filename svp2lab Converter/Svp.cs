using System.Text.RegularExpressions;

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

        public string GetLength(int i)
        {
            int tick;
            if (i < Notes.Count - 1)
            {
                var end = (int)decimal.Round(Notes[i + 1].Onset / 1470000 / 10) * 10; // 10Tickでクオンタイズ
                tick = end - Notes[i].OnsetTick;
                Notes[i + 1].OnsetTick = Notes[i].OnsetTick + tick;
            }
            else
            {
                tick = 480;
            }
            return $"Length={tick}";
        }

        public string GetLyric(int i)
        {
            var lyric = Notes[i].Phonemes;
            lyric = Regex.Replace(lyric, "^(pau|SP)$", "R");
            return $"Lyric={lyric}";
        }

        public string GetTone(int i)
        {
            int tone = (int)Math.Round(Notes[i].Tone, MidpointRounding.AwayFromZero);
            return $"NoteNum={tone}";
        }
    }
    
    public class SvpNote
    {
        public long Onset;
        public float OnsetSec;
        public string Lyric;
        public string Phonemes;
        public float Tone;
        public int OnsetTick;

        public SvpNote() { }
    }
}
