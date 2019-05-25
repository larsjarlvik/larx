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
        public Matrix4 ProjectionMatrix;
        public Matrix4 ViewMatrix;

        public Vector3 Position;

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
                    cameraRotation -= 0.1f;
                    break;
                case CameraMoveDirection.Right:
                    cameraRotation += 0.1f;
                    break;
            }

            Console.WriteLine(cameraRotation);
        }

        public void Update()
        {
            Position = new Vector3(
                (float)Math.Sin(cameraRotation) * cameraDistance,
                cameraDistance * 0.5f,
                (float)Math.Cos(cameraRotation) * cameraDistance
            );

            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)Math.PI / 4f, AspectRatio, 1, 1000);
            ViewMatrix = Matrix4.LookAt(Position, new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        }

        public void ApplyCamera(Shader shader)
        {
            GL.UniformMatrix4(shader.ViewMatrix, false, ref ViewMatrix);
            GL.UniformMatrix4(shader.ProjectionMatrix, false, ref ProjectionMatrix);
        }
    }
}