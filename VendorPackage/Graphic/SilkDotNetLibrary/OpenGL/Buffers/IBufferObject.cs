using Silk.NET.OpenGL;

namespace SilkDotNetLibrary.OpenGL.Buffers;

public interface IBufferObject
{
    uint BufferHandle { get; }
    void BindBy(GL gl);
}
