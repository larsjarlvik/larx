using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx
{
    public enum CameraMoveDirection
    {
        Forward,
        Left,
        Back,
        Right
    }

    public class Camera
    {
        private float cameraDistance = 20.0f;
        private float cameraRotation = 0.0f;

        public float AspectRatio { get; set; }

        public void Move(CameraMoveDirection direction)
        {
            switch (direction)
            {
                case CameraMoveDirection.Forward:
                    cameraDistance -= 0.5f;
                    break;
                case CameraMoveDirection.Back:
                    cameraDistance += 0.5f;
                    break;
                case CameraMoveDirection.Left:
                    cameraRotation -= 0.5f;
                    break;
                case CameraMoveDirection.Right:
                    cameraRotation += 0.5f;
                    break;
            }
        }

        public void ApplyCamera(Shader shader)
        {
            var camX = (float)Math.Sin(cameraRotation) * cameraDistance;
            var camZ = (float)Math.Cos(cameraRotation) * cameraDistance;

            Matrix.SetViewMatrix(shader.ViewMatrix, new Vector3(camX, cameraDistance * 0.5f, camZ), new Vector3(0, 0, 0));
            Matrix.SetProjectionMatrix(shader.ProjectionMatrix, (float)Math.PI / 4, AspectRatio);
        }
    }
}