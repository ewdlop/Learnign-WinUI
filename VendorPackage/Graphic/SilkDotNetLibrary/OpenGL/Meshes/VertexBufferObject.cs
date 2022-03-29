using Silk.NET.OpenGL;
using System;

namespace SilkDotNetLibrary.OpenGL.Meshes;

public readonly struct VertexBufferObject
{
    public uint BufferHandle { get; }
    public BufferTargetARB BufferTargetARB { get; }

    public unsafe VertexBufferObject(GL gl, ReadOnlySpan<Vertex> span, BufferTargetARB bufferTargetARB)
    {
        //Setting the gl instance and storing our buffer type.
        BufferHandle = gl.GenBuffer();
        BufferTargetARB = bufferTargetARB;
        BindBy(gl);
        fixed (void* data = span)
        {
            gl.BufferData(BufferTargetARB,
                (nuint)(span.Length * sizeof(Vertex)),
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
        OnDispose(gl);
    }
}
