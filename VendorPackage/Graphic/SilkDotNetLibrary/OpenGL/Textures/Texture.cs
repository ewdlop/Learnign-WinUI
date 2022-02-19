using System;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace SilkDotNetLibrary.OpenGL.Textures;

public struct Texture
{
    private bool disposedValue;
    public uint Texturehandle { get; }
    public unsafe Texture(GL gl, in string imagePath)
    {
        disposedValue = false;
        Texturehandle = gl.GenTexture();
        //Loading an image using imagesharp.
        Image<Rgba32> image = (Image<Rgba32>)Image.Load(imagePath);
        image.Mutate(x => x.Flip(FlipMode.Vertical));
        uint imageWidth = (uint)image.Width;
        uint imageHeight = (uint)image.Height;
        //// OpenGL has image origin in the bottom-left corner
        Texture tmpThis = this;
        image.ProcessPixelRows(accessor =>
        {
            fixed (void* data = &MemoryMarshal.GetReference(accessor.GetRowSpan(0)))
            {
                //Loading the actual image.
                tmpThis.Load(gl, data, imageWidth, imageHeight);
            }
        });

        //Deleting the img from imagesharp.
        image.Dispose();
    }

    public unsafe Texture(in GL gl, Span<byte> data, in uint width, in uint height)
    {
        disposedValue = false;
        Texturehandle = gl.GenTexture();
        fixed (void* d = &data[0])
        {
            Load(gl, d, width, height);
        }
    }

    private unsafe void Load(in GL gl, void* data, in uint width, in uint height)
    {
        BindBy(gl);
        gl.TexImage2D(GLEnum.Texture2D,
                       0,
                       (int)InternalFormat.Rgba,
                       width,
                       height,
                       0,
                       PixelFormat.Rgba,
                       PixelType.UnsignedByte,
                       data);
        //Setting some texture parameters so the texture behaves as expected.
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

        //Generating mipmaps.
        gl.GenerateMipmap(TextureTarget.Texture2D);
    }

    public void BindBy(in GL gl, TextureUnit textureUnit = TextureUnit.Texture0)
    {
        //When we bind a texture we can choose which textureslot we can bind it to.
        gl.ActiveTexture(textureUnit);
        gl.BindTexture(TextureTarget.Texture2D, Texturehandle);
    }
    private void OnDispose(in GL gl) => gl.DeleteTexture(Texturehandle);

    private void Dispose(bool disposing, in GL gl)
    {
        if (disposedValue) return;
        if (disposing)
        {
            OnDispose(gl);
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Texture()
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
