using System;
using Silk.NET.OpenGL;

namespace SilkDotNetLibrary.OpenGL.Textures;

public unsafe readonly struct FrameBufferTexture
{
    public uint FrameBufferTextureHandle { get; }
    public FrameBufferTexture(GL gl, uint width, uint height)
    {
        FrameBufferTextureHandle = gl.GenTexture();
        Load(gl, width, height);
    }

    public void BindBy(GL gl, TextureUnit textureUnit = TextureUnit.Texture0)
    {
        gl.ActiveTexture(textureUnit);
        gl.BindTexture(GLEnum.Texture2D, FrameBufferTextureHandle);
    }


    public void Load(GL gl,uint width,uint height)
    {
        BindBy(gl);
        gl.TexImage2D(GLEnum.Texture2D, 0, (int)InternalFormat.Rgba, width, height, 0, PixelFormat.Rgba,
                       PixelType.UnsignedByte,null);
        gl.TexParameter(GLEnum.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
        gl.TexParameter(GLEnum.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
        gl.FramebufferTexture2D(GLEnum.Framebuffer, GLEnum.ColorAttachment0, GLEnum.Texture2D, FrameBufferTextureHandle, 0);
    }
    private void OnDispose(GL gl) => gl.DeleteTexture(FrameBufferTextureHandle);

    public void DisposeBy(GL gl)
    {
        OnDispose(gl);
    }
}