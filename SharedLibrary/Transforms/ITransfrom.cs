using System.Numerics;

namespace SharedLibrary.Transforms;

public interface ITransfrom
{
    Vector3 Position { get; }

    float Scale { get; }

    Quaternion Rotation { get;}

    Matrix4x4 ViewMatrix { get; }
}
