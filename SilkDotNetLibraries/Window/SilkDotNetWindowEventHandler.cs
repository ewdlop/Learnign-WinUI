using SilkDotNetWrapper.OpenGL;
using Serilog;
using Silk.NET.Windowing;
using System;

namespace SilkDotNetLibraries.Window
{
    public class SilkDotNetWindowEventHandler : WindowEventHandler
    {
        public SilkDotNetWindowEventHandler(IWindow window, OpenGLContext openGLContext) : base(window, openGLContext) 
        {
            Log.Information("Creating SilkDotNetWindowEventHandler...");
        }

        public override void OnStop()
        {

        }

        public override void Dispose()
        {
            Log.Information("SilkDotNetWindowEventHandler Disposing...");
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected override void Dispose(bool disposing)
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

        ~SilkDotNetWindowEventHandler()
        {
            Log.Information("SilkDotNetWindowEventHandler Finalizer Disposing...");
            Dispose(disposing: false);
        }
    }
}
