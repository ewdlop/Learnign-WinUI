using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Events;
using Serilog;
using System;
using CoreLibrary.SilkDotNet.Window;
using Silk.NET.Windowing;

namespace ConsoleApp
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
                Log.Information("Starting host");
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
                    services.AddScoped(
                        (servicProvider) =>
                        {
                            var options = WindowOptions.Default;
                            options.Title = "LearnOpenGL with Silk.NET";
                            var window = Window.Create(options);
                            return new SilkDotNetWindowEventHandler(window);
                        }
                    );
                    services.AddHostedService(
                        (servicProvider)=>
                        {
                            Log.Information("Creating App...");
                            return new App(servicProvider.GetRequiredService<SilkDotNetWindowEventHandler>());
                        }
                    );
                });
    }
}
