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
        public static async Task Main(string[] args)
        {
            var isBreak = false;

            Console.CancelKeyPress += (sender, e) => isBreak = true;

            using var host = CreateHostBuilder(args).Build();

            await host.StartAsync();

            var helpMessage =
                "Wrong input."
                + Environment.NewLine + "Expected format for input is string with format 'functionName param1 param2'."
                + Environment.NewLine + "For example: 'add 3 4' or 'mult 2 add 3 4'.";

            using var servicesScope = host.Services.CreateScope();

            while (!isBreak)
            {
                var input = Console.ReadLine();

                if (input == "exit" || isBreak)
                    break;

                if (input is not null)
                {
                    var analyzer = servicesScope.ServiceProvider.GetRequiredService<ExpressionAnalyzer>();

                    var result = analyzer.GetResultFromExpression(input);

                    if (result is not null)
                        Console.WriteLine(result.HasValue ? $"Result: {result}." : helpMessage);
                }
                else Console.WriteLine(helpMessage);
            }

            await host.StopAsync();
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
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.Configure<ExpressionAnalyzerOptions>(
                     hostContext.Configuration.GetSection(nameof(ExpressionAnalyzerOptions)));

                services.AddTransient<ExpressionAnalyzer>();
            });
    }
}
