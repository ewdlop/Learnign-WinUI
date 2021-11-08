using Serilog;
using Silk.NET.Input;
using Silk.NET.Windowing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SilkDotNetLibrary.OpenGL.Window
{
    public class WindowEventHandler : IWindowEventHandler
    {
        private readonly OpenGLContext _openGLContext;
        protected bool disposedValue;

        private IInputContext Input { get; set; }
        private IWindow Window { get; set; }

        public WindowEventHandler(IWindow window, OpenGLContext openGLContext)
        {
            Window = window;
            _openGLContext = openGLContext;
        }

        public virtual Task Start(CancellationToken cancellationToken)
        {
            Window.Load += OnLoad;
            Window.Update += OnUpdate;
            Window.Render += OnRender;
            Window.Closing += OnClosing;
            return Task.Run(() => Window?.Run(), cancellationToken);
        }
        public virtual Task Stop(CancellationToken cancellationToken)
        {
            Log.Information("Window Closing...");
            return Task.Run(() => Window?.Close(), cancellationToken);
        }

        public virtual void OnLoad()
        {
            Input = Window.CreateInput();
            for (int i = 0; i < Input.Keyboards.Count; i++)
            {
                Input.Keyboards[i].KeyDown += OnKeyDown;
            }
            _openGLContext.OnLoad();
        }

        public virtual void OnRender(double dt) => _openGLContext.OnRender(dt);

        public virtual void OnStop()
        {

        }

        public virtual void OnUpdate(double dt) => _openGLContext.OnUpdate(dt);

        public virtual void OnClosing()
        {
            //trigger by closing Window
            _openGLContext.Dispose();
            Window = null;
        }

        public virtual void OnKeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            Log.Information("Escpae Key Pressed...");
            if (arg2 == Key.Escape)
            {
                OnDispose();
            }
        }

        protected virtual void OnDispose()
        {
            Log.Information("Input Dispose...");
            Input?.Dispose();
            _openGLContext.Dispose();
            Log.Information("Window Disposing...");
            if (Window is not null)
            {
                Window.Dispose();
                Log.Information("Window Disposed...");
            }
            else
            {
                Log.Information("Window could not be found...");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    OnDispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
            Log.Information("WindowEventHandler Already Diposed...");
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~WindowEventHandler()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Log.Information("WindowEventHandler Disposing...");
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
