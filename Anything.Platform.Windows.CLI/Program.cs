using Anything.Core.Models;
using Anything.Core.Services;
using Anything.Platform.Windows;

internal class Program
{
    static async Task<int> Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Anything CLI");
            Console.WriteLine("Использование: anything <query>");
            return 1;
        }

        string query = string.Join(" ", args);

        // Платформенный провайдер
        var provider = new WindowsFileIndexProvider();

        // Основной сервис поиска
        var service = new AnythingSearchService(provider);

        Console.WriteLine("Индексация файлов...");
        await service.BuildIndexAsync();

        Console.WriteLine($"Поиск: {query}");
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
            Console.WriteLine("Ничего не найдено.");

        return 0;
    }
}
