using SharedLibrary.Event.Handler;
using SharedLibrary.Event.Listener;
using SharedLibrary.Math;
using SharedLibrary.Transforms;
using System;
using System.Numerics;

namespace SharedLibrary.Cameras
{
    public class Camera : ICamera, IDisposable
    {
        private float _lookSensitivity = 0.1f;
        private bool disposedValue;
        private readonly ICameraTransform _cameraTransform;
        private readonly IEventHandler _eventHandler;
        public Vector3 Position => _cameraTransform.CameraPosition;
        public Vector3 Front => _cameraTransform.CameraFront;
        public Vector3 Up => _cameraTransform.CameraUp;
        public Vector3 Direction => _cameraTransform.CameraDirection;
        public float Yaw => _cameraTransform.CameraYaw;
        public float Pitch => _cameraTransform.CameraPitch;
        public float Zoom => _cameraTransform.CameraZoom;
        public float Speed { get; private set; } = 5.0f;
        public float AspectRatio { get; private set; } = 8f / 6;
        public Camera(IEventHandler eventHandler)
        {
            disposedValue = false;
            _cameraTransform = new CameraTransform();
            _eventHandler = eventHandler;
            //_eventHandler.OnMouseMove += (this as IMouseEventListener).OnMouseMove;
            //_eventHandler.OnMouseScrollWheel += (this as IMouseEventListener).OnMouseWheel;
            _eventHandler.OnKeyBoardKeyDown += (this as IKeyBoardEventListener).OnKeyBoardKeyDown;
        }

        void IMouseEventListener.OnMouseMove(object sender, (Vector2 Position, Vector2 LastMousePosition) e)
        {
            var xOffset = (e.Position.X - e.LastMousePosition.X) * _lookSensitivity;
            var yOffset = (e.Position.Y - e.LastMousePosition.Y) * _lookSensitivity;

            _cameraTransform.RotateYaw(xOffset);
            _cameraTransform.RotatePitch(-1 * yOffset);
            _cameraTransform.SetDirection();
        }

        void IMouseEventListener.OnMouseWheel(object sender, (float x, float y) e)
        {
            _cameraTransform.ZoomIn(-1.0f * e.y);
        }

        void IKeyBoardEventListener.OnKeyBoardKeyDown(object sender, string keyCode)
        {
            switch (keyCode)
            {
                case "W":
                    _cameraTransform.MoveForward(Speed * 0.01f);
                    break;
                case "S":
                    _cameraTransform.MoveBackward(Speed * 0.01f);
                    break;
                case "A":
                    _cameraTransform.MoveLeft(Speed * 0.01f);
                    break;
                case "D":
                    _cameraTransform.MoveRight(Speed * 0.01f);
                    break;
            }
        }
        public void Move() => throw new NotImplementedException();

        public void Reset() => throw new NotImplementedException();

        public Matrix4x4 GetViewMatrix()
        {
            return Matrix4x4.CreateLookAt(Position, Position + Front, Up);
        }

        public Matrix4x4 GetProjectionMatrix()
        {
            return Matrix4x4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(Zoom), AspectRatio, 0.1f, 100.0f);
        }

        private void OnDipose()
        {
            _eventHandler.OnMouseMove -= (this as IMouseEventListener).OnMouseMove;
            _eventHandler.OnMouseScrollWheel -= (this as IMouseEventListener).OnMouseWheel;
            _eventHandler.OnKeyBoardKeyDown -= (this as IKeyBoardEventListener).OnKeyBoardKeyDown;
            //_cameraTransform.Dispose();
        }
        private void Dispose(bool disposing)
        {
            if (disposedValue) return;
            if (disposing)
            {
                OnDipose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            disposedValue = true;
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Camera()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
