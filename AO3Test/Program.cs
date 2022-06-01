using AO3Statistics;

namespace AO3Test;

internal class Program
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

        var filepath = args[1];

        if ((args.Length is not 3 || args[2] is not "-m") && IsCurrent(filepath))
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
            navigator = new(uri, Navigator.PathMultiChapter);
        }
        catch (NavigatorException ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine($"Path used: \"{ex.Path}\"");
            
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
                navigator = new(uri, Navigator.PathSingleChapter);
            }
            catch (NavigatorException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine($"Path used: \"{ex.Path}\"");
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

        if (hits.IsSuccess & kudos.IsSuccess & words.IsSuccess & chapters.IsSuccess)
        {
            StatModel stats = new()
            {
                Hits = hits.value,
                Kudos = kudos.value,
                Words = words.value,
                Chapters = chapters.value,
                Date = DateTime.Now
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

    private static void SaveStatsToFile(StatModel stats, string path)
    {
        Console.WriteLine($"Saving Data to: \"{path}\"");
        File.AppendAllText(path, $"{stats}\n");
    }

    private static bool IsCurrent(string path) =>
        File.Exists(path) && StatModel.FromString(File.ReadLines(path).Last()).Date.Date >= DateTime.Now.Date;

}
