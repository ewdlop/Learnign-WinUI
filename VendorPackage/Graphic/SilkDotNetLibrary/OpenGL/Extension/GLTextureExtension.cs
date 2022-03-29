using Silk.NET.OpenGL;
using System;

namespace SilkDotNetLibrary.OpenGL.Extension;

public static class GLTextureExtension
{
    public static void BindTexture(this GL gl, uint textureHandle, TextureUnit textureUnit = TextureUnit.Texture0)
    {
        gl.ActiveTexture(textureUnit);
        gl.BindTexture(TextureTarget.Texture2D, textureHandle);
    }

    //public unsafe static uint GenTexture(this GL gl, in string imagePath)
    //{
    //    uint texturehandle = gl.GenTexture();
    //    //Loading an image using imagesharp.
    //    Image<Rgba32> image = (Image<Rgba32>)Image.Load(imagePath);
    //    image.Mutate(x => x.Flip(FlipMode.Vertical));
    //    uint imageWidth = (uint)image.Width;
    //    uint imageHeight = (uint)image.Height;
    //    //// OpenGL has image origin in the bottom-left corner.
    //    fixed (void* data = &MemoryMarshal.GetReference(image.GetPixelRowSpan(0)))
    //    {
    //        //Loading the actual image.
    //        gl.LoadTexture(data, imageWidth, imageHeight, textureHandle);
    //    }

    //    //Deleting the img from imagesharp.
    //    image.Dispose();

    //    return texturehandle;
    //}

    private static unsafe uint GenTexture(this GL gl, ReadOnlySpan<byte> data, in uint width, in uint height)
    {
        uint textureHandle = gl.GenTexture();
        fixed (void* d = &data[0])
        {
            gl.LoadTexture(d, width, height, textureHandle);
        }
        return textureHandle;
    }

    private static unsafe void LoadTexture(this GL gl, void* data, in uint width, in uint height, in uint texturehandle)
    {
        gl.BindTexture(texturehandle);
        gl.TexImage2D(GLEnum.Texture2D,
                       0,
                       (int)InternalFormat.Rgba,
                       width,
                       height,
                       0,
                       PixelFormat.Rgba,
                       PixelType.UnsignedByte,
                       data);
        //Setting some texture perameters so the texture behaves as expected.
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

        //Generating mipmaps.
        gl.GenerateMipmap(TextureTarget.Texture2D);
    }
}