using Serilog;
using Silk.NET.Input;
using Silk.NET.Windowing;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoreLibrary.SilkDotNet.Window
{
    public abstract class WindowEventHandler : IWindowEventHandler
    {
        protected IWindow Window { get; set; }
        protected bool disposed;
        public WindowEventHandler(IWindow Window)
        {
            this.Window = Window;
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
            return Task.Run(() => Window?.Close(), cancellationToken);
        }

        public virtual void OnLoad()
        {
            IInputContext input = Window.CreateInput();
            for (int i = 0; i < input.Keyboards.Count; i++)
            {
                input.Keyboards[i].KeyDown += KeyDown;
            }
        }

        public abstract void OnRender(double dt);

        public abstract void OnStop();

        public abstract void OnUpdate(double dt);

        public virtual void OnClose()
        {
            Log.Information("Window Closing...");
            Window?.Close();
        }

        public virtual void OnClosing()
        {
            Window = null;
        }

        public virtual void KeyDown(IKeyboard arg1, Key arg2, int arg3)
        {
            //Check to close the window on escape.
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
