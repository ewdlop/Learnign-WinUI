using Microsoft.Extensions.Hosting;
using Serilog.Events;
using Serilog;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using SilkDotNetLibrary.OpenGL.Service;
using System;

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
                    services.UseSilkDotNetOpenGLWindow(options =>
                    {
                        options.Title = "LearnOpenGL with Silk.NET";
                        options.Size = new Vector2D<int>(800, 600);
                    });
                });
    }
}
