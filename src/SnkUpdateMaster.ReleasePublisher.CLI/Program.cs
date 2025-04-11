using SnkUpdateMaster.Core.ReleasePublisher;
using SnkUpdateMaster.SqlServer;
using SnkUpdateMaster.SqlServer.Configuration;
using SnkUpdateMaster.SqlServer.Database;

class Program
{
    private const string connectionString = "Data Source=localhost;Initial Catalog= SnkUpdateMasterDb;Integrated Security=True;Persist Security Info=False;Pooling=False;Multiple Active Result Sets=False;Connect Timeout=60;Encrypt=True;Trust Server Certificate=True;Command Timeout=0";

    private static readonly ISqlConnectionFactory _sqlConnectionFactory = new SqlConnectionFactory(connectionString);

    private static readonly IReleaseInfoSource _releaseInfoSource = new SqlServerReleaseInfoSource(_sqlConnectionFactory);

    private static readonly ReleaseManager _releaseManager = new ReleaseManagerBuilder()
        .WithZipPackager()
        .WithSqlServerReleaseSource(connectionString)
        .Build();

    private static CancellationTokenSource? _cts;

    static async Task Main(string[] args)
    {
        Console.WriteLine("SNK release publisher CLI");

        while (true)
        {
            Console.WriteLine("\nMain menu:");
            Console.WriteLine("1. Publish new release");
            Console.WriteLine("2. View published releases");
            Console.WriteLine("3. Exit");
            Console.WriteLine("Select an option: ");

            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    await HandlePublishNewRelease();
                    break;
                case "2":
                    await HandleViewPublishedReleases();
                    break;
                case "3":
                    return;
                default:
                    Console.WriteLine("Invalid option");
                    break;
            }
        }
    }

    private static async Task HandlePublishNewRelease()
    {
        try
        {
            Console.Write("Enter application directory path: ");
            var appDir = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(appDir))
            {
                Console.WriteLine("Application directory path is null or empty");
                return;
            }

            Console.Write("Enter version (format: major.minor.build): ");
            if (!Version.TryParse(Console.ReadLine(), out var version))
            {
                Console.WriteLine("Invalid version format");
                return;
            }

            _cts = new CancellationTokenSource();
            var progress = new Progress<double>(p => Console.Write($"\rPublish progress: {p:P0} "));

            Console.WriteLine("Starting publish... (Press 'C' to stop)");

            var publishTask = _releaseManager.PulishReleaseAsync(
                appDir,
                version,
                progress,
                _cts.Token);

            _ = Task.Run(() =>
            {
                if (Console.ReadKey(true).Key == ConsoleKey.C)
                {
                    _cts.Cancel();
                    Console.WriteLine("\nPublish stoped");
                }
            });

            var result = await publishTask;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Publish was canceled");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during publish: {ex.Message}");
        }
    }

    private static async Task HandleViewPublishedReleases()
    {
        try
        {
            int? currentPage = 1;
            int? pageSize = 20;

            while (true)
            {
                var pagedReleaseInfos = await _releaseInfoSource.GetReleaseInfosPagedAsync(
                    currentPage, pageSize);

                Console.WriteLine("\nReleases:");
                Console.WriteLine($"Page {pagedReleaseInfos.PageNumber} of {pagedReleaseInfos.TotalCount}");
                Console.WriteLine("---------------------------------");

                foreach (var releaseInfo in pagedReleaseInfos.Data)
                {
                    Console.WriteLine($"Version: {releaseInfo.Version}");
                    Console.WriteLine($"Published: {releaseInfo.ReleaseDate}");
                    Console.WriteLine("---------------------------------");
                }

                Console.WriteLine("\nNavigation");
                Console.WriteLine("N - Next page | P - Previous page | E - Exit to main menu");
                Console.WriteLine("Select action: ");

                var action = Console.ReadLine()?.Trim().ToUpper();

                switch (action)
                {
                    case "N":
                        if (currentPage < pagedReleaseInfos.TotalCount)
                        {
                            currentPage++;
                        }
                        else
                        {
                            Console.WriteLine("Already on last page");
                        }
                        break;
                    case "P":
                        if (currentPage > 1)
                        {
                            currentPage--;
                        }
                        else
                        {
                            Console.WriteLine("Already on first page");
                        }
                        break;
                    case "E":
                        return;
                    default:
                        Console.WriteLine("Invalid action");
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving releases: {ex.Message}");
        }
    }
}
