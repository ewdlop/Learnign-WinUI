using System;
using Silk.NET.OpenGL;

namespace SilkDotNetLibrary.OpenGL.Buffers;

public readonly struct FrameBufferObject
{
    public uint FrameBufferObjectHandle { get; }
    // create a color attachment texture
    public FrameBufferObject(GL gl)
    {
        FrameBufferObjectHandle = gl.GenFramebuffer();
    }

    public void Load(GL gl)
    {
        BindBy(gl);
    }
    public void BindBy(GL gl) => gl.BindFramebuffer(GLEnum.Framebuffer,FrameBufferObjectHandle);
    private void OnDispose(GL gl) => gl.DeleteFramebuffer(FrameBufferObjectHandle);
    public void DisposeBy(GL gl)
    {
        OnDispose(gl);
    }
}