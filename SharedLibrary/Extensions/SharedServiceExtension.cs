using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Cameras;

namespace SharedLibrary.Extensions;

public static class SharedServiceExtension
{
    public static IServiceCollection AddShared(this IServiceCollection services)
    {
        services.AddSerilog();
        services.AddScoped<ICamera, Camera>();
        services.AddScoped<Event.Handler.IEventHandler, Event.Handler.EventHandler>();
        return services;
    }
}