using System.Numerics;

namespace SharedLibrary.Transforms
{
    public class CameraTransform
    {
        public Vector3 CameraPosition { get; internal set; } = new Vector3(0.0f, 0.0f, 3.0f);
        public Vector3 CameraFront { get; internal set; }  = new Vector3(0.0f, 0.0f, -1.0f);
        public Vector3 CameraUp { get; internal set; }  = Vector3.UnitY;
        public Vector3 CameraDirection { get; internal set; }  = Vector3.Zero;
        public float CameraYaw { get; internal set; }  = -90f;
        public float CameraPitch { get; internal set; }  = 0f;
        public float CameraZoom { get; internal set; }  = 45f;
    }
}
