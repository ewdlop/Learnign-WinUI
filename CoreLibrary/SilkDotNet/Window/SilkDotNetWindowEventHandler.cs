using Serilog;
using Silk.NET.Windowing;
using System;

namespace CoreLibrary.SilkDotNet.Window
{
    public class SilkDotNetWindowEventHandler : WindowEventHandler
    {
        public SilkDotNetWindowEventHandler(IWindow Window) : base(Window){}

        public override void OnRender(double dt)
        {
        }

        public override void OnStop()
        {

        }

        public override void OnUpdate(double dt)
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
