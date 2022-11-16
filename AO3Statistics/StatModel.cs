namespace AO3Statistics;

/// <summary>
/// Data class representing work statistics.
/// </summary>
public class StatModel
{
    /// <summary>
    /// Date and time of sampling.
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Number of hits on the fic.
    /// </summary>
    public int Hits { get; set; }

    /// <summary>
    /// Number of kudos on the fic.
    /// </summary>
    public int Kudos { get; set; }

    /// <summary>
    /// Word count of published chapters.
    /// </summary>
    public int Words { get; set; }

    /// <summary>
    /// Number of published chapters.
    /// </summary>
    public int Chapters { get; set; }

    /// <summary>
    /// Serializes the instance to a CSV format string./>
    /// </summary>
    /// <returns>CSV formatted string.</returns>
    public override string ToString()
        => String.Join(',', Date.ToString("O"), Kudos, Words, Chapters, Hits);

    /// <summary>
    /// Produces a formatted string representation.
    /// </summary>
    /// <param name="_">Value isn't relevant</param>
    /// <returns>
    /// A string formatted like:<br/>
    /// <example>
    /// <code>
    /// Date:     <see cref="Date"/><br/>
    /// Hits:     <see cref="Hits"/><br/>
    /// Kudos:    <see cref="Kudos"/><br/>
    /// Words:    <see cref="Words"/><br/>
    /// Chapters: <see cref="Chapters"/>
    /// </code>
    /// </example>
    /// </returns>
    public string ToString(bool _)
    {
        return $"""
                Date:     {Date}
                Hits:     {Hits}
                Kudos:    {Kudos}
                Words:    {Words}
                Chapters: {Chapters}
                """;
    }

    /// <summary>
    /// Deserializes a CSV formatted string into a <see cref="StatModel"/>.
    /// </summary>
    /// <param name="s">String to deserialize.</param>
    /// <returns>A new <see cref="StatModel"/> filled with values from the string.</returns>
    public static StatModel FromString(string s)
    {
        int propCount = typeof(StatModel).GetProperties().Length;

        string[] elements = s.Split(',');
        if (elements.Length != propCount)
        {
            throw new StatModelException(elements, $"String contained more than ({propCount}). Likely malformed CSV.");
        }

        return new StatModel()
        {
            Date = DateTime.ParseExact(elements[0], "O", null),
            Hits = int.Parse(elements[1]),
            Kudos = int.Parse(elements[2]),
            Words = int.Parse(elements[3]),
            Chapters = int.Parse(elements[4]),
        };
    }
}