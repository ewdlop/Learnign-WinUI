using SilkDotNetLibraries.OpenGL.Window;
using Serilog;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp
{
    public class App : IApp
    {
        private readonly IWindowEventHandler _windowEventHandler;
        protected bool disposedValue;

        public App(WindowEventHandler windowEventHandler)
        {
            Log.Information("Creating App...");
            _windowEventHandler = windowEventHandler;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Log.Information("App Starting...");
            await _windowEventHandler.Start(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            Log.Information("App Stopping...");
            await _windowEventHandler.Stop(cancellationToken);
        }
        protected virtual void OnDipose()
        {
            _windowEventHandler.Dispose();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OnDipose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
            Log.Information("App Already Diposed...");
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
            Log.Information("App Disposing...");
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
