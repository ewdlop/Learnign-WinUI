using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog.Events;
using Serilog;
using System;
using SilkDotNetLibraries.Window;
using Silk.NET.Windowing;
using Silk.NET.Maths;
using SilkDotNetWrapper.OpenGL;
using CoreLibrary.DependencyInjectionOperation;

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
                    services.AddTransient<IOperationTransient, Operation>();
                    services.AddScoped<IOperationScoped, Operation>();
                    services.AddSingleton<IOperationSingleton, Operation>();
                    services.AddScoped(
                        (servicProvider) =>
                        {
                            var options = WindowOptions.Default;
                            options.Title = "LearnOpenGL with Silk.NET";
                            options.Size = new Vector2D<int>(800, 600);
                            return Window.Create(options);
                        }
                    );
                    services.AddScoped(
                        (servicProvider) =>
                        {
                            return new OpenGLContext(servicProvider.GetRequiredService<IWindow>());
                        }
                    );
                    services.AddScoped(
                        (servicProvider) =>
                        {
                            return new SilkDotNetWindowEventHandler(servicProvider.GetRequiredService<IWindow>(),
                                                                    servicProvider.GetRequiredService<OpenGLContext>());
                        }
                    );
                    services.AddHostedService(
                        (servicProvider)=>
                        {
                            return new App(servicProvider.GetRequiredService<SilkDotNetWindowEventHandler>());
                        }
                    );
                });
    }
}
