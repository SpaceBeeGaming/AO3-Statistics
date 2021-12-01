namespace AO3Statistics;

public class StatModel
{
    public DateTime DateTime { get; set; }
    public int Hits { get; set; }
    public int Kudos { get; set; }
    public int Words { get; set; }
    public int Chapters { get; set; }

    public override string ToString()
        => String.Join(',', DateTime.ToString("O"), Kudos, Words, Chapters, Hits);

    public string ToString(bool readable) =>
        $"Date: {DateTime}\nHits: {Hits}\nKudos: {Kudos}\nWords: {Words}\nChapters: {Chapters}";
    
    public static StatModel FromString(string s)
    {
        var components = s.Split(',');

        return new StatModel()
        {
            DateTime = DateTime.ParseExact(components[0], "O", null),
            Hits = int.Parse(components[1]),
            Kudos = int.Parse(components[2]),
            Words = int.Parse(components[3]),
            Chapters = int.Parse(components[4]),
        };
    }
}