using System;
using Larx.Buffers;
using Larx.Utils;
using OpenTK;

namespace Larx.Shadows
{
    public class ShadowBox
    {
        public readonly Framebuffer ShadowBuffer;

        public Matrix4 ProjectionMatrix;
        public Matrix4 ViewMatrix;
        public Matrix4 ShadowMatrix;
        public Vector4[] ShadowFrustumPlanes;

        public ShadowBox()
        {
            ShadowBuffer = new Framebuffer(new Size(State.ShadowMapResolution, State.ShadowMapResolution), false, true);
        }

        public void Update(Camera camera, Light light)
        {
            var shadowFarPlane = 400.0f;
            var projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathF.PI / 4f, State.Window.Aspect, State.Near, shadowFarPlane);

            ViewMatrix = Matrix4.LookAt(camera.Position - light.Direction, camera.Position, new Vector3(0.0f, 1.0f, 0.0f));
            var frustumCorners = Frustum.GetFrustumCorners(Frustum.ExtractFrustum(ViewMatrix, projectionMatrix));

            var min = Vector3.Zero;
            var max = Vector3.Zero;
            var first = false;

            foreach(var point in frustumCorners)
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

            min *= new Vector3(2.0f);
            max *= new Vector3(2.0f);

            ProjectionMatrix = Matrix4.Identity;
            ProjectionMatrix.M11 =  2.0f / (max.X - min.X);
            ProjectionMatrix.M22 =  2.0f / (max.Y - min.Y);
            ProjectionMatrix.M33 = -2.0f / (max.Z - min.Z);
            ProjectionMatrix.M44 =  1.0f;

            ShadowMatrix = ViewMatrix * ProjectionMatrix *
                Matrix4.CreateScale(new Vector3(0.5f)) *
                Matrix4.CreateTranslation(new Vector3(0.5f));

            ShadowFrustumPlanes = Frustum.ExtractFrustum(ViewMatrix, ProjectionMatrix);
        }
    }
}