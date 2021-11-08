using Microsoft.Extensions.DependencyInjection;
using Silk.NET.Windowing;
using SilkDotNetLibrary.OpenGL.Window;
using SilkDotNetLibrary.OpenGL.Apps;
using System;

namespace SilkDotNetLibrary.OpenGL.Services
{
    public static class OpenGLWindowService
    {
        public static IServiceCollection UseSilkDotNetOpenGLWindow(this IServiceCollection services,
                                                                     Action<WindowOptions> configure)
        {
            var windowOptions = WindowOptions.Default;
            configure(windowOptions);

            return services.AddScoped(
                    (servicProvider) => Silk.NET.Windowing.Window.Create(windowOptions)
                )
            .AddScoped<OpenGLContext>()
            .AddScoped<WindowEventHandler>()
            .AddHostedService<App>();
        }
    }
}
