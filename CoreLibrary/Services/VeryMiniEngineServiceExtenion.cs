using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SharedLibrary.Options;
using Silk.NET.Maths;
using SilkDotNetLibrary.OpenGL.Services;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using ECS;

namespace CoreLibrary.Services;

public static class VeryMiniEngineServiceExtension
{
    public static IServiceCollection AddVeryMiniEngine(this IServiceCollection services, Action<WindowOptions> configure)
    {
        AssemblyInformationalVersionAttribute assemblyInformation = ((AssemblyInformationalVersionAttribute[])typeof(object).Assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false))[0];
        string[] informationalVersionSplit = assemblyInformation.InformationalVersion.Split('+');

        Log.Information("**.NET information**");
        Log.Information($"{nameof(Environment.Version)}: {Environment.Version}");
        Log.Information($"{nameof(RuntimeInformation.FrameworkDescription)}: {RuntimeInformation.FrameworkDescription}");
        Log.Information($"Libraries version: {informationalVersionSplit[0]}");
        Log.Information($"Libraries hash: {informationalVersionSplit[1]}");
        Log.Information("");
        Log.Information("**Environment information");
        Log.Information($"{nameof(Environment.ProcessorCount)}: {Environment.ProcessorCount}");
        Log.Information($"{nameof(RuntimeInformation.OSArchitecture)}: {RuntimeInformation.OSArchitecture}");
        Log.Information($"{nameof(RuntimeInformation.OSDescription)}: {RuntimeInformation.OSDescription}");
        Log.Information($"{nameof(Environment.OSVersion)}: {Environment.OSVersion}");
        Log.Information("**");
        Log.Information("");

        WindowOptions windowOptions = new();
        configure(windowOptions);
        services.AddSilkDotNetOpenGLWindow(options =>
        {
            options.Title = windowOptions.Title;
            options.Size = new Vector2D<int>(windowOptions.Width, windowOptions.Height);
        });
        services.AddEcs();
        return services;
    }
}
