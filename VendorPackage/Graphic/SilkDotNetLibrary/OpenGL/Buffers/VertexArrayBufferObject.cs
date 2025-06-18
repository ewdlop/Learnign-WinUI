using Silk.NET.OpenGL;
using System;

namespace SilkDotNetLibrary.OpenGL.Buffers;

public readonly struct VertexArrayBufferObject<TVertexType, TIndexType>
   where TVertexType : unmanaged
   where TIndexType : unmanaged
{
    public uint VertexArrayBufferObjectHandle { get; }

    public VertexArrayBufferObject(GL gl, BufferObject<TVertexType> vbo, BufferObject<TIndexType> ebo)
    {
        //Setting out handle and binding the VBO and EBO to this VAO.
        VertexArrayBufferObjectHandle = gl.GenVertexArray();
        BindBy(gl);
        vbo.BindBy(gl);
        ebo.BindBy(gl);
    }

    public VertexArrayBufferObject(GL gl, BufferObject<TVertexType> vbo)
    {
        //Setting out handle and binding the VBO and EBO to this VAO.
        VertexArrayBufferObjectHandle = gl.GenVertexArray();
        BindBy(gl);
        vbo.BindBy(gl);
    }


    public unsafe void VertexAttributePointer(GL gl,
                                              uint index,
                                              int count,
                                              VertexAttribPointerType vertexAttribPointerType,
                                              uint vertexSize,
                                              nint offSet)
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

    public void BindBy(GL gl) => gl.BindVertexArray(VertexArrayBufferObjectHandle);

    private void OnDispose(GL gl) => gl.DeleteVertexArray(VertexArrayBufferObjectHandle);
    public void DisposeBy(GL gl)
    {
        OnDispose( gl);
    }
}
