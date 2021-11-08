using Microsoft.Extensions.Hosting;
using Serilog.Events;
using Serilog;
using System;
using CoreLibrary.Services;

namespace SilkDotNetWindowApp
{
    public static class Program
    {
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
               .Enrich.FromLogContext()
               .WriteTo.Console()
               .CreateLogger();

            try
            {
                Log.Information("Starting Host...");
                CreateHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureServices((context, services) => {
                    Log.Information("Configuring Service Provider...");
                    services.UseVeryMiniEngine();
                });
    }
}
