﻿using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace SilkDotNetLibrary.OpenGL.Tests;

[StructLayout(LayoutKind.Sequential)]
public record struct Vertex
{
    public Vector3 Position;
    public Vector3 Normal;
    public Vector2 TexCoords;
    public Vector3 Tangent;
    public Vector3 BiTangent;
    public Vector3 color;

    public const uint PositionOffset = 0;
    public const uint NormalOffset = 12;
    public const uint UvOffset = 24;
    public const uint ColorOffset = 32;

    public const int MAX_BONE_INFLUENCE = 4;

    public Memory<int> BoneIds = Array.Empty<int>();
    public Memory<float> Weights = Array.Empty<float>();

    public Vertex()
    {
    }
}