using System;
using Silk.NET.OpenGL;

namespace SilkDotNetLibrary.OpenGL.Buffers;

public readonly struct RenderBufferObject
{
    public uint VertexArrayBufferObjectHandle { get; }

    public RenderBufferObject(GL gl, uint width, uint height)
    {
        VertexArrayBufferObjectHandle = gl.GenRenderbuffer();
    }

    public void Load(GL gl,uint width, uint height)
    {
        BindBy(gl);
        gl.RenderbufferStorage(GLEnum.Renderbuffer, GLEnum.Depth24Stencil8, width, height);
        gl.BindRenderbuffer(GLEnum.Renderbuffer, 0);
        gl.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.DepthStencilAttachment, GLEnum.Renderbuffer, VertexArrayBufferObjectHandle);
    }

    private void BindBy(GL gl)
    {
        gl.BindRenderbuffer(GLEnum.Renderbuffer, VertexArrayBufferObjectHandle);
    }
    private void OnDispose(GL gl) => gl.DeleteRenderbuffer(VertexArrayBufferObjectHandle);
    public void DisposeBy(GL gl)
    {
        OnDispose(gl);
    }
}