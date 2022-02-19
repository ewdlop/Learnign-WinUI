using System;

namespace SharedLibrary.Event.Handler;

public interface IKeyBoardEventHandler
{
    event EventHandler<char> OnKeyBoardKeyDown;
    void OnKeyBoardKeyDownHandler(char keyCode);
}
