using Silk.NET.OpenGL;
using System;

namespace SilkDotNetLibrary.OpenGL.Buffers;

public struct VertexArrayBufferObject<TVertexType, TIndexType> : IDisposable
   where TVertexType : unmanaged
   where TIndexType : unmanaged
{
    public readonly uint _vertexArrayBufferObjectHandle;
    private readonly GL _gl;
    private bool disposedValue;

    public VertexArrayBufferObject(GL gl, BufferObject<TVertexType> vbo, BufferObject<TIndexType> ebo)
    {
        disposedValue = false;
        _gl = gl;
        //Setting out handle and binding the VBO and EBO to this VAO.
        _vertexArrayBufferObjectHandle = _gl.GenVertexArray();
        Bind();
        vbo.Bind();
        ebo.Bind();
    }

    public unsafe void VertexAttributePointer(uint index,
                                              int count,
                                              VertexAttribPointerType vertexAttribPointerType,
                                              uint vertexSize,
                                              int offSet)
    {
        //Setting up a vertex attribute pointer
        _gl.VertexAttribPointer(index,
                                size: count,
                                vertexAttribPointerType,
                                false,
                                vertexSize * (uint)sizeof(TVertexType),
                                (void*)(offSet * sizeof(TVertexType)));
        _gl.EnableVertexAttribArray(index);
    }

    public void Bind() =>
        //Binding the vertex array.
        _gl.BindVertexArray(_vertexArrayBufferObjectHandle);

    private void OnDipose() => _gl.DeleteVertexArray(_vertexArrayBufferObjectHandle);

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                OnDipose();
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

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
