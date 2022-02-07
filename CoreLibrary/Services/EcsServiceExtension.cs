using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Components;

namespace Leopotam.EcsLite;

public static class EcsServiceExtension
{
    public static IServiceCollection AddEcs(this IServiceCollection services)
    {
        services.AddScoped<EcsSystems>();
        services.AddScoped<EntitySystem>();
        services.AddScoped<ComponentSystem<TransformComponent>>();
        return services;
    }
}