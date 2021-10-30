using Serilog;
using Silk.NET.Input;
using Silk.NET.Windowing;
using System;
using System.Threading;
using System.Threading.Tasks;
using SilkDotNetWrapper.OpenGL;

namespace SilkDotNetLibraries.Window
{
    public abstract class WindowEventHandler : IWindowEventHandler
    {
        private readonly OpenGLContext _openGLContext;
        protected IInputContext Input { get; set; }
        protected IWindow Window { get; set; }
        protected bool disposed;

        protected WindowEventHandler(IWindow window, OpenGLContext openGLContext)
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
                Input.Keyboards[i].KeyDown += KeyDown;
            }
            _openGLContext.OnLoad();
        }

        public virtual void OnRender(double dt) => _openGLContext.OnRender(dt);

        public abstract void OnStop();

        public virtual void OnUpdate(double dt) => _openGLContext.OnUpdate(dt);

        public virtual void OnClose()
        {
            _openGLContext.OnClose();
            Log.Information("Window Closing...");
            if(Window is not null)
            {
                Window.Close();
                Log.Information("Window Closed...");
            }
            else
            {
                Log.Information("Window could not be found...");
            }
        }

        public virtual void OnClosing()
        {
            _openGLContext.OnClose();
            Window = null; //cannot access IsClosed from IView rip
        }

        public virtual void KeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            Log.Information("Escpae Key Pressed...");
            if (arg2 == Key.Escape)
            {
                OnClose();
            }
        }

        public virtual void Dispose()
        {
            Log.Information("WindowEventHandler Disposing...");
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    // Dispose managed resources.
                    Input.Dispose();
                    OnClose();
                }

                // Note disposing has been done.
                disposed = true;
            }
        }
        ~WindowEventHandler()
        {
            Log.Information("WindowEventHandler Finalizer Disposing...");
            Dispose(disposing: false);
        }
    }
}
