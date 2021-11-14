using Serilog;
using SharedLibrary.Event.Handler;
using Silk.NET.Input;
using Silk.NET.Windowing;
using System;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace SilkDotNetLibrary.OpenGL.Windows;

public class WindowEventHandler : IWindowEventHandler
{
    protected bool disposedValue;
    private readonly OpenGLContext _openGLContext;
    private readonly IEventHandler _eventHandler;
    private IKeyboard PrimaryKeyboard { get; set; }
    private IInputContext Input { get; set; }
    private IWindow Window { get; set; }
    private Vector2 LastMousePosition { get; set; }

    //private EventHandler<>
    public WindowEventHandler(IWindow window, OpenGLContext openGLContext, IEventHandler eventHandler)
    {
        Window = window;
        _openGLContext = openGLContext;
        _eventHandler = eventHandler;
    }

    public virtual Task Start(CancellationToken cancellationToken)
    {
        Window.Load += OnLoad;
        Window.Update += OnUpdate;
        Window.Render += OnRender;
        Window.Closing += OnClosing;
        return Task.Run(() =>
        {
            Log.Information("Window Running thread ID: {0}", Environment.CurrentManagedThreadId);
            Window?.Run();
        }, cancellationToken);
    }
    public virtual Task Stop(CancellationToken cancellationToken)
    {
        Log.Information("Window Closing...");
        return Task.Run(() =>
        {
            Log.Information("Window Closing thread ID: {0}", Environment.CurrentManagedThreadId);
            Window?.Close();
        }, cancellationToken);
    }

    public virtual void OnLoad()
    {
        Input = Window.CreateInput();
        PrimaryKeyboard = Input.Keyboards.FirstOrDefault();
        if (PrimaryKeyboard != null)
        {
            PrimaryKeyboard.KeyDown += OnKeyDown;
        }
        for (int i = 0; i < Input.Mice.Count; i++)
        {
            Input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
            Input.Mice[i].MouseMove += OnMouseMove;
            Input.Mice[i].Scroll += OnMouseWheel;
        }
        _openGLContext.OnLoad();
    }

    public virtual void OnRender(double dt) => _openGLContext.OnRender(dt);

    public virtual void OnStop()
    {

    }

    public virtual void OnUpdate(double dt)
    {
        var moveSpeed = 2.5f * (float)dt;

        if (PrimaryKeyboard.IsKeyPressed(Key.W))
        {
            //Move forwards
            //CameraPosition += moveSpeed * CameraFront;
        }
        if (PrimaryKeyboard.IsKeyPressed(Key.S))
        {
            //Move backwards
            //CameraPosition -= moveSpeed * CameraFront;
        }
        if (PrimaryKeyboard.IsKeyPressed(Key.A))
        {
            //Move left
            //CameraPosition -= Vector3.Normalize(Vector3.Cross(CameraFront, CameraUp)) * moveSpeed;
        }
        if (PrimaryKeyboard.IsKeyPressed(Key.D))
        {
            //Move right
            //CameraPosition += Vector3.Normalize(Vector3.Cross(CameraFront, CameraUp)) * moveSpeed;
        }
        _openGLContext.OnUpdate(dt);
    }
    public virtual void OnClosing()
    {
        //trigger by closing Window
        Log.Information("Window Closing thread ID: {0}", Environment.CurrentManagedThreadId);
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
    public void OnMouseMove(IMouse mouse, Vector2 position)
    {
        if (LastMousePosition == default)
        { 
            LastMousePosition = position; 
        }
        else
        {
            _eventHandler.OnMouseMoveHandler(new()
            {
                LastMousePosition = LastMousePosition,
                Position = position
            });
            LastMousePosition = position;
        }
    }

    public void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
    {
        _eventHandler.OnMouseScrollWheelHandler(new()
        {
            X = scrollWheel.X,
            Y = scrollWheel.Y
        });
    }

    protected virtual void OnDispose()
    {
        Log.Information("Window Dispose thread ID: {0}", Environment.CurrentManagedThreadId);
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
