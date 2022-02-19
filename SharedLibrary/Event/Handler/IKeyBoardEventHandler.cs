using System;

namespace SharedLibrary.Event.Handler;

public interface IKeyBoardEventHandler
{
    event EventHandler<string> OnKeyBoardKeyDown;
    void OnKeyBoardKeyDownHandler(string keyCode);
}
