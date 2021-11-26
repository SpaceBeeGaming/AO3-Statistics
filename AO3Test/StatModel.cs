namespace AO3Test;

public class StatModel
{
    public int Hits { get; set; }
    public int Kudos { get; set; }
    public int Words { get; set; }
    public int Chapters { get; set; }
    public DateTime Time { get; set; }

    public override string ToString() 
        => String.Join(',', Time.ToString("O"), Kudos, Words, Chapters, Hits);
}