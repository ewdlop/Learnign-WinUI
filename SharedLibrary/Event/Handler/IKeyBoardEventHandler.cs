using SharedLibrary.Event.EventArgs;
using System;

namespace SharedLibrary.Event.Handler;

public interface IKeyBoardEventHandler
{
    event EventHandler<KeyBoardKeyDownEventArgs> OnKeyBoardKeyDown;
    void OnKeyBoardKeyDownHandler(KeyBoardKeyDownEventArgs e);
}
