using Microsoft.Extensions.DependencyInjection;
using Silk.NET.Windowing;
using SilkDotNetLibrary.OpenGL.Windows;
using SilkDotNetLibrary.OpenGL.Apps;
using System;
using SharedLibrary.Cameras;

namespace SilkDotNetLibrary.OpenGL.Services;

public static class SilkDotNetOpenGLWindowServiceExtension
{
    public static IServiceCollection AddSilkDotNetOpenGLWindow(this IServiceCollection services,
                                                                 Action<WindowOptions> configure)
    {
        var windowOptions = WindowOptions.Default;
        configure(windowOptions);

        return services.AddScoped(_ => Window.Create(windowOptions))
            .AddScoped<SharedLibrary.Event.Handler.IEventHandler, SharedLibrary.Event.Handler.EventHandler>()
            .AddScoped<ICamera, Camera>()
            .AddScoped<OpenGLContext>()
            .AddScoped<IWindowEventHandler,WindowEventHandler>()
            .AddHostedService<App>();
    }
}
