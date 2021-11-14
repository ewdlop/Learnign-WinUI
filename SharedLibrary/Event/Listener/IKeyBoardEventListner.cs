using SharedLibrary.Event.EventArgs;

namespace SharedLibrary.Event.Listener;

public interface IKeyBoardEventListner
{
    void OnKeyBoardKeyDown(object sender, KeyBoardKeyDownEventArgs e);
}
