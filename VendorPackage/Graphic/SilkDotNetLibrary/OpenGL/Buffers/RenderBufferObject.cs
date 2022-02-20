using Silk.NET.OpenGL;

namespace SilkDotNetLibrary.OpenGL.Buffers;

public readonly struct RenderBufferObject
{
    public uint RenderBufferObjectHandle { get; }

    public RenderBufferObject(GL gl, uint width, uint height)
    {
        RenderBufferObjectHandle = gl.GenRenderbuffer();
        Load(gl, width,height);
    }

    public void Load(GL gl,uint width, uint height)
    {
        BindBy(gl);
        gl.RenderbufferStorage(GLEnum.Renderbuffer, GLEnum.Depth24Stencil8, width, height);
        gl.FramebufferRenderbuffer(GLEnum.Framebuffer, GLEnum.DepthStencilAttachment, GLEnum.Renderbuffer, RenderBufferObjectHandle);
    }

    private void BindBy(GL gl)
    {
        gl.BindRenderbuffer(GLEnum.Renderbuffer, RenderBufferObjectHandle);
    }
    private void OnDispose(GL gl) => gl.DeleteRenderbuffer(RenderBufferObjectHandle);
    public void DisposeBy(GL gl)
    {
        OnDispose(gl);
    }
}