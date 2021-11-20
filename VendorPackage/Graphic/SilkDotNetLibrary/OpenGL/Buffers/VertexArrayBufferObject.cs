using Silk.NET.OpenGL;
using System;

namespace SilkDotNetLibrary.OpenGL.Buffers;

public struct VertexArrayBufferObject<TVertexType, TIndexType>
   where TVertexType : unmanaged
   where TIndexType : unmanaged
{
    public uint VertexArrayBufferObjectHandle { get; init; }
    private bool disposedValue;

    public VertexArrayBufferObject(in GL gl, in BufferObject<TVertexType> vbo, in BufferObject<TIndexType> ebo)
    {
        disposedValue = false;
        //Setting out handle and binding the VBO and EBO to this VAO.
        VertexArrayBufferObjectHandle = gl.GenVertexArray();
        BindBy(gl);
        vbo.Bind(gl);
        ebo.Bind(gl);
    }

    public unsafe void VertexAttributePointer(in GL gl,
                                              uint index,
                                              int count,
                                              VertexAttribPointerType vertexAttribPointerType,
                                              uint vertexSize,
                                              int offSet)
    {
        //Setting up a vertex attribute pointer
        gl.VertexAttribPointer(index,
                                size: count,
                                vertexAttribPointerType,
                                false,
                                vertexSize * (uint)sizeof(TVertexType),
                                (void*)(offSet * sizeof(TVertexType)));
        gl.EnableVertexAttribArray(index);
    }

    public void BindBy(in GL gl) => gl.BindVertexArray(VertexArrayBufferObjectHandle);

    private void OnDispose(in GL gl) => gl.DeleteVertexArray(VertexArrayBufferObjectHandle);

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
    // ~VertexArrayObjectObject()
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
