using System;
using Silk.NET.OpenGL;

namespace SilkDotNetLibrary.OpenGL.Textures;

public unsafe struct FrameBufferTexture
{
    private bool _disposedValue;
    public uint FrameBufferTextureHandle { get; }

    public FrameBufferTexture(GL gl)
    {
        _disposedValue = false;
        FrameBufferTextureHandle = gl.GenTexture();
        BindBy(gl);
    }

    public void BindBy(GL gl) => gl.BindTexture(GLEnum.Texture2D, FrameBufferTextureHandle);

    public void Load(GL gl,uint width,uint height)
    {
        gl.TexImage2D(GLEnum.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba,
                       PixelType.UnsignedByte,null);
        gl.TexParameter(GLEnum.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
        gl.TexParameter(GLEnum.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
        gl.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.ColorAttachment0, GLEnum.Texture2D, FrameBufferTextureHandle,
            0);
    }
    private void OnDispose(GL gl) => gl.DeleteTexture(FrameBufferTextureHandle);

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
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true, gl);
        GC.SuppressFinalize(this);
    }
}