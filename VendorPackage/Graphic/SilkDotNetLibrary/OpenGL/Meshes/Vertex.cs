using System.Numerics;

namespace SilkDotNetLibrary.OpenGL.Meshes;

//https://learnopengl.com/code_viewer_gh.php?code=includes/learnopengl/model.h

public struct Vertex
{
    public Vector3 Position { get; init; }
    public Vector3 Normal { get; init; }
    public Vector2 TexCoords { get; init; }
    public Vector3 Tangent { get; init; }
    public Vector3 BiTangent { get; init; }
    //public int* BonesID = stackalloc int*[5];
    //public Span<int> BonesID = stackalloc int*[5];
}
