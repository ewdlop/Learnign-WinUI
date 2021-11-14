using SharedLibrary.Event.Listener;
using System.Numerics;

namespace SharedLibrary.Cameras
{
    public interface ICamera : ICameraMoveable, IMouseEventListener, IKeyBoardEventListner
    {
        public Vector3 CameraPosition { get; }
        public Vector3 CameraFront { get; }
        public Vector3 CameraUp { get; }
        public Vector3 CameraDirection { get; }
        public float CameraYaw { get; }
        public float CameraPitch { get; }
        public float CameraZoom { get; }
    }
}
