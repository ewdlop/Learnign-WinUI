using Microsoft.Extensions.DependencyInjection;
using Silk.NET.Windowing;
using SilkDotNetLibrary.OpenGL.Windows;
using SilkDotNetLibrary.OpenGL.Apps;
using System;
using SharedLibrary.Cameras;
using Silk.NET.OpenGL;

namespace SilkDotNetLibrary.OpenGL.Services;

public static class OpenGLWindowService
{
    public static IServiceCollection UseSilkDotNetOpenGLWindow(this IServiceCollection services,
                                                                 Action<WindowOptions> configure)
    {
        var windowOptions = WindowOptions.Default;
        configure(windowOptions);

        return services.AddScoped((servicProvider) => Window.Create(windowOptions))
            .AddScoped<SharedLibrary.Event.Handler.IEventHandler, SharedLibrary.Event.Handler.EventHandler>()
            .AddScoped<ICamera, Camera>()
            //.AddScoped((servicProvider) =>
            //{
            //    var window = servicProvider.GetService<IWindow>();
            //    return window.CreateOpenGL();
            //})
            .AddScoped<OpenGLContext>()
            .AddScoped<WindowEventHandler>()
            .AddHostedService<App>();
    }
}
