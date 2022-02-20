using System;
using Silk.NET.OpenGL;

namespace SilkDotNetLibrary.OpenGL.Buffers;

public unsafe struct FrameBufferObject
{
    public uint _frameBufferObjectHandle;
    // create a color attachment texture
    public FrameBufferObject(GL gl)
    {
        fixed (uint* hFbo = &_frameBufferObjectHandle)
        {
            gl.GenFramebuffers(1, hFbo);
        }
        Load(gl);
    }

    public void Load(GL gl)
    {
        BindBy(gl);
    }
    public void BindBy(GL gl) => gl.BindFramebuffer(GLEnum.Framebuffer, _frameBufferObjectHandle);
    private void OnDispose(GL gl) => gl.DeleteFramebuffer(_frameBufferObjectHandle);
    public void DisposeBy(GL gl)
    {
        OnDispose(gl);
    }
}