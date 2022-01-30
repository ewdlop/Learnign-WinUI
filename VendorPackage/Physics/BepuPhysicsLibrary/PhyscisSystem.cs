using System;
using BepuPhysics;
using BepuUtilities;
using BepuUtilities.Memory;
using Serilog;
using SharedLibrary.Cameras;

namespace BepuPhysicsLibrary;

public abstract class PhysicsSystem : IDisposable
{
    public const float TIMESTEP_DURATION = 1 / 60f;
    private bool _disposedValue;
    private readonly BufferPool _bufferPool;
    private readonly IThreadDispatcher ThreadDispatcher;
    private readonly ILogger _logger;
    private readonly Camera _camera;
    public Simulation Simulation { get; protected set; }

    protected PhysicsSystem(ILogger logger)
    {
        _logger = logger;
        _bufferPool = new BufferPool();
        var targetThreadCount = Math.Max(1,
            Environment.ProcessorCount > 4 ? Environment.ProcessorCount - 2 : Environment.ProcessorCount - 1);
        //ThreadDispatcher = new ThreadDispatcher(targetThreadCount);
    }

    public virtual void LoadGraphicalContent(ContentArchive content, RenderSurface surface)
    {
    }

    public abstract void Initialize(ContentArchive content, Camera camera);
    public virtual void Update(float dt)
    {
        Simulation.Timestep(TIMESTEP_DURATION);
    }

    public void OnDispose()
    {
        (_bufferPool as IDisposable)?.Dispose();
        Simulation?.Dispose();
    }

    public void Dispose(bool disposing)
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
        _logger.Information("OpnGLContext Already Disposed...");
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}

public class ContentArchive
{

}
public class RenderSurface
{

}