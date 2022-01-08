using Serilog;
using SharedLibrary.Event.Handler;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace SilkDotNetLibrary.OpenGL.Windows;

public class WindowEventHandler : IWindowEventHandler
{
    protected bool disposedValue;
    private GL GL { get; set; }
    private readonly OpenGLContext _openGLContext;
    private readonly IEventHandler _eventHandler;
    private readonly IReadOnlyDictionary<Key, string> _keyBoardKeyMap;
    private IKeyboard PrimaryKeyboard { get; set; }
    private IInputContext InputContext { get; set; }
    private ImGuiController ImGuiController { get; set; }
    private IWindow Window { get; set; }
    private Vector2 LastMousePosition { get; set; }

    //private EventHandler<>
    public WindowEventHandler(IWindow window,
                              OpenGLContext openGLContext,
                              IEventHandler eventHandler)
    {
        Window = window;
        _openGLContext = openGLContext;
        _eventHandler = eventHandler;
        _keyBoardKeyMap = new Dictionary<Key, string>()
        {
            {Key.W, "W"},
            {Key.S, "S" },
            {Key.A, "A" },
            {Key.D, "D" }
        };
    }

    public virtual Task Start(CancellationToken cancellationToken)
    {
        Window.Load += OnLoad;
        Window.Update += OnUpdate;
        Window.Render += OnRender;
        Window.Closing += OnClosing;
        Window.FramebufferResize += OnFramebufferResize;
        return Task.Run(() =>
        {
            Window?.Run();
        }, cancellationToken);
    }
    public virtual Task Stop(CancellationToken cancellationToken)
    {
        Log.Information("Window Closing...");
        return Task.Run(() =>
        {
            Window?.Close();
        }, cancellationToken);
    }

    public void OnLoad()
    {
        InputContext = Window.CreateInput();
        PrimaryKeyboard = InputContext.Keyboards.FirstOrDefault();
        if (PrimaryKeyboard != null)
        {
            PrimaryKeyboard.KeyDown += OnKeyDown;
        }
        for (int i = 0; i < InputContext.Mice.Count; i++)
        {
            //InputContext.Mice[i].Cursor.CursorMode = CursorMode.Normal;
            InputContext.Mice[i].MouseMove += OnMouseMove;
            InputContext.Mice[i].Scroll += OnMouseWheel;
        }
        _openGLContext.OnLoad();
        //need to think of a better way to do this;
        //and it didnt work
        GL = _openGLContext.OnLoad();
        ImGuiController = new ImGuiController(GL, Window, InputContext);
    }

    public void OnRender(double dt)
    {
        ImGuiController.Update((float)dt);
        _openGLContext.OnRender(dt);
        //doesnt work rip
        ImGuiNET.ImGui.ShowDemoWindow();
        // Make sure ImGui renders too!
        ImGuiController.Render();
    }

    public void OnStop()
    {

    }

    public void OnFramebufferResize(Vector2D<int> resize)
    {
        _openGLContext.OnWindowFrameBufferResize(in resize);
    }

    public void OnUpdate(double dt)
    {
        foreach (var keyValue in _keyBoardKeyMap)
        {
            if (PrimaryKeyboard.IsKeyPressed(keyValue.Key))
            {
                _eventHandler.OnKeyBoardKeyDownHandler(new SharedLibrary.Event.EventArgs.KeyBoardKeyDownEventArgs() { KeyCode = keyValue.Value });
            }
        }
        _openGLContext.OnUpdate(dt);
    }
    public virtual void OnClosing()
    {
        //trigger by closing Window
        //mabye a onclose for this?
        //Window.Close();
        //ImGuiController?.Dispose();
        //_openGLContext?.Dispose();
        //Window?.Dispose();
        //Window = null;
        //Window.Close();
        Dispose();
    }

    public virtual void OnKeyDown(IKeyboard arg1, Key arg2, int arg3)
    {
        //Log.Information("Escpae Key Pressed...");
        //if(_keyBoardKeyMap.TryGetValue(arg2, out string keycode))
        //{
        //    _eventHandler.OnKeyBoardKeyDownHandler(new SharedLibrary.Event.EventArgs.KeyBoardKeyDownEventArgs()
        //    {
        //        KeyCode = keycode
        //    });
        //}

        if (arg2 == Key.Escape)
        {
            OnClosing();
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
        Log.Information("ImGui Disposing...");
        ImGuiController?.Dispose();
        Log.Information("Input Disposing...");
        InputContext?.Dispose();
        Log.Information("GL Disposing...");
        _openGLContext.Dispose();
        Log.Information("Window Disposes...");
        //if (Window is not null)
        //{
        //    Window.Dispose();
        //    Log.Information("Window Disposed...");
        //}
        //else
        //{
        //    Log.Information("Window could not be found...");
        //}
        Window.Dispose();
        Log.Information("Window Disposed...");
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
