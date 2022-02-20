using System;
using Silk.NET.OpenGL;

namespace SilkDotNetLibrary.OpenGL.Buffers;

public struct FrameBufferObject
{
    private bool _disposedValue;
    public uint FrameBufferObjectHandle { get; }
    // create a color attachment texture
    public FrameBufferObject(GL gl)
    {
        _disposedValue = false;
        FrameBufferObjectHandle = gl.GenFramebuffer();
    }

    public void Load(GL gl)
    {
        BindBy(gl);
    }
    public void BindBy(GL gl) => gl.BindFramebuffer(GLEnum.Framebuffer,FrameBufferObjectHandle);
    private void OnDispose(GL gl) => gl.DeleteFramebuffer(FrameBufferObjectHandle);
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

    public void DisposeBy(GL gl)
    {
        Dispose(disposing: true, gl);
    }
}