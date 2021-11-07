using SilkDotNetWrapper.OpenGL;
using Serilog;
using Silk.NET.Windowing;

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
    }
}
