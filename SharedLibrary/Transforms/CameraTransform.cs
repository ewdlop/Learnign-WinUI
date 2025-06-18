using SharedLibrary.Math;
using System;
using System.Numerics;

namespace SharedLibrary.Transforms
{
    public class CameraTransform : ICameraTransform
    {
        public Vector3 CameraPosition { get; private set; } = Vector3.UnitZ * 6;
        public Vector3 CameraFront { get; private set; } = Vector3.UnitZ * -1;
        public Vector3 CameraUp { get; private set; } = Vector3.UnitY;
        public Vector3 CameraDirection { get; private set; } = Vector3.Zero;
        public float CameraYaw { get; private set; } = -90f;
        public float CameraPitch { get; private set; } = 0f;
        public float CameraZoom { get; private set; } = 45f;

        public void MoveUp(float speed)
        {
            CameraPosition += speed * CameraUp;
        }

        public void MoveDown(float speed)
        {
            CameraPosition -= speed * CameraUp;
        }

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
