using System;
using Larx.Utils;
using OpenTK;

namespace Larx
{
    public class Camera
    {
        public const float LookDeceleration = 8.0f;
        public const float RotationDeceleration = 0.1f;
        public const float DistanceDeceleration = 4.0f;

        public float Distance = 150.0f;
        private float cameraDistanceSpeed = 0.0f;
        public Vector3 Look;
        public Vector2 Rotation;
        public Vector3 LookSpeed;
        public Vector2 RotationSpeed;
        public Vector3 Position;

        public Matrix4 ProjectionMatrix;
        public Matrix4 ViewMatrix;

        public Vector4[] FrustumPlanes { get; private set; }

        public Camera()
        {
            Look = new Vector3();
            Rotation = new Vector2(0.0f, 30.0f);
            Update(0.0f);
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
            var rx = MathLarx.DegToRad(Rotation.X);

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
            Distance += cameraDistanceSpeed;
            cameraDistanceSpeed /= (frameTime * DistanceDeceleration) + 1.0f;
        }

        public void InvertY()
        {
            var invertedPosition = new Vector3(Position.X, -Position.Y, Position.Z);
            ViewMatrix = Matrix4.LookAt(invertedPosition, Look, new Vector3(0, -1, 0));
        }

        public void Reset()
        {
            ViewMatrix = Matrix4.LookAt(Position, Look, new Vector3(0, 1, 0));
        }

        public void Update(float frameTime)
        {
            move(frameTime);
            rotate(frameTime);
            zoom(frameTime);

            var rx = MathLarx.DegToRad(Rotation.X);
            var ry = MathLarx.DegToRad(Rotation.Y);

            Position = new Vector3(
                Look.X - (MathF.Sin(rx) * MathF.Cos(ry) * Distance),
                Distance * MathF.Sin(ry),
                Look.Z - (MathF.Cos(rx) * MathF.Cos(ry) * Distance)
            );

            ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathF.PI / 4f, State.Window.Aspect, State.Near, State.Far);
            ViewMatrix = Matrix4.LookAt(Position, Look, new Vector3(0, 1, 0));
            FrustumPlanes = Frustum.ExtractFrustum(ViewMatrix, ProjectionMatrix);
        }

        public Vector3 GetPoint(Vector3 pos)
        {
            var clipCoords = new Vector4(pos.X, pos.Y, -1f, 1f);
            var eyeCoords = Vector4.Transform(clipCoords, Matrix4.Invert(ProjectionMatrix));
            var trimmedEyeCoords = new Vector4(eyeCoords.X, eyeCoords.Y, -1, 0);

            var worldCoords = Vector4.Transform(trimmedEyeCoords, Matrix4.Invert(ViewMatrix)).Normalized();
            var ray = worldCoords.Normalized().Xyz;

            return Vector3.Add(Position, ray * pos.Z);
        }
    }
}