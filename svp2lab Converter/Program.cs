// See https://aka.ms/new-console-template for more information
// dotnet publish -c Release -r win-x64 --self-contained false
// dotnet publish -c Release -r osx-x64 --self-contained false
using svp2lab_Converter;
using System.Text;

while (true)
{
    Console.WriteLine("Hello Label Converter!");
    Console.WriteLine("");
    Console.WriteLine("これは何？：svpをlabにしたりustにしたりするよ");
    Console.WriteLine("What is this? : make lab or ust from svp");
    Console.WriteLine("");
    Console.WriteLine("Paste from \"Copy Notes for Label Converter\" script:");

    var list = new List<string>();
    var input = new StringBuilder();

    while(true)
    {
        var line = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(line))
        {
            break;
        }
        input.AppendLine(line);
        list.Add(line);
    }
    Svp svp = YamlLoadUtil.Load<Svp>(input.ToString());
    if(svp == null)
    {
        Console.WriteLine("貼るもの間違えてね？");
        Console.ReadLine();
        continue;
    }
    svp.FillPhoneme();

    var replace = new List<Replace>();
    /*
    dsdictを読もうとしてたけど子音のnが母音のNに変換されたりするのでやめた
    Console.WriteLine("dsdict.yaml path:");
    var dspath = Console.ReadLine().Replace("\"", "");
    Dsdict dsdict = YamlLoadUtil.LoadFile<Dsdict>(dspath);
    if (dsdict == null)
    {
        Console.WriteLine("dsdictなしで続行します　Continue without dsdict");
    }
    else
    {
        replace.AddRange(dsdict.entries.Select(e => new Replace(e.grapheme, e.phonemes)));
        Console.WriteLine("dsdict ok!");
    }*/

    Console.WriteLine("");
    Console.WriteLine("Replace Config Path:");
    var replacePath = Console.ReadLine().Replace("\"", "");
    var repList = Replace.LoadFile(replacePath);
    if (repList == null)
    {
        Console.WriteLine("replace configなしで続行します　Continue without replace config");
    }
    else
    {
        replace.AddRange(repList);
        Console.WriteLine("replace config ok!");
    }

    var length = new List<LengthConfig>();

    Console.WriteLine("");
    Console.WriteLine("Length Config Path:");
    var lengthPath = Console.ReadLine().Replace("\"", "");
    var lenList = LengthConfig.LoadFile(lengthPath);
    if (lenList == null)
    {
        Console.WriteLine("length configなしで続行します　Continue without length config");
    }
    else
    {
        length.AddRange(lenList);
        lenList = lenList.OrderByDescending(l => l.Key.Length).ToList();
        Console.WriteLine("length config ok!");
    }

    Console.WriteLine("");
    Console.WriteLine("Output");

    string path;
    while (true)
    {
        Console.WriteLine("Wav Path:");
        path = Console.ReadLine().Replace("\"", "");
        if (File.Exists(path) && Path.GetExtension(path) == ".wav")
        {
            break;
        }
        Console.WriteLine("それwavじゃないかも！");
    }
    var lab = path.Replace(".wav", ".lab");
    try
    {
        CreateLab(lab);
        Console.WriteLine("");
        Console.WriteLine("Output Succeed!");
        Console.WriteLine(lab);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }

    Console.ReadLine();
    Console.WriteLine("");
    Console.WriteLine("");


    void CreateLab(string path)
    {
        // ノート→音素
        var phonemes = new List<Phoneme>();
        foreach (var note in svp.Notes)
        {
            var notePhonemes = new List<Phoneme>();
            replace.ForEach(rep => note.Phonemes = note.Phonemes.Replace(rep.before, string.Join(" ", rep.after)));
            bool firstVow = false;

            foreach (var phoneme in note.Phonemes.Split(" "))
            {
                var p = new Phoneme(phoneme);
                var con = length.FirstOrDefault(l => l.Key == phoneme);
                if (con != null)
                {
                    p.MaxLength = con.Length;
                }
                else if(!firstVow)
                {
                    p.Start = note.OnsetSec;
                    firstVow = true;
                }
                notePhonemes.Add(p);
            }
            if (notePhonemes.All(p => p.Start == 0))
            {
                notePhonemes.First().Start = note.OnsetSec;
            }
            phonemes.AddRange(notePhonemes);
        }

        // 音素の長さ
        for (int i = 0; i < phonemes.Count;)
        {
            // 今のノートの音素＋次のノートの前につく子音　の数と長さ
            int phonemeCount = 1;
            float length = 0.5f;
            while (phonemes.Count > i + phonemeCount)
            {
                if (phonemes[i + phonemeCount].Start > 0)
                {
                    length = phonemes[i + phonemeCount].Start - phonemes[i].Start;
                    break;
                }
                phonemeCount++;
            }

            if (phonemeCount == 1)
            {
                phonemes[i].End = phonemes[i].Start + length;
            }
            else
            {
                var list = phonemes.Skip(i).Take(phonemeCount).ToList();

                if (list.All(p => p.MaxLength > 0)) // 全部子音
                {
                    var TotalMaxLength = list.Sum(p => p.MaxLength);
                    var start = phonemes[i].Start;
                    foreach (var p in list)
                    {
                        p.Start = start;
                        p.End = start + length / TotalMaxLength * p.MaxLength;
                        start += length / TotalMaxLength * p.MaxLength;
                    }
                }
                else 
                {
                    var skip = new List<Phoneme>();
                    while (true)
                    {
                        if (list.Any(p => !skip.Contains(p) && p.MaxLength > 0 && p.MaxLength < length / phonemeCount))
                        {
                            var cons = list.FindAll(p => !skip.Contains(p) && p.MaxLength > 0 && p.MaxLength < length / phonemeCount);
                            skip.AddRange(cons);
                            phonemeCount -= cons.Count;
                            length -= cons.Sum(p => p.MaxLength);
                        }
                        else
                        {
                            break;
                        }
                    }
                    float start = phonemes[i].Start;
                    foreach (var p in list)
                    {
                        p.Start = start;
                        if (skip.Contains(p))
                        {
                            p.End = start + p.MaxLength;
                        }
                        else
                        {
                            p.End = start + length / phonemeCount;
                        }
                        start = p.End;
                    }
                    phonemeCount = list.Count;
                }
            }
            i += phonemeCount;
        }
        if (phonemes.Last().Start < svp.LengthSec)
        {
            phonemes.Last().End = svp.LengthSec;
        }

        var labs = phonemes.Select(p => new Lab(p.phoneme, p.Start, p.End)).Select(l => l.ToString());
        File.WriteAllLines(path, labs);
    }
}