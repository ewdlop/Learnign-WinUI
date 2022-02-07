using Microsoft.Extensions.Logging;
using SharedLibrary.Components;

namespace Leopotam.EcsLite;

public class EntitySystem : IDisposable
{
    private bool _disposedValue;
    private readonly EcsWorld _ecsWorld;
    private readonly ILogger<EntitySystem> _logger;

    public EntitySystem(EcsWorld ecsWorld, ILogger<EntitySystem> logger)
    {
        _ecsWorld = ecsWorld;
        _logger = logger;
        //_ecsSystems.Init();
        _logger.LogInformation("Creating EntitySystem...");
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
            _logger.LogInformation(ex.Message);
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
        _logger.LogInformation("EntitySystem Already Disposed...");
    }

    // // TODO: override finalizer only if 'Dispose

    public void Dispose()
    {
        _logger.LogInformation("EntitySystem Disposing...");
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}