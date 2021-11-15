using SharedLibrary.Math;
using System;
using System.Numerics;

namespace SharedLibrary.Transforms
{
    public struct CameraTransform : ICameraTransform
    {
        public Vector3 CameraPosition { get; set; } = new Vector3(0.0f, 0.0f, 3.0f);
        public Vector3 CameraFront { get; set; } = new Vector3(0.0f, 0.0f, -1.0f);
        public Vector3 CameraUp { get; set; } = Vector3.UnitY;
        public Vector3 CameraDirection { get; set; } = Vector3.Zero;
        public float CameraYaw { get; set; } = -90f;
        public float CameraPitch { get; set; } = 0f;
        public float CameraZoom { get; set; } = 45f;

        public void MoveForward(float speed)
        {
            CameraPosition += speed * CameraFront;
        }

        public void MoveBackward(float speed)
        {
            CameraPosition -= speed * CameraFront;
        }

        public void MoveLeft(float speed)
        {
            CameraPosition -= Vector3.Normalize(Vector3.Cross(CameraFront, CameraUp)) * speed;
        }

        public void MoveRight(float speed)
        {
            CameraPosition += Vector3.Normalize(Vector3.Cross(CameraFront, CameraUp)) * speed;
        }

        public void RotateYaw(float angle)
        {
            CameraYaw += angle;
        }

        public void RotatePitch(float angle)
        {
            CameraPitch = System.Math.Clamp(CameraPitch + angle, -89.0f, 89.0f); ;
        }

        public void SetDirection()
        {
            float CameraDirectionX = MathF.Cos(MathHelper.DegreesToRadians(CameraYaw)) * MathF.Cos(MathHelper.DegreesToRadians(CameraPitch));
            float CameraDirectionY = MathF.Sin(MathHelper.DegreesToRadians(CameraPitch));
            float CameraDirectionZ = MathF.Sin(MathHelper.DegreesToRadians(CameraYaw)) * MathF.Cos(MathHelper.DegreesToRadians(CameraPitch));
            Vector3 cameraDirection = new()
            {
                X = CameraDirectionX,
                Y = CameraDirectionY,
                Z = CameraDirectionZ
            };
            CameraDirection = cameraDirection;
            CameraFront = Vector3.Normalize(CameraDirection);
        }

        public void ZoomIn(float speed) => CameraZoom = System.Math.Clamp(CameraZoom + speed, 1.0f, 45f);
    }
}
