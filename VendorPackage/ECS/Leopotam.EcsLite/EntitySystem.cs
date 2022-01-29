using Serilog;
using Leopotam.EcsLite;

namespace ECS;

public class EntitySystem : IDisposable
{
    private bool _disposedValue;
    private readonly EcsWorld _ecsWorld;
    private readonly ILogger _logger;
    public EntitySystem(EcsWorld ecsWorld, ILogger logger)
    {
        _ecsWorld = ecsWorld;
        _logger = logger;
        //_ecsSystems.Init();
        _logger.Information("Creating EntitySystem...");
    }

    public void OnUpdate(double dt)
    {
        //_ecsSystems.Run();
    }

    public EcsPackedEntityWithWorld CreateEntity()
    {
        int entity = _ecsWorld.NewEntity();
        return _ecsWorld.PackEntityWithWorld(entity);
    }

    public bool TryRemoveEntity(EcsPackedEntityWithWorld entity)
    {
        try
        {
            _ecsWorld.DelEntity(entity.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Information(ex.Message);
            return false;
        }
    }
    public bool TryAddComponent<T>(EcsPackedEntityWithWorld entity, ref T component) where T : struct
    {
        try
        {
            EcsPool<T> pool = _ecsWorld.GetPool<T>();
            component = pool.Add(entity.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Information(ex.Message);
            return false;
        }
    }

    public bool TryGetComponent<T>(EcsPackedEntityWithWorld entity, ref T component) where T : struct
    {
        try
        {
            EcsPool<T> pool = _ecsWorld.GetPool<T>();
            component = pool.Get(entity.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Information(ex.Message);
            return false;
        }
    }

    public bool TryRemoveComponent<T>(EcsPackedEntityWithWorld entity) where T : struct
    {
        try
        {
            EcsPool<T> pool = _ecsWorld.GetPool<T>();
            pool.Del(entity.Id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.Information(ex.Message);
            return false;
        }
    }

    private void OnDispose()
    {
        //_ecsSystems.Destroy();
        _ecsWorld.Destroy();
        //_ecsWorld = null;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                OnDispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _disposedValue = true;
        }
        _logger.Information("EntitySystem Already Disposed...");
    }

    // // TODO: override finalizer only if 'Dispose

    public void Dispose()
    {
        _logger.Information("EntitySystem Disposing...");
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}