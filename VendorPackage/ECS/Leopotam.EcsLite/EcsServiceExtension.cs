using Leopotam.EcsLite;
using Microsoft.Extensions.DependencyInjection;

namespace ECS;

public static class EcsServiceExtension
{
    public static IServiceCollection AddEcs(this IServiceCollection services)
    {
        services.AddScoped<EcsSystems>();
        services.AddScoped<EntitySystem>();
        return services;
    }
}