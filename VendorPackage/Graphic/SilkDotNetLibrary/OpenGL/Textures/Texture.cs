using System;
using System.Runtime.InteropServices;
using Silk.NET.Assimp;
using Silk.NET.OpenGL;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace SilkDotNetLibrary.OpenGL.Textures;

public readonly struct Texture
{
    public uint TextureHandle { get; }
    public TextureType TextureType { get; } = default;
    /// <summary>
    /// Texture is flipped automatically
    /// </summary>
    /// <param name="gl"></param>
    /// <param name="imagePath"></param>
    /// <param name="textureType"></param>
    public unsafe Texture(GL gl, Image image,TextureType textureType=default)
    {
        try
        {
            TextureHandle = gl.GenTexture();
            TextureType = textureType;
            Texture tmpThis = this;
            uint imageWidth = (uint)image.Width;
            uint imageHeight = (uint)image.Height;
            switch (image.PixelType.BitsPerPixel)
            {
                case 32:
                    {
                        Image<Rgba32> imag32 = (Image<Rgba32>)image;
                        //// OpenGL has image origin in the bottom-left corner
                        imag32.ProcessPixelRows(accessor =>
                        {
                            fixed (void* data = &MemoryMarshal.GetReference(accessor.GetRowSpan(0)))
                            {
                                //Loading the actual image.

                                //Note 8/30/2023
                                //Batman Metalness material breaks here
                                //corrupted memoery
                                tmpThis.Load(gl, data, imageWidth, imageHeight);
                            }
                        });
                        break;
                    }

                case 24:
                    {
                        Image<Rgb24> image24 = (Image<Rgb24>)image;
                        //// OpenGL has image origin in the bottom-left corner
                        image24.ProcessPixelRows(accessor =>
                        {
                            fixed (void* data = &MemoryMarshal.GetReference(accessor.GetRowSpan(0)))
                            {
                                //Loading the actual image.
                                tmpThis.Load24BitTexuture(gl, data, imageWidth, imageHeight, InternalFormat.Rgb8, PixelFormat.Rgb, PixelType.UnsignedByte);
                            }
                        });
                        break;
                    }

                case 48:
                    {
                        Image<Rgb48> image48 = (Image<Rgb48>)image;
                        //// OpenGL has image origin in the bottom-left corner
                        image48.ProcessPixelRows(accessor =>
                        {
                            fixed (void* data = &MemoryMarshal.GetReference(accessor.GetRowSpan(0)))
                            {
                                //Loading the actual image.
                                tmpThis.Load(gl, data, imageWidth, imageHeight, InternalFormat.Rgb8, PixelFormat.Rgb);
                            }
                        });
                        break;
                    }

                case 64:
                    {
                        Image<Rgba64> image64 = (Image<Rgba64>)image;
                        //// OpenGL has image origin in the bottom-left corner
                        image64.ProcessPixelRows(accessor =>
                        {
                            fixed (void* data = &MemoryMarshal.GetReference(accessor.GetRowSpan(0)))
                            {
                                //Loading the actual image.
                                tmpThis.Load(gl, data, imageWidth, imageHeight);
                            }
                        });
                        break;
                    }
            }
            //Deleting the img from imagesharp.
            image.Dispose();
        }
        catch (Exception ex)
        {
            Console.Write(ex.ToString());
            throw;
        }
    }

    public unsafe Texture(GL gl, Span<byte> data, uint width, uint height)
    {
        TextureHandle = gl.GenTexture();
        fixed (void* d = &data[0])
        {
            Load(gl, d, width, height);
        }
    }

    private unsafe void Load24BitTexuture(GL gl,
                         void* data,
                         uint width,
                         uint height,
                         InternalFormat internalForamt = InternalFormat.Rgba,
                         PixelFormat pixelFormat = PixelFormat.Rgba,
                         PixelType pixelType = PixelType.UnsignedByte)
    {
        BindBy(gl);
        try
        {
            gl.TexImage2D(GLEnum.Texture2D,
               0,
               3,
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

        //Setting some texture parameters so the texture behaves as expected.
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

        //Generating mipmaps.
        gl.GenerateMipmap(TextureTarget.Texture2D);
    }


    private unsafe void Load(GL gl,
                             void* data,
                             uint width,
                             uint height,
                             InternalFormat internalForamt = InternalFormat.Rgba,
                             PixelFormat pixelFormat = PixelFormat.Rgba,
                             PixelType pixelType = PixelType.UnsignedByte)
    {
        BindBy(gl);
        try
        {
            gl.TexImage2D(GLEnum.Texture2D,
               0,
               internalForamt,
               width,
               height,
               0,
               pixelFormat,
               pixelType,
               data);
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        //Setting some texture parameters so the texture behaves as expected.
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
        gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);

        //Generating mipmaps.
        gl.GenerateMipmap(TextureTarget.Texture2D);
    }

    public void BindBy(GL gl, TextureUnit textureUnit = TextureUnit.Texture0)
    {
        //When we bind a texture we can choose which textureslot we can bind it to.
        gl.ActiveTexture(textureUnit);
        gl.BindTexture(TextureTarget.Texture2D, TextureHandle);
    }

    private void OnDispose(GL gl) => gl.DeleteTexture(TextureHandle);

    public void DisposeBy(in GL gl)
    {
        OnDispose(gl);
    }
}