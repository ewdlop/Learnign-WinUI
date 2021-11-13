using Microsoft.Extensions.DependencyInjection;
using Silk.NET.Windowing;
using SilkDotNetLibrary.OpenGL.Windows;
using SilkDotNetLibrary.OpenGL.Apps;
using System;

namespace SilkDotNetLibrary.OpenGL.Services;

public static class OpenGLWindowService
{
    public static IServiceCollection UseSilkDotNetOpenGLWindow(this IServiceCollection services,
                                                                 Action<WindowOptions> configure)
    {
        var windowOptions = WindowOptions.Default;
        configure(windowOptions);

        return services.AddScoped(
                (servicProvider) => Window.Create(windowOptions)
            )
        .AddScoped<OpenGLContext>()
        .AddScoped<WindowEventHandler>()
        .AddHostedService<App>();
    }
}
