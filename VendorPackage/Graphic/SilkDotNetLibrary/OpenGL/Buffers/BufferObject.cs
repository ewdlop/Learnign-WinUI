using Silk.NET.OpenGL;
using System;

namespace SilkDotNetLibrary.OpenGL.Buffers;

public struct BufferObject<TDataType> : IBufferObject<TDataType>
    where TDataType : unmanaged
{
    private bool disposedValue;
    public uint BufferHandle { get; init; }
    public BufferTargetARB BufferTargetARB { get; init; }

    public unsafe BufferObject(in GL gl, Span<TDataType> span, in BufferTargetARB bufferTargetARB)
    {
        //Setting the gl instance and storing our buffer type.
        disposedValue = false;
        BufferHandle = gl.GenBuffer();
        BufferTargetARB = bufferTargetARB;
        Bind(gl);
        fixed (void* data = span)
        {
            gl.BufferData(BufferTargetARB,
                           (nuint)(span.Length * sizeof(TDataType)),
                           data,
                           BufferUsageARB.StaticDraw);
        }
    }

    public void Bind(in GL gl)
    {
        //Binding the buffer object, with the correct buffer type.
        gl.BindBuffer(BufferTargetARB, BufferHandle);
    }

    private void OnDispose(in GL gl)
    {
        gl.DeleteBuffer(BufferHandle);
    }
    private void Dispose(bool disposing, in GL gl)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                OnDispose(gl);
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~BufferObject()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void DisposeBy(in GL gl)
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true, gl);
        GC.SuppressFinalize(this);
    }
}
