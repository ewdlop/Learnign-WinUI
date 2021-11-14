using Silk.NET.Input;
using System.Numerics;

namespace SilkDotNetLibrary.Input;

public interface IMouseInputHandler
{
    unsafe void OnMouseMove(IMouse mouse, Vector2 position);
    unsafe void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel);
}
