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
    //public int* BonesID = stackalloc int*[5];
    //public Span<int> BonesID = stackalloc int*[5];
}
