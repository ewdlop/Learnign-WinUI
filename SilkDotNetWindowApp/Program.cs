using Microsoft.Extensions.Hosting;
using Serilog.Events;
using Serilog;
using System;
using CoreLibrary.Services;
using Microsoft.Extensions.Configuration;
using Serilog.Sinks.SystemConsole.Themes;
using Microsoft.Extensions.DependencyInjection;

var builder = Host.CreateDefaultBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(theme: SystemConsoleTheme.Literate)
    .CreateBootstrapLogger();

builder.UseSerilog((context, services, configuration) =>
{
        configuration.MinimumLevel.Override("Microsoft", LogEventLevel.Debug)
                    .Enrich.FromLogContext()
                    .Enrich.WithEnvironmentUserName()
                    .Enrich.WithThreadId()
                    .WriteTo.Console(theme: SystemConsoleTheme.Literate)
                    .WriteTo.Async(configure => configure.File("Logs/log.txt",
                        fileSizeLimitBytes: 1_000_000,
                        rollOnFileSizeLimit: true,
                        shared: true,
                        flushToDiskInterval: TimeSpan.FromSeconds(1),
                        rollingInterval: RollingInterval.Day));
})
.ConfigureServices((context, services) =>
{
    IConfiguration configuration = context.Configuration;
    Log.Information("Configuring Service Provider...");
    services.AddVeryMiniEngine(options =>
    {
        options.Title = "LearnOpenGL with Silk.NET";
        options.Width = 800;
        options.Height = 600;
    });
    services.AddApplicationInsightsTelemetryWorkerService();
});
//.UseConsoleLifetime();

IHost host = builder.Build();
try
{
    Log.Information("Starting Host...");
    using (host)
    {
        await host.StartAsync();
        await host.WaitForShutdownAsync();
    }
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

return 0;