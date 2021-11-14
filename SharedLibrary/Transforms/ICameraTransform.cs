using System.Numerics;

namespace SharedLibrary.Transforms;

public interface ICameraTransform : IReadOnlyCameraTransform
{
    public void SetCameraPosition(Vector3 position);
    public void SetCameraFront(Vector3 front);
    public void SetCameraUp(Vector3 up);
    public void SetCameraDirection(Vector3 direction);
    public float SetCameraYaw(float yaw);
    public float SetCameraPitch(float pitch);
    public float SetCameraZoom(float zoom);

}
