using System.Numerics;

namespace SharedLibrary.Transforms;

public interface ICameraTransform
{
    Vector3 CameraPosition { get; }
    Vector3 CameraFront { get; }
    Vector3 CameraUp { get; }
    Vector3 CameraDirection { get; }
    float CameraYaw { get; }
    float CameraPitch { get; }
    float CameraZoom { get; }
    void MoveForward(float speed);
    void MoveBackward(float speed);
    void MoveLeft(float speed);
    void MoveRight(float speed);
    void RotateYaw(float angle);
    void RotatePitch(float angle);
    void SetDirection();
    void ZoomIn(float speed);
}
