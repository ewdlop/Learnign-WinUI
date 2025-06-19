using System;
using System.Runtime.InteropServices;
using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SilkDotNetLibrary.OpenGL.Textures;

public readonly struct CubeMapTexture
{
    public uint TextureHandle { get; }
    /// <summary>
    /// Texture is flipped automatically
    /// </summary>
    /// <param name="gl"></param>
    /// <param name="imagePath"></param>
    /// <param name="textureType"></param>
    public unsafe CubeMapTexture(GL gl, Image[] images, TextureType textureType = default)
    {
        try
        {
            TextureHandle = gl.GenTexture();
            BindBy(gl);

            for (uint i = 0; i < images.Length; i++)
            {
                Image<Rgba32> imag32 = (Image<Rgba32>)images[i];
                //// OpenGL has image origin in the bottom-left corner
                CubeMapTexture tmpThis = this;
                imag32.ProcessPixelRows(accessor =>
                {
                    fixed (void* data = &MemoryMarshal.GetReference(accessor.GetRowSpan(0)))
                    {
                        tmpThis.Load(gl, i, data, (uint)images[i].Width, (uint)images[i].Height);
                    }
                });
            }

            //Setting some texture parameters so the texture behaves as expected.
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapR, (int)GLEnum.ClampToEdge);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

            //Deleting the img from imagesharp.
            //image.Dispose();
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
            throw;
        }
    }

    private unsafe void Load(GL gl,
                             uint index,
                             void* data,
                             uint width,
                             uint height,
                             InternalFormat internalForamt = InternalFormat.Rgb,
                             PixelFormat pixelFormat = PixelFormat.Rgb,
                             PixelType pixelType = PixelType.UnsignedByte)
    {
        try
        {
            uint TextureTarget = (uint)GLEnum.TextureCubeMapPositiveX + index;
            GLEnum textureTargetEnum = (GLEnum)TextureTarget;
            gl.TexImage2D(textureTargetEnum,
                0,
                internalForamt,
                width,
                height,
                0,
                pixelFormat,
                pixelType,
                data);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }

    public void BindBy(GL gl, TextureUnit textureUnit = TextureUnit.Texture0)
    {
        //When we bind a texture we can choose which textureslot we can bind it to.
        //gl.ActiveTexture(textureUnit);
        gl.BindTexture(TextureTarget.TextureCubeMap, TextureHandle);
    }

    private void OnDispose(GL gl) => gl.DeleteTexture(TextureHandle);

    public void DisposeBy(in GL gl)
    {
        OnDispose(gl);
    }
}