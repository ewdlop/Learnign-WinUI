using Silk.NET.Input;

namespace SilkDotNetLibrary.Input;

public interface IKeyboardInputHandler
{
    void OnKeyDown(IKeyboard arg1, Key arg2, int arg3);
}
