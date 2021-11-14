using SharedLibrary.Event.EventArgs;
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
        private readonly CameraTransform _cameraTransform;
        private readonly IEventHandler _eventHandler;
        public Vector3 CameraPosition => _cameraTransform.CameraPosition;
        public Vector3 CameraFront => _cameraTransform.CameraFront;
        public Vector3 CameraUp => _cameraTransform.CameraUp;
        public Vector3 CameraDirection => _cameraTransform.CameraDirection;
        public float CameraYaw => _cameraTransform.CameraYaw;
        public float CameraPitch => _cameraTransform.CameraPitch;
        public float CameraZoom => _cameraTransform.CameraZoom;
        public Camera(CameraTransform cameraTransform, IEventHandler eventHandler)
        {
            _cameraTransform = cameraTransform;
            _eventHandler = eventHandler;
            _eventHandler.OnMouseMove += (this as IMouseEventListener).OnMouseMove;
            _eventHandler.OnMouseScrollWheel += (this as IMouseEventListener).OnMouseWheel;
        }

        void IMouseEventListener.OnMouseMove(object sender, MouseMoveEventArgs e)
        {

            var xOffset = (e.Position.X - e.LastMousePosition.X) * _lookSensitivity;
            var yOffset = (e.Position.Y - e.LastMousePosition.Y) * _lookSensitivity;

            _cameraTransform.CameraYaw += xOffset;
            _cameraTransform.CameraPitch -= yOffset;

            //We don't want to be able to look behind us by going over our head or under our feet so make sure it stays within these bounds
            _cameraTransform.CameraPitch = System.Math.Clamp(_cameraTransform.CameraPitch, -89.0f, 89.0f);

            float CameraDirectionX = MathF.Cos(MathHelper.DegreesToRadians(_cameraTransform.CameraYaw)) * MathF.Cos(MathHelper.DegreesToRadians(_cameraTransform.CameraPitch));
            float CameraDirectionY = MathF.Sin(MathHelper.DegreesToRadians(_cameraTransform.CameraPitch));
            float CameraDirectionZ = MathF.Sin(MathHelper.DegreesToRadians(_cameraTransform.CameraYaw)) * MathF.Cos(MathHelper.DegreesToRadians(_cameraTransform.CameraPitch));
            Vector3 CameraDirection = new()
            {
                X = CameraDirectionX,
                Y = CameraDirectionY,
                Z = CameraDirectionZ
            };
            _cameraTransform.CameraDirection = CameraDirection;
            _cameraTransform.CameraFront = Vector3.Normalize(CameraDirection);
        }

        void IMouseEventListener.OnMouseWheel(object sender, MouseScrollWheelEventArgs e)
        {
            _cameraTransform.CameraZoom = System.Math.Clamp(_cameraTransform.CameraZoom - e.Y, 1.0f, 45f);
        }
        private void OnDipose()
        {
            _eventHandler.OnMouseMove -= (this as IMouseEventListener).OnMouseMove;
            _eventHandler.OnMouseScrollWheel -= (this as IMouseEventListener).OnMouseWheel;
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
