using SilkDotNetLibrary.Input;
using System;
using System.Threading;
using System.Threading.Tasks;
using SharedLibrary.Systems;

namespace SilkDotNetLibrary.OpenGL.Windows;

public interface IWindowEventHandler : ISystem, IKeyboardInputHandler, IMouseInputHandler,IDisposable
{
    Task Start(CancellationToken cancellationToken);
    Task Stop(CancellationToken cancellationToken);
    void OnRender(double dt);
    void OnClosing();

}
