namespace svp2lab_Converter
{
    public class Lab
    {
        public string Phoneme;
        public long Start; // 1/10,000,000 s
        public long End;

        public Lab(string phoneme, float start, float end)
        {
            this.Phoneme = phoneme;
            this.Start = (long)Math.Round(start * 10000000);
            this.End = (long)Math.Round(end * 10000000);
        }
        public override string ToString()
        {
            return $"{Start} {End} {Phoneme}";
        }
    }

    public class Phoneme
    {
        public string phoneme;
        public float Start;
        public float MaxLength;
        public float End;

        public Phoneme(string phoneme)
        {
            this.phoneme = phoneme;
        }
    }
}
