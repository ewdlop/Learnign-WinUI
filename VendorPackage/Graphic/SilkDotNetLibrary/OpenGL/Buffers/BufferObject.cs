using Silk.NET.OpenGL;
using System;

namespace SilkDotNetLibrary.OpenGL.Buffers;

public struct BufferObject<TDataType> : IBufferObject<TDataType>
    where TDataType : unmanaged
{
    private bool _disposedValue;
    public uint BufferHandle { get; }
    public BufferTargetARB BufferTargetARB { get; }

    public unsafe BufferObject(GL gl, ReadOnlySpan<TDataType> span, BufferTargetARB bufferTargetARB)
    {
        //Setting the gl instance and storing our buffer type.
        _disposedValue = false;
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
    private void Dispose(bool disposing, GL gl)
    {
        if (_disposedValue) return;
        if (disposing)
        {
            OnDispose(gl);
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        _disposedValue = true;
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~BufferObject()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void DisposeBy(GL gl)
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true, gl);
        GC.SuppressFinalize(this);
    }
}
