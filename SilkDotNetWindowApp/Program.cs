using Microsoft.Extensions.Hosting;
using Serilog.Events;
using Serilog;
using System;
using CoreLibrary.Services;
using Microsoft.Extensions.Configuration;
using CoreLibrary.Options;

namespace SilkDotNetWindowApp;

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
            .ConfigureServices((context, services) =>
            {
                IConfiguration configuration = context.Configuration;
                //WindowOptions windowOption = configuration.GetSection(nameof(WindowOptions)).Get<WindowOptions>();
                Log.Information("Configuring Service Provider...");
                services.UseVeryMiniEngine(options =>
                {
                    options.Title = "LearnOpenGL with Silk.NET";
                    options.Width = 800;
                    options.Height = 600;
                });
            });
}
