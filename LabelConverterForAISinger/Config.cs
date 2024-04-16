namespace svp2lab_Converter
{
    public class LengthConfig
    {
        public string Key;
        public float Length;

        public LengthConfig() { }
        public LengthConfig(string key, float length)
        {
            Key = key;
            Length = length / 1000;
        }

        public static List<LengthConfig> LoadFile(string path)
        {
            if (!File.Exists(path))
            {
                return null;
            }
            var list = new List<LengthConfig>();
            try
            {
                var lines = File.ReadAllLines(path);
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        var split = line.Split("\t");
                        var len = new LengthConfig(split[0], float.Parse(split[1]));
                        list.Add(len);
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
}
