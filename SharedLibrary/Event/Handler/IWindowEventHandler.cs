using System;

namespace SharedLibrary.Event.Handler;

public interface IWindowEventHandler
{
    event EventHandler<double> OnWindowUpdate;
    void OnWindowUpdateUpdateHandler(double dt);
}