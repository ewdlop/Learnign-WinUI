using Silk.NET.OpenGL;
using SilkDotNetLibrary.OpenGL.Buffers;
using System;
using System.Collections.Generic;

namespace SilkDotNetLibrary.OpenGL.Tests;

public class Mesh : IDisposable
{
    public float[] Vertices { get; private set; }
    public uint[] Indices { get; private set; }
    public IReadOnlyList<Texture> Textures { get; private set; }
    public VertexArrayBufferObject<float, uint> VAO { get; set; }
    public BufferObject<float> VBO { get; set; }
    public BufferObject<uint> EBO { get; set; }

    public Mesh(GL gl, float[] vertices, uint[] indices, List<Texture> textures)
    {
        Vertices = vertices;
        Indices = indices;
        Textures = textures;
        SetupMesh(gl);
    }

    public unsafe void SetupMesh(GL gl)
    {
        EBO = new BufferObject<uint>(gl, Indices, BufferTargetARB.ElementArrayBuffer);
        VBO = new BufferObject<float>(gl, Vertices, BufferTargetARB.ArrayBuffer);
        VAO = new VertexArrayBufferObject<float, uint>(gl, VBO, EBO);
        VAO.VertexAttributePointer(gl,0, 3, VertexAttribPointerType.Float, 5, 0);
        VAO.VertexAttributePointer(gl,1, 2, VertexAttribPointerType.Float, 5, 3);
    }

    public void BindBy(GL gl)
    {
        VAO.BindBy(gl);
    }
    
    public void Dispose()
    {
        Textures = null;

    }
    
    public void DisposeBy(GL gl)
    {
        VAO.DisposeBy(gl);
        VBO.DisposeBy(gl);
        EBO.DisposeBy(gl);
    }
}