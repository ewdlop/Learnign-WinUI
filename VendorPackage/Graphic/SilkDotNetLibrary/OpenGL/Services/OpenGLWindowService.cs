using Microsoft.Extensions.DependencyInjection;
using Silk.NET.Windowing;
using SilkDotNetLibrary.OpenGL.Apps;
using SilkDotNetLibrary.OpenGL.Windows;
using System;

namespace SilkDotNetLibrary.OpenGL.Services;

public static class SilkDotNetOpenGLWindowServiceExtension
{
    public static IServiceCollection AddSilkDotNetOpenGLWindow(this IServiceCollection services,
                                                                 Action<WindowOptions> configure)
    {
        var windowOptions = WindowOptions.Default;
        configure(windowOptions);
        return services.AddScoped(_ => Window.Create(windowOptions))
            .AddScoped<OpenGLContext>()
            .AddScoped<IWindowEventHandler,WindowEventHandler>()
            .AddHostedService<App>();
    }
}
