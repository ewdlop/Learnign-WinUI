using System.Numerics;

namespace SharedLibrary.Transforms
{
    public struct CameraTransform
    {
        public Vector3 CameraPosition { get; set; } = new Vector3(0.0f, 0.0f, 3.0f);
        public Vector3 CameraFront { get; set; }  = new Vector3(0.0f, 0.0f, -1.0f);
        public Vector3 CameraUp { get; set; }  = Vector3.UnitY;
        public Vector3 CameraDirection { get; set; }  = Vector3.Zero;
        public float CameraYaw { get; set; }  = -90f;
        public float CameraPitch { get; set; }  = 0f;
        public float CameraZoom { get; set; }  = 45f;
    }

    public class Camera
    {
        private readonly CameraTransform _cameraTransform;
        //private reado
        //public unsafe void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
        //{
        //    //We don't want to be able to zoom in too close or too far away so clamp to these values
        //    CameraZoom = Math.Clamp(CameraZoom - scrollWheel.Y, 1.0f, 45f);
        //}
    }
}
