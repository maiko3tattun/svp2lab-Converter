namespace svp2lab_Converter
{
    public class Replace
    {
        public string before;
        public List<string> after = new List<string>();

        public Replace() { }
        public Replace(string before, List<string> after)
        {
            this.before = before;
            this.after = after;
        }

        public static List<Replace> LoadFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            var list = new List<Replace>();
            try
            {
                var lines = File.ReadAllLines(path);
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var split = line.Split("\t");
                        var rep = new Replace(split[0], split[1].Split(" ").ToList());
                        list.Add(rep);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }

    public class Dsdict
    {
        public List<DsEntrie> entries;

        public Dsdict() { }
    }

    public class DsEntrie
    {
        public string grapheme;
        public List<string> phonemes;

        public DsEntrie() { }
    }
}
