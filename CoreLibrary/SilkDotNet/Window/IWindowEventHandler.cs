using Silk.NET.Input;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CoreLibrary.SilkDotNet.Window
{
    public interface IInputInputHandler
    {
        void KeyDown(IKeyboard arg1, Key arg2, int arg3);
    }
    public interface IWindowEventHandler : IInputInputHandler, IDisposable
    {
        Task Start(CancellationToken cancellationToken);
        Task Stop(CancellationToken cancellationToken);
        void OnLoad();
        void OnUpdate(double dt);
        void OnRender(double dt);
        void OnClose();
        void OnClosing();
        void OnStop();
    }
}
