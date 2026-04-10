using Anything.Core.Models;
using Anything.Core.Services;
using Anything.Platform.Posix;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Anything POSIX CLI");
            Console.WriteLine("Usage: anything <query>");
            return 1;
        }

        string query = string.Join(" ", args);

        var provider = new PosixFileIndexProvider("/");
        var service = new AnythingSearchService(provider);

        Console.WriteLine("Indexing filesystem...");
        await service.BuildIndexAsync();

        Console.WriteLine($"Searching: {query}");
        Console.WriteLine();

        var results = await service.SearchAsync(query);

        int count = 0;
        foreach (FileEntry entry in results)
        {
            Console.WriteLine(entry.Name);
            Console.WriteLine("  " + entry.Path);
            Console.WriteLine();
            count++;
        }

        if (count == 0)
            Console.WriteLine("No results.");

        return 0;
    }
}
