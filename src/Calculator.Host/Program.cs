using Calculator.Analyzer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace Calculator.Host
{
    public class Program
    {
        private static IHost? _host;
        private static IConfiguration? _configuration;
        private static Lazy<IServiceProvider>? _serviceProvider;

        public static async Task Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);

            _serviceProvider = new Lazy<IServiceProvider>(() => _host!.Services);
            _host = CreateHostBuilder(args).Build();

            await _host.StartAsync();

            var helpMessage =
                "Wrong input."
                + Environment.NewLine + "Expected format for input is string with format 'functionName param1 param2'."
                + Environment.NewLine + "For example: 'add 3 4' or 'mult 2 add 3 4'.";

            while (true)
            {
                var input = Console.ReadLine();

                if (input is not null)
                {
                    var analyzer = _serviceProvider!.Value.GetRequiredService<ExpressionAnalyzer>();

                    var result = analyzer.GetResultFromExpression(input);

                    if (result is not null)
                        Console.WriteLine($"Result: {result}.");
                }

                Console.WriteLine(helpMessage);
            }
        }

        static void CurrentDomain_ProcessExit(object? sender, EventArgs? e)
        {
            _host?.StopAsync().Wait();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host
            .CreateDefaultBuilder(args)
            .UseConsoleLifetime()
            .ConfigureAppConfiguration((hostContext, configuration) =>
            {
                configuration
                    .AddJsonFile("appsettings.json")
                    .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true);

                _configuration = configuration.Build();
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<ExpressionAnalyzerOptions>(
                     _configuration!.GetSection(nameof(ExpressionAnalyzerOptions)));

                services.AddTransient<ExpressionAnalyzer>();
            });
    }
}
