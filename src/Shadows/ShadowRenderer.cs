using System;
using Larx.Buffers;
using Larx.Utils;
using OpenTK;

namespace Larx.Shadows
{
    public class ShadowRenderer
    {
        public readonly Framebuffer ShadowBuffer;
        public const float ShadowDistance = 250.0f;
        private Vector3 min;
        private Vector3 max;

        public Matrix4 ProjectionMatrix;
        public Matrix4 ViewMatrix;
        public Matrix4 ShadowMatrix;

        public ShadowRenderer()
        {
            ShadowBuffer = new Framebuffer(new Size(4096, 4096), false, true);
        }

        public void Update(Camera camera, Light light)
        {
            var points = new Vector3[8];
            points[0] = camera.getPoint(new Vector3(0, 0, State.Near));
            points[1] = camera.getPoint(new Vector3(0, 0, ShadowDistance));
            points[2] = camera.getPoint(new Vector3(1, 0, State.Near));
            points[3] = camera.getPoint(new Vector3(1, 0, ShadowDistance));
            points[4] = camera.getPoint(new Vector3(0, 1, State.Near));
            points[5] = camera.getPoint(new Vector3(0, 1, ShadowDistance));
            points[6] = camera.getPoint(new Vector3(1, 1, State.Near));
            points[7] = camera.getPoint(new Vector3(1, 1, ShadowDistance));
            var first = false;

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

                max.X = MathF.Max(max.X, point.X);
                min.X = MathF.Min(min.X, point.X);
                max.Y = MathF.Max(max.Y, point.Y);
                min.Y = MathF.Min(min.Y, point.Y);
                max.Z = MathF.Max(max.Z, point.Z);
                min.Z = MathF.Min(min.Z, point.Z);
            }

            ViewMatrix = Matrix4.LookAt(camera.Look - light.Direction, camera.Look, new Vector3(0.0f, 1.0f, 0.0f));
            ProjectionMatrix = Matrix4.CreateOrthographic((max.X - min.X), (max.Y - min.Y), min.Z, max.Z);
            ShadowMatrix = ViewMatrix * ProjectionMatrix *
                Matrix4.CreateScale(new Vector3(0.5f)) *
                Matrix4.CreateTranslation(new Vector3(0.5f));
        }
    }
}