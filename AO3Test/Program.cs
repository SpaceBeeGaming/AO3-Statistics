using HtmlAgilityPack;

namespace AO3Test;

internal class Program
{
    private const string Website = "archiveofourown.org";
    private static int Main(string[] args)
    {
        if (args.Length is not 2)
        {
            Console.WriteLine("Number of arguments doesnt match.");
            Console.WriteLine("Please provide:");
            Console.WriteLine("1) An web andress pointing to a fic on AO3.");
            Console.WriteLine("2) A path where .csv file is to be saved.");

            return 1;
        }

        Uri uri = new(args[0]);
        if (uri.Host is not Website)
        {
            Console.WriteLine($"This program only works with {Website}.");
            return 1;
        }

        HtmlDocument document = new HtmlWeb().Load(uri);
        var filepath = args[1];

        //var document = new HtmlDocument();
        //document.Load(@"C:\Users\janne\Desktop\html.html");

        var node = NavigateToNode(document);

        if (node is null)
        {
            Console.WriteLine("Failed to Navigate Node tree. (AO3 could have changed page format.)");
            return 1;
        }

        var hits = ConvertToInt(ExtractValue(node, "hits"));
        var kudos = ConvertToInt(ExtractValue(node, "kudos"));
        var words = ConvertToInt(ExtractValue(node, "words"));
        var chapters = ConvertToInt(ExtractValue(node, "chapters").Split('/')[0]);

        if (hits.success & kudos.success & words.success & chapters.success)
        {
            StatModel stats = new()
            {
                Hits = hits.result,
                Kudos = kudos.result,
                Words = words.result,
                Chapters = chapters.result,
                Time = DateTime.Now
            };

            Console.WriteLine(stats);

            SaveStatsToFile(stats, filepath);
            return 0;
        }
        else
        {
            Console.WriteLine("Error Parsong Data.");
            return 1;
        }
    }

    private static (bool success, int result) ConvertToInt(string value) => (Int32.TryParse(value, out int result), result);

    private static void SaveStatsToFile(StatModel stats, string path)
    {
        Console.WriteLine($"Saving Data to: \"{path}\"");
        File.AppendAllText(path, $"{stats}\n");
    }

    private static string ExtractValue(HtmlNode node, string name)
        => node.SelectSingleNode($"./dd[@class='{name}']").InnerText;

    private static HtmlNode? NavigateToNode(HtmlDocument document)
    {
        try
        {
            // The location within the Html document where the statistics are located.
            return document.DocumentNode.SelectSingleNode("//body")
                                            .SelectSingleNode("./div[@class='wrapper']") //outer
                                            .SelectSingleNode("./div[@class='wrapper']") //inner
                                            .SelectSingleNode("./div[@class='chapters-show region']") //main
                                            .SelectSingleNode("./div[@class='work']")
                                            .SelectSingleNode("./div[@class='wrapper']")
                                            .SelectSingleNode("./dl")
                                            .SelectSingleNode("./dd[@class='stats']")
                                            .SelectSingleNode("./dl");
        }
        catch (NullReferenceException)
        {
            return null;
        }
    }
}
