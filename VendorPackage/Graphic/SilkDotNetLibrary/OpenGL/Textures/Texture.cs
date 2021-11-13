using System;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SilkDotNetLibrary.OpenGL.Textures;

public struct Texture : IDisposable
{
    private bool disposedValue;
    private readonly uint _texturehandle;
    private readonly GL _gl;

    public unsafe Texture(GL gl, string imagePath)
    {
        disposedValue = false;
        _gl = gl;
        //Generating the opengl handle;
        _texturehandle = _gl.GenTexture();

        //Loading an image using imagesharp.
        Image<Rgba32> image = (Image<Rgba32>)Image.Load(imagePath);

        //// OpenGL has image origin in the bottom-left corner.
        fixed (void* data = &MemoryMarshal.GetReference(image.GetPixelRowSpan(0)))
        {
            //Loading the actual image.
            Load(data, (uint)image.Width, (uint)image.Height);
        }

        //Deleting the img from imagesharp.
        image.Dispose();
    }

    private unsafe void Load(void* data, uint width, uint height)
    {
        Bind();
        _gl.TexImage2D(GLEnum.Texture2D,
                       0,
                       (int)InternalFormat.Rgba,
                       width,
                       height,
                       0,
                       PixelFormat.Rgba,
                       PixelType.UnsignedByte,
                       data);
        //Setting some texture perameters so the texture behaves as expected.
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

        //Generating mipmaps.
        _gl.GenerateMipmap(TextureTarget.Texture2D);
    }

    public void Bind(TextureUnit textureUnit = TextureUnit.Texture0)
    {
        //When we bind a texture we can choose which textureslot we can bind it to.
        _gl.ActiveTexture(textureUnit);
        _gl.BindTexture(TextureTarget.Texture2D, _texturehandle);
    }
    private void OnDispose() => _gl.DeleteTexture(_texturehandle);

    private void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                OnDispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Texture()
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
