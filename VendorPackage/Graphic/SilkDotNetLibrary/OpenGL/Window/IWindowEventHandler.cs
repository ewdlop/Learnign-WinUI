using SilkDotNetLibrary.Input;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SilkDotNetLibrary.OpenGL.Window
{
    public interface IWindowEventHandler : IInputInputHandler, IDisposable
    {
        Task Start(CancellationToken cancellationToken);
        Task Stop(CancellationToken cancellationToken);
        //need to move this one interface further down
        void OnLoad();
        void OnUpdate(double dt);
        void OnRender(double dt);
        void OnClosing();
        void OnStop();
    }
}
