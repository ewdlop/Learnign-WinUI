using SharedLibrary.Event.Listener;
using System.Numerics;

namespace SharedLibrary.Cameras
{
    public interface ICamera : ICameraMoveable, IMouseEventListener, IKeyBoardEventListener
    {
        public Vector3 Position { get; }
        public Vector3 Front { get; }
        public Vector3 Up { get; }
        public Vector3 Direction { get; }
        public float Yaw { get; }
        public float Pitch { get; }
        public float Zoom { get; }

        Matrix4x4 GetProjectionMatrix();
        Matrix4x4 GetViewMatrix();
    }
}
