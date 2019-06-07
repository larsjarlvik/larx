using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx
{
    public class Camera
    {
        public const float LookDeceleration = 8.0f;
        public const float RotationDeceleration = 0.1f;
        public const float DistanceDeceleration = 4.0f;

        private float cameraDistance = 40.0f;
        private float cameraDistanceSpeed = 0.0f;
        public Vector3 Look;
        public Vector2 Rotation;
        public Vector3 LookSpeed;
        public Vector2 RotationSpeed;
        public Vector3 Position;

        public Matrix4 ProjectionMatrix;
        public Matrix4 ViewMatrix;



        private float DegToRad(float input) {
            return (MathF.PI / 180) * input;
        }

        public Camera()
        {
            Look = new Vector3();
            Rotation = new Vector2(0.0f, 30.0f);
        }

        public void Move(Vector3 delta)
        {
            LookSpeed += delta * 0.2f;

            if (LookSpeed.X > 2.0f) LookSpeed.X = 2.0f;
            if (LookSpeed.X < -2.0f) LookSpeed.X = -2.0f;
            if (LookSpeed.Z > 2.0f) LookSpeed.Z = 2.0f;
            if (LookSpeed.Z < -2.0f) LookSpeed.Z = -2.0f;
        }

        private void move(float frameTime)
        {
            var rx = DegToRad(Rotation.X);

            Look.Z += LookSpeed.Z * MathF.Cos(rx);
            Look.X += LookSpeed.Z * MathF.Sin(rx);

            Look.Z += LookSpeed.X * MathF.Cos(rx + MathF.PI / 2);
            Look.X += LookSpeed.X * MathF.Sin(rx + MathF.PI / 2);

            LookSpeed /= (frameTime * LookDeceleration) + 1.0f;
        }

        public void Rotate(Vector2 delta)
        {
            Rotation += delta * 0.5f;
        }

        private void rotate(float frameTime)
        {
            Rotation += RotationSpeed;
            RotationSpeed /= (frameTime * RotationDeceleration) + 1.0f;
        }

        public void Zoom(float delta)
        {
            cameraDistanceSpeed += delta;

            if (cameraDistanceSpeed > 1.0f) cameraDistanceSpeed = 1.0f;
            if (cameraDistanceSpeed < -1.0f) cameraDistanceSpeed = -1.0f;
        }

        private void zoom(float frameTime)
        {
            cameraDistance += cameraDistanceSpeed;
            cameraDistanceSpeed /= (frameTime * DistanceDeceleration) + 1.0f;
        }

        public void Update(float frameTime)
        {
            move(frameTime);
            rotate(frameTime);
            zoom(frameTime);

            var rx = DegToRad(Rotation.X);
            var ry = DegToRad(Rotation.Y);

            Position = new Vector3(
                Look.X - (MathF.Sin(rx) * MathF.Cos(ry) * cameraDistance),
                cameraDistance * MathF.Sin(ry),
                Look.Z - (MathF.Cos(rx) * MathF.Cos(ry) * cameraDistance)
            );

            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathF.PI / 4f, State.Window.Aspect, 1, 1000);
            ViewMatrix = Matrix4.LookAt(Position, Look, new Vector3(0, 1, 0));
        }

        public void ApplyCamera(Shader shader)
        {
            GL.UniformMatrix4(shader.ViewMatrix, false, ref ViewMatrix);
            GL.UniformMatrix4(shader.ProjectionMatrix, false, ref ProjectionMatrix);
        }
    }
}