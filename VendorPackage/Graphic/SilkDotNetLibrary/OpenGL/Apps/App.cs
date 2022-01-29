using Microsoft.Extensions.Logging;
using SilkDotNetLibrary.OpenGL.Windows;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SilkDotNetLibrary.OpenGL.Apps;

public class App : IApp
{
    private readonly ILogger _logger;
    private readonly IWindowEventHandler _windowEventHandler;
    protected bool _disposedValue;

    public App(IWindowEventHandler windowEventHandler, ILogger<App> logger)
    {
        _logger = logger;
        _logger.LogInformation("Creating App...");
        _windowEventHandler = windowEventHandler;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("App Starting...");
        _logger.LogInformation("App Starting thread ID: {threadId}", Environment.CurrentManagedThreadId);
        await _windowEventHandler.Start(cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Window App Stopping...");
        _logger.LogInformation("Window App Stopping thread ID: {threadId}", Environment.CurrentManagedThreadId);
        await _windowEventHandler.Stop(cancellationToken);
    }
    protected virtual void OnDispose()
    {
        _windowEventHandler.Dispose();
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
        _logger.LogInformation("App Already Disposed...");
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~App()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        _logger.LogInformation("App Disposing...");
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
