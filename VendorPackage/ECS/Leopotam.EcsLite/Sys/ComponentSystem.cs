using Microsoft.Extensions.Logging;
using SharedLibrary.Components;

namespace Leopotam.EcsLite.Sys;

public class ComponentSystem<T> where T : struct, IComponent
{
    private readonly EcsWorld _ecsWorld;
    private readonly ILogger<ComponentSystem<T>> _logger;
    public ComponentSystem(EcsWorld ecsWorld, ILogger<ComponentSystem<T>> logger)
    {
        _ecsWorld = ecsWorld;
        _logger = logger;
        //_ecsSystems.Init();
        _logger.LogInformation("Creating {name}", nameof(T));
    }


    public bool TryAddComponent(EcsPackedEntityWithWorld entity, ref T component)
    {
        try
        {
            EcsPool<T> pool = _ecsWorld.GetPool<T>();
            component = pool.Add(entity.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogInformation("{Message}", ex.Message);
            return false;
        }
    }

    public bool TryGetComponent(EcsPackedEntityWithWorld entity, ref T component)
    {
        try
        {
            EcsPool<T> pool = _ecsWorld.GetPool<T>();
            component = pool.Get(entity.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogInformation("{Message}", ex.Message);
            return false;
        }
    }

    public bool TryRemoveComponent(EcsPackedEntityWithWorld entity)
    {
        try
        {
            EcsPool<T> pool = _ecsWorld.GetPool<T>();
            pool.Del(entity.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogInformation("{Message}", ex.Message);
            return false;
        }
    }
}