using System.Numerics;

namespace SharedLibrary.Transforms;

public interface ICameraTransform : IReadOnlyCameraTransform
{
    void SetCameraPosition(Vector3 position);
    void SetCameraFront(Vector3 front);
    void SetCameraUp(Vector3 up);
    void SetCameraDirection(Vector3 direction);
    void SetCameraYaw(float yaw);
    void SetCameraPitch(float pitch);
    void SetCameraZoom(float zoom);

}
