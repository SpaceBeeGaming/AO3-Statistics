using AO3Statistics;

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

        //HtmlDocument document = new HtmlWeb().Load(uri);
        var filepath = args[1];

        //var document = new HtmlDocument();
        //document.Load(@"C:\Users\janne\Desktop\html.html");

        //var node = Navigator.NavigateToNode(document);

        AO3Statistics.Navigator navigator;
        try
        {
            navigator = new(uri);
        }
        catch (NavigatorException ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine($"Path used: \"{ex.Path}\"");
            if (ex.InnerException is not null)
            {
                Console.WriteLine($"Inner Exception:\n{ex.InnerException}");
            }
#if DEBUG
            throw;
#else
            return 1;
#endif
        }

        var hits = ConvertToInt(navigator.GetValue(StatTypes.Hits));
        var kudos = ConvertToInt(navigator.GetValue(StatTypes.Kudos));
        var words = ConvertToInt(navigator.GetValue(StatTypes.Words));
        var chapters = ConvertToInt(navigator.GetValue(StatTypes.Chapters).Split('/')[0]);

        if (hits.IsSuccess & kudos.IsSuccess & words.IsSuccess & chapters.IsSuccess)
        {
            StatModel stats = new()
            {
                Hits = hits.value,
                Kudos = kudos.value,
                Words = words.value,
                Chapters = chapters.value,
                Time = DateTime.Now
            };

            Console.WriteLine(stats.ToString(true));

            SaveStatsToFile(stats, filepath);
            return 0;
        }
        else
        {
            Console.WriteLine("Error Parsing Data.");
            return 1;
        }
    }

    private static (bool IsSuccess, int value) ConvertToInt(string? value) => (Int32.TryParse(value, out int result), result);

    private static void SaveStatsToFile(StatModel stats, string path)
    {
        Console.WriteLine($"Saving Data to: \"{path}\"");
        File.AppendAllText(path, $"{stats}\n");
    }

}
