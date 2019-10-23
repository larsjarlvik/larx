using System;
using Larx.Buffers;
using Larx.Utils;
using OpenTK;

namespace Larx.Shadows
{
    public class ShadowRenderer
    {
        public readonly Framebuffer ShadowBuffer;
        private readonly Vector4 forward = new Vector4(0, 0, -1, 0);
        private readonly Vector4 up = new Vector4(0, 1, 0, 0);
        private const float offset = 10.0f;
        private const float shadowDistance = 100.0f;
        private Vector3 min;
        private Vector3 max;
        private float farWidth;
        private float farHeight;
        private float nearWidth;
        private float nearHeight;

        public Matrix4 ProjectionMatrix;
        public Matrix4 ViewMatrix;
        public Matrix4 ShadowMatrix;

        public ShadowRenderer()
        {
            ShadowBuffer = new Framebuffer(new Size(4096, 4096), false, true);
        }

        private Matrix4 calculateCameraRotation(Camera camera)
        {
            var rX = Matrix4.CreateRotationX(MathLarx.DegToRad(camera.Rotation.X));
            var rY = Matrix4.CreateRotationY(MathLarx.DegToRad(camera.Rotation.Y));
            return rX * rY;
        }

        Vector4 calculateLightSpaceFrustumCorner(Vector3 startPoint, Vector3 direction, float width)
        {
            var point = new Vector4(Vector3.Add(startPoint, direction * width), 1.0f);
            return Vector4.Transform(point, ViewMatrix);
        }

        private Vector4[] calculateFrustumVertices(Matrix4 rotation, Vector3 forwardVector, Vector3 centerNear, Vector3 centerFar)
        {
            var upVector = Vector4.Transform(rotation, up).Xyz;
            var rightVector = Vector3.Cross(forwardVector, upVector);
            var downVector = new Vector3(-upVector);
            var leftVector = new Vector3(-rightVector);
            var farTop = Vector3.Add(centerFar, new Vector3(upVector * farHeight));
            var farBottom = Vector3.Add(centerFar, new Vector3(downVector * farHeight));
            var nearTop = Vector3.Add(centerNear, new Vector3(upVector * nearHeight));
            var nearBottom = Vector3.Add(centerNear, new Vector3(downVector * nearHeight));

            var points = new Vector4[8];
            points[0] = calculateLightSpaceFrustumCorner(farTop, rightVector, farWidth);
            points[1] = calculateLightSpaceFrustumCorner(farTop, leftVector, farWidth);
            points[2] = calculateLightSpaceFrustumCorner(farBottom, rightVector, farWidth);
            points[3] = calculateLightSpaceFrustumCorner(farBottom, leftVector, farWidth);
            points[4] = calculateLightSpaceFrustumCorner(nearTop, rightVector, nearWidth);
            points[5] = calculateLightSpaceFrustumCorner(nearTop, leftVector, nearWidth);
            points[6] = calculateLightSpaceFrustumCorner(nearBottom, rightVector, nearWidth);
            points[7] = calculateLightSpaceFrustumCorner(nearBottom, leftVector, nearWidth);
            return points;
        }

        public void Update(Camera camera, Light light)
        {
            updateViewMatrix(camera, light);
            updateProjectionMatrix();

            ShadowMatrix = ViewMatrix * ProjectionMatrix;
            ShadowMatrix *= Matrix4.CreateScale(new Vector3(0.5f));
            ShadowMatrix *= Matrix4.CreateTranslation(new Vector3(0.5f));

            var rotation = calculateCameraRotation(camera);
            var forwardVector = Vector4.Transform(forward, rotation).Xyz;
            var toFar = new Vector3(forwardVector) * shadowDistance;
            var toNear = new Vector3(forwardVector) * State.Near;
            var centerNear = Vector3.Add(toNear, camera.Position);
            var centerFar = Vector3.Add(toFar, camera.Position);

            var points = calculateFrustumVertices(rotation, forwardVector, centerNear, centerFar);
            var first = true;

            foreach(var point in points)
            {
                if (first) {
                    min.X = point.X;
                    max.X = point.X;
                    min.Y = point.Y;
                    max.Y = point.Y;
                    min.Z = point.Z;
                    max.Z = point.Z;
                    first = false;
                    continue;
                }

                if (point.X > max.X) max.X = point.X;
                else if (point.X < min.X) min.X = point.X;

                if (point.Y > max.Y) max.Y = point.Y;
                else if (point.Y < min.Y) min.Y = point.Y;

                if (point.Z > max.Z) max.Z = point.Z;
                else if (point.Z < min.Z) min.Z = point.Z;
            }

            max.Z += offset;
        }

        private void updateProjectionMatrix()
        {
            ProjectionMatrix = Matrix4.CreateOrthographic((max.X - min.X), (max.Y - min.Y), min.Z, max.Z);
        }

        private void updateViewMatrix(Camera camera, Light light)
        {
            ViewMatrix =
                Matrix4.CreateTranslation(-camera.Position) *
                Matrix4.CreateRotationX(-light.Direction.Y) *
                Matrix4.CreateRotationY(-light.Direction.X);
        }


        public void Resize()
        {
            var aspect = (float)State.Window.Size.Width / (float)State.Window.Size.Height;
            farWidth = shadowDistance * MathF.Tan(MathF.PI / 4f);
            nearWidth = State.Near * MathF.Tan(MathF.PI / 4f);
            farHeight = farWidth / aspect;
            nearHeight = nearWidth / aspect;
        }
    }
}