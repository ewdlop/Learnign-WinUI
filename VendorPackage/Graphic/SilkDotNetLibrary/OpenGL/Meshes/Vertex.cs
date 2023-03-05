using System.Numerics;
using System.Runtime.InteropServices;

namespace SilkDotNetLibrary.OpenGL.Meshes;

//https://learnopengl.com/code_viewer_gh.php?code=includes/learnopengl/model.h

/// <summary>
/// Is not using property for now. Marshal.OffsetOf is not working with properties.
/// </summary>
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
    //public int* BonesID = stackalloc int*[5];
    //public Span<int> BonesID = stackalloc int*[5];
}

public record struct Vertex2
{
    public Vector3 Position;
    public Vector3 Normal;
    public Vector2 UV;
    public Vector3 Color;
    //public int* BonesID = stackalloc int*[5];
    //public Span<int> BonesID = stackalloc int*[5];
}