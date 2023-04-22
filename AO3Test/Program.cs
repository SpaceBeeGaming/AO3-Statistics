using AO3Statistics;

namespace AO3Test;

internal static class Program
{
    private const string Website = "archiveofourown.org";
    private static int Main(string[] args)
    {
        if (args.Length is not (2 or 3))
        {
            Console.WriteLine("Number of arguments doesn't match.");
            Console.WriteLine("Please provide:");
            Console.WriteLine("1) A web address pointing to a fic on AO3.");
            Console.WriteLine("2) A path where .csv file is to be saved.");
            Console.WriteLine("3) Optional switch '-m' that will ignore a check for existing entry from current date.");

            return 1;
        }

        string filePath = args[1];

        if ((args.Length is not 3 || args[2] is not "-m") && IsCurrent(filePath))
        {
            Console.WriteLine("Entry already exists for this date, skipping...");
            Console.WriteLine("Add '-m' as a third argument to ignore this check.");
            return 2;
        }

        Uri uri = new(args[0]);
        if (uri.Host is not Website)
        {
            Console.WriteLine($"This program only works with {Website}.");
            return 1;
        }

        //HtmlDocument document = new HtmlWeb().Load(uri);

        //var document = new HtmlDocument();
        //document.Load(@"C:\Users\janne\Desktop\html.html");

        //var node = Navigator.NavigateToNode(document);

        Navigator? navigator = null;
        try
        {
            navigator = new(uri, Navigator.XPathMultiChapter);
        }
        catch (NavigatorException ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine($"XPath used: \"{ex.XPath}\"");

            if (ex.IsRecoverable is null or false)
            {
#if DEBUG
                throw;
#else
            return 1;
#endif
            }
        }

        if (navigator is null)
        {
            try
            {
                navigator = new(uri, Navigator.XPathSingleChapter);
            }
            catch (NavigatorException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"XPath used: \"{ex.XPath}\"");
#if DEBUG
                throw;
#else
            return 1;
#endif
            }
        }

        var hits = navigator.GetValue(StatTypes.Hits);
        var kudos = navigator.GetValue(StatTypes.Kudos);
        var words = navigator.GetValue(StatTypes.Words);
        var chapters = navigator.GetValue(StatTypes.Chapters);
        Dictionary<StatTypes, (bool IsSuccess, int value)> stats = new()
        {
            { StatTypes.Hits, hits },
            { StatTypes.Kudos, kudos },
            { StatTypes.Words, words },
            { StatTypes.Chapters, chapters }
        };

        if (stats.All(x => x.Value.IsSuccess))
        {
            StatModel statModel = new()
            {
                Hits = hits.value,
                Kudos = kudos.value,
                Words = words.value,
                Chapters = chapters.value,
                Date = DateTime.Now
            };

            Console.WriteLine(statModel.ToString(true));
            SaveStatsToFile(statModel, filePath);
            return 0;
        }
        else
        {
            foreach (var stat in stats.Where(x => x.Value.IsSuccess is not true))
            {
                Console.WriteLine($"Error Parsing Data: {stat.Key}");
            }
            return 1;
        }
    }

    private static void SaveStatsToFile(StatModel stats, string path)
    {
        Console.WriteLine($"Saving Data to: \"{path}\"");
        File.AppendAllText(path, $"{stats}\n");
    }

    private static bool IsCurrent(string path) =>
        File.Exists(path) && StatModel.FromString(File.ReadLines(path).Last()).Date.Date >= DateTime.Now.Date;

}
