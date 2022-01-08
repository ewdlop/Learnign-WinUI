using Microsoft.Extensions.Hosting;
using Serilog.Events;
using Serilog;
using System;
using CoreLibrary.Services;
using Microsoft.Extensions.Configuration;

namespace SilkDotNetWindowApp;

public static class Program
{
    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
           .MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
           .Enrich.FromLogContext()
           .WriteTo.Console()
           .WriteTo.Async(configuration => configuration.File("Logs/log.txt",
                fileSizeLimitBytes: 1_000_000,
                rollOnFileSizeLimit: true,
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(1),
                rollingInterval: RollingInterval.Day))
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
            //.UseSerilog((context, services, configuration) => configuration
            //        .ReadFrom.Configuration(context.Configuration)
            //        .ReadFrom.Services(services))
            .ConfigureServices((context, services) =>
            {
                IConfiguration configuration = context.Configuration;
                Log.Information("Configuring Service Provider...");
                services.UseVeryMiniEngine(options =>
                {
                    options.Title = "LearnOpenGL with Silk.NET";
                    options.Width = 800;
                    options.Height = 600;
                });
            });
}
