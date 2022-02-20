using Silk.NET.OpenGL;
using System;

namespace SilkDotNetLibrary.OpenGL.Buffers;

public readonly struct BufferObject<TDataType> : IBufferObject<TDataType>
    where TDataType : unmanaged
{
    public uint BufferHandle { get; }
    public BufferTargetARB BufferTargetARB { get; }

    public unsafe BufferObject(GL gl, ReadOnlySpan<TDataType> span, BufferTargetARB bufferTargetARB)
    {
        //Setting the gl instance and storing our buffer type.
        BufferHandle = gl.GenBuffer();
        BufferTargetARB = bufferTargetARB;
        BindBy(gl);
        fixed (void* data = span)
        {
            gl.BufferData(BufferTargetARB,
                           (nuint)(span.Length * sizeof(TDataType)),
                           data,
                           BufferUsageARB.StaticDraw);
        }
    }

    public void BindBy(GL gl)
    {
        //Binding the buffer object, with the correct buffer type.
        gl.BindBuffer(BufferTargetARB, BufferHandle);
    }

    private void OnDispose(GL gl)
    {
        gl.DeleteBuffer(BufferHandle);
    }

    public void DisposeBy(GL gl)
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        OnDispose(gl);
    }
}
