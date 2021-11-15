using Silk.NET.OpenGL;

namespace SilkDotNetLibrary.OpenGL.Buffers;

public interface IBufferObject<TDataType> where TDataType : unmanaged
{
    uint BufferHandle { get; }
    void Bind(in GL gl);
}
