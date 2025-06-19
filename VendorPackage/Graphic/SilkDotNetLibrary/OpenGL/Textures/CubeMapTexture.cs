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
    /// Texture is handled automatically with proper format detection
    /// </summary>
    /// <param name="gl"></param>
    /// <param name="images">Array of 6 images in order: right, left, top, bottom, front, back</param>
    /// <param name="textureType"></param>
    public unsafe CubeMapTexture(GL gl, Image[] images, TextureType textureType = default)
    {
        try
        {
            TextureHandle = gl.GenTexture();
            BindBy(gl);

            for (uint i = 0; i < images.Length; i++)
            {
                LoadCubeFace(gl, i, images[i]);
            }

            // Setting cube map specific texture parameters
            gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
            gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
            gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)GLEnum.ClampToEdge);
            gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
            gl.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CubeMapTexture Error: {ex}");
            throw;
        }
    }

    private unsafe void LoadCubeFace(GL gl, uint faceIndex, Image image)
    {
        uint width = (uint)image.Width;
        uint height = (uint)image.Height;
        uint textureTarget = (uint)GLEnum.TextureCubeMapPositiveX + faceIndex;
        GLEnum target = (GLEnum)textureTarget;

        // Auto-detect format based on pixel format
        switch (image.PixelType.BitsPerPixel)
        {
            case 24: // RGB
                {
                    Image<Rgb24> img24 = image.CloneAs<Rgb24>();
                    img24.ProcessPixelRows(accessor =>
                    {
                        fixed (void* data = &MemoryMarshal.GetReference(accessor.GetRowSpan(0)))
                        {
                            gl.TexImage2D(target, 0, InternalFormat.Rgb, width, height, 0, 
                                        PixelFormat.Rgb, PixelType.UnsignedByte, data);
                        }
                    });
                    img24.Dispose();
                    break;
                }
            case 32: // RGBA
                {
                    Image<Rgba32> img32 = image.CloneAs<Rgba32>();
                    img32.ProcessPixelRows(accessor =>
                    {
                        fixed (void* data = &MemoryMarshal.GetReference(accessor.GetRowSpan(0)))
                        {
                            gl.TexImage2D(target, 0, InternalFormat.Rgba, width, height, 0, 
                                        PixelFormat.Rgba, PixelType.UnsignedByte, data);
                        }
                    });
                    img32.Dispose();
                    break;
                }
            case 8: // Grayscale
                {
                    Image<L8> img8 = image.CloneAs<L8>();
                    img8.ProcessPixelRows(accessor =>
                    {
                        fixed (void* data = &MemoryMarshal.GetReference(accessor.GetRowSpan(0)))
                        {
                            gl.TexImage2D(target, 0, InternalFormat.Red, width, height, 0, 
                                        PixelFormat.Red, PixelType.UnsignedByte, data);
                        }
                    });
                    img8.Dispose();
                    break;
                }
            case 16: // Grayscale + Alpha
                {
                    Image<La16> img16 = image.CloneAs<La16>();
                    img16.ProcessPixelRows(accessor =>
                    {
                        fixed (void* data = &MemoryMarshal.GetReference(accessor.GetRowSpan(0)))
                        {
                            gl.TexImage2D(target, 0, InternalFormat.RG, width, height, 0, 
                                        PixelFormat.RG, PixelType.UnsignedByte, data);
                        }
                    });
                    img16.Dispose();
                    break;
                }
            default:
                {
                    // Fallback: Convert to RGBA32 for any unsupported format
                    Console.WriteLine($"Warning: Unsupported format {image.PixelType.BitsPerPixel} bits, converting to RGBA32");
                    Image<Rgba32> imgFallback = image.CloneAs<Rgba32>();
                    imgFallback.ProcessPixelRows(accessor =>
                    {
                        fixed (void* data = &MemoryMarshal.GetReference(accessor.GetRowSpan(0)))
                        {
                            gl.TexImage2D(target, 0, InternalFormat.Rgba, width, height, 0, 
                                        PixelFormat.Rgba, PixelType.UnsignedByte, data);
                        }
                    });
                    imgFallback.Dispose();
                    break;
                }
        }

        // Check for OpenGL errors after loading each face
        var error = gl.GetError();
        if (error != GLEnum.NoError)
        {
            throw new Exception($"OpenGL Error loading cube face {faceIndex}: {error}");
        }
    }

    public void BindBy(GL gl, TextureUnit textureUnit = TextureUnit.Texture0)
    {
        gl.ActiveTexture(textureUnit);
        gl.BindTexture(TextureTarget.TextureCubeMap, TextureHandle);
    }

    private void OnDispose(GL gl) => gl.DeleteTexture(TextureHandle);

    public void DisposeBy(in GL gl)
    {
        OnDispose(gl);
    }
}