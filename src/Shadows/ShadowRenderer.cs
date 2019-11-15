using System;
using Larx.Buffers;
using Larx.Utils;
using OpenTK;

namespace Larx.Shadows
{
    public class ShadowRenderer
    {
        public readonly Framebuffer ShadowBuffer;
        private Vector3 min;
        private Vector3 max;

        public Matrix4 ProjectionMatrix;
        public Matrix4 ViewMatrix;
        public Matrix4 ShadowMatrix;

        public ShadowRenderer()
        {
            ShadowBuffer = new Framebuffer(new Size(State.ShadowMapResolution, State.ShadowMapResolution), false, true);
        }

        public void Update(Camera camera, Light light)
        {
            var shadowFarPlane = camera.Distance * 3.0f + 20.0f;
            var first = true;

            foreach(var point in camera.GetFrustumCorners(State.Near, shadowFarPlane))
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

                max.X = MathF.Max(max.X, point.X);
                min.X = MathF.Min(min.X, point.X);
                max.Y = MathF.Max(max.Y, point.Y);
                min.Y = MathF.Min(min.Y, point.Y);
                max.Z = MathF.Max(max.Z, point.Z);
                min.Z = MathF.Min(min.Z, point.Z);
            }

            min.Y *= MathF.Sin(light.Rotation.Y);
            max.Y *= MathF.Sin(light.Rotation.Y);

            ViewMatrix = Matrix4.LookAt(camera.Look - light.Direction, camera.Look, new Vector3(0.0f, 1.0f, 0.0f));
            ProjectionMatrix = Matrix4.Identity;
            ProjectionMatrix.M11 = 2.0f / (max.X - min.X);
            ProjectionMatrix.M22 = 2.0f / (max.Y - min.Y);
            ProjectionMatrix.M33 = -2.0f / (max.Z - min.Z);
            ProjectionMatrix.M44 = 1.0f;

            ShadowMatrix = ViewMatrix * ProjectionMatrix *
                Matrix4.CreateScale(new Vector3(0.5f)) *
                Matrix4.CreateTranslation(new Vector3(0.5f));
        }
    }
}