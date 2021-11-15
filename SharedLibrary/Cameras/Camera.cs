using SharedLibrary.Event.EventArgs;
using SharedLibrary.Event.Handler;
using SharedLibrary.Event.Listener;
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
        public Vector3 CameraPosition => _cameraTransform.CameraPosition;
        public Vector3 CameraFront => _cameraTransform.CameraFront;
        public Vector3 CameraUp => _cameraTransform.CameraUp;
        public Vector3 CameraDirection => _cameraTransform.CameraDirection;
        public float CameraYaw => _cameraTransform.CameraYaw;
        public float CameraPitch => _cameraTransform.CameraPitch;
        public float CameraZoom => _cameraTransform.CameraZoom;
        public float Speed { get; private set; } = 5;

        public Camera(IEventHandler eventHandler)
        {
            disposedValue = false;
            _cameraTransform = new CameraTransform();
            _eventHandler = eventHandler;
            _eventHandler.OnMouseMove += (this as IMouseEventListener).OnMouseMove;
            _eventHandler.OnMouseScrollWheel += (this as IMouseEventListener).OnMouseWheel;
            _eventHandler.OnKeyBoardKeyDown += (this as IKeyBoardEventListner).OnKeyBoardKeyDown;
        }

        void IMouseEventListener.OnMouseMove(object sender, MouseMoveEventArgs e)
        {
            var xOffset = (e.Position.X - e.LastMousePosition.X) * _lookSensitivity;
            var yOffset = -1.0f * (e.Position.Y - e.LastMousePosition.Y) * _lookSensitivity;

            _cameraTransform.RotateYaw(xOffset);
            _cameraTransform.RotatePitch(yOffset);
            _cameraTransform.SetDirection();
        }

        void IMouseEventListener.OnMouseWheel(object sender, MouseScrollWheelEventArgs e)
        {
            _cameraTransform.ZoomIn(-1.0f * e.Y);
        }

        void IKeyBoardEventListner.OnKeyBoardKeyDown(object sender, KeyBoardKeyDownEventArgs e)
        {
            switch (e.KeyCode)
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
                default:
                    break;
            }
        }
        public void Move() => throw new NotImplementedException();

        public void Reset() => throw new NotImplementedException();

        private void OnDipose()
        {
            _eventHandler.OnMouseMove -= (this as IMouseEventListener).OnMouseMove;
            _eventHandler.OnMouseScrollWheel -= (this as IMouseEventListener).OnMouseWheel;
            _eventHandler.OnKeyBoardKeyDown -= (this as IKeyBoardEventListner).OnKeyBoardKeyDown;
        }
        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    OnDipose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Camera()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
