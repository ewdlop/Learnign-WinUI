using SharedLibrary.Cameras;
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Numerics;
using Texture = SilkDotNetLibrary.OpenGL.Textures.Texture;

namespace SilkDotNetLibrary.OpenGL.Meshes;

public readonly struct MeshComponent//model is an entity? class?
{
    public List<(Mesh, List<Texture>)> Meshes { get; init; }
    //public Texture[] Textures { get; init; }
    public string Directory { get; init; }
    public bool GammaCoorection { get; init; }

    public void Draw(GL gl, SilkDotNetLibrary.OpenGL.Shaders.Shader shader, ICamera camera, Vector3 lampPosition)
    {
        foreach ((Mesh mesh, List<Texture> textures) in Meshes)
        {
            shader.UseBy(gl);
            //var diffuseColor = new Vector3(1f);
            //var ambientColor = diffuseColor * new Vector3(1);
            shader.SetUniformBy(gl, "uModel", Matrix4x4.Identity);/* * Matrix4x4.CreateTranslation(new Vector3(0f, -1 * Time, 0f))*/
            shader.SetUniformBy(gl, "uView", camera.GetViewMatrix());
            shader.SetUniformBy(gl, "uProjection", camera.GetProjectionMatrix());
            //shaders[i].SetUniformBy(gl, "light.ambient", ambientColor);
            //shaders[i].SetUniformBy(gl, "light.diffuse", diffuseColor); // darkened
            //shaders[i].SetUniformBy(gl, "light.position", lampPosition);
            mesh.Draw(gl, shader, textures);
        }
    }
    
    public void Draw(GL gl, ReadOnlySpan<SilkDotNetLibrary.OpenGL.Shaders.Shader> shaders, ICamera camera, Vector3 lampPosition)
    {
        //Console.WriteLine("=== FORCING VISIBLE RENDERING ===");

        //// Disable depth test temporarily
        //bool depthWasEnabled = gl.IsEnabled(EnableCap.DepthTest);
        //gl.Disable(EnableCap.DepthTest);

        //// Disable face culling
        //bool cullWasEnabled = gl.IsEnabled(EnableCap.CullFace);
        //gl.Disable(EnableCap.CullFace);

        //// Set a contrasting clear color
        //gl.ClearColor(0.1f, 0.1f, 0.1f, 1.0f); // Dark gray
        //gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


        int i = 0;
        foreach((Mesh mesh, List<Texture> textures) in Meshes)
        {
            shaders[i].UseBy(gl);
            //var diffuseColor = new Vector3(1f);
            //var ambientColor = diffuseColor * new Vector3(1);
            var scale = Matrix4x4.CreateScale(100); // 10x bigger
            shaders[i].SetUniformBy(gl, "uModel", scale);/* * Matrix4x4.CreateTranslation(new Vector3(0f, -1 * Time, 0f))*/
            shaders[i].SetUniformBy(gl, "uView", camera.GetViewMatrix());
            shaders[i].SetUniformBy(gl, "uProjection", camera.GetProjectionMatrix());
            //shaders[i].SetUniformBy(gl, "light.ambient", ambientColor);
            //shaders[i].SetUniformBy(gl, "light.diffuse", diffuseColor); // darkened
            //shaders[i].SetUniformBy(gl, "light.position", lampPosition);
            mesh.Draw(gl, shaders[i++], textures);

            // 4. Check if anything was drawn
            unsafe
            {

                var pixelData = new byte[4];
                fixed (byte* pixelDataPtr = pixelData)
                {
                    gl.ReadPixels(500 / 2, 400 / 2, 1, 1, GLEnum.Rgba, GLEnum.UnsignedByte, pixelDataPtr);

                    Console.WriteLine($"Center pixel after draw: R={pixelData[0]}, G={pixelData[1]}, B={pixelData[2]}");
                }
            }
        }


    }

    public void DrawWithoutTexture(GL gl, ReadOnlySpan<SilkDotNetLibrary.OpenGL.Shaders.Shader> shaders, ICamera camera, Vector3 lampPosition)
    {
        int i = 0;
        foreach ((Mesh mesh, List<Texture> textures) in Meshes)
        {
            shaders[i].UseBy(gl);
            //var diffuseColor = new Vector3(1f);
            //var ambientColor = diffuseColor * new Vector3(1);
            shaders[i].SetUniformBy(gl, "uModel", Matrix4x4.Identity);
            shaders[i].SetUniformBy(gl, "uView", camera.GetViewMatrix());
            shaders[i].SetUniformBy(gl, "uProjection", camera.GetProjectionMatrix());
            //shaders[i].SetUniformBy(gl, "light.ambient", ambientColor);
            //shaders[i].SetUniformBy(gl, "light.diffuse", diffuseColor); // darkened
            //shaders[i].SetUniformBy(gl, "light.position", lampPosition);
            mesh.Draw(gl);
            i++;
        }
    }
}

//public unsafe ref struct RefVertex
//{
//    public Vector3 Position { get; init; }
//    public Vector3 Normal { get; init; }
//    public Vector2 TexCoords { get; init; }
//    public Vector3 Tangent { get; init; }
//    public Vector3 BiTangent { get; init; }
//    public Span<int> BonesID { get; init; } = stackalloc int[5];
//    public Span<float> Weights { get; init; } = stackalloc float[5];
//}


//public static class MeshExensions
//{
//    public static (List<T1>, List<T2>) AddRange<T1, T2>(this (List<T1>, List<T2>) x, (List<T1>, List<T2>) y)
//    {
//        x.Item1.AddRange(y.Item1);
//        x.Item2.AddRange(y.Item2);
//        return x;
//    }
//}

////whhhyyy??????????????
////public static class X<T> where T : unmanaged
////{
////    public static T[] ToAppendedArray<T>(this ReadOnlySpan<T> array, T element)
////    {
////        Span<T> newArray = stackalloc T[array.Length + 1];
////        array.CopyTo(newArray);
////        newArray[array.Length] = element;
////        return newArray.ToArray();
////    }
////}
//public static class X//<T>where T : unmanaged
//{
//    public static T[] ToAppendedArray<T>(this ReadOnlySpan<T> array, T element) where T : unmanaged
//    {
//        Span<T> newArray = stackalloc T[array.Length + 1];
//        array.CopyTo(newArray);
//        newArray[array.Length] = element;
//        return newArray.ToArray();
//    }
//    public static T[] ConvertToAppendedArray<T>(ReadOnlySpan<T> array, ReadOnlySpan<T> element) where T : unmanaged
//    {
//        Span<T> newArray = stackalloc T[array.Length + element.Length];
//        array.CopyTo(newArray);
//        for (int i = 0; i < element.Length; i++)
//        {
//            newArray[i + array.Length] = element[i];
//        }
//        return newArray.ToArray();
//    }
//    //public static T[] ToAppendedArray2<T>(T[] array, T[] element) where T : unmanaged
//    //{
//    //    return ConvertToAppendedArray(array.AsSpan(), element.AsSpan());
//    //}
//    public static void a(out Span<int> x)
//    {
//        x = new Span<int>(new int[]{1});
//    }
//}