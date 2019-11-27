using System;
using Larx.Buffers;
using Larx.Utils;
using OpenTK;

namespace Larx.Shadows
{
    public struct ShadowFrustum
    {
        public Vector4[] Planes { get; }

        public ShadowFrustum(Vector4[] planes)
        {
            Planes = planes;
        }
    }

    public class ShadowBox
    {
        public const int CascadeCount = 3;
        private float[] shadowFarPlanes = new float[] {
            100f,
            300f,
            600f,
        };

        public readonly Framebuffer[] ShadowBuffer;
        public Matrix4[] ProjectionMatrix;
        public Matrix4[] ViewMatrix;
        public Matrix4[] ShadowMatrix;
        public ShadowFrustum[] ShadowFrustumPlanes;

        public ShadowBox()
        {
            ShadowFrustumPlanes = new ShadowFrustum[CascadeCount];
            ProjectionMatrix = new Matrix4[CascadeCount];
            ViewMatrix = new Matrix4[CascadeCount];
            ShadowMatrix = new Matrix4[CascadeCount];
            ShadowBuffer = new Framebuffer[CascadeCount];
            for (var i = 0; i < CascadeCount; i ++)
                ShadowBuffer[i] = new Framebuffer(new Size(State.ShadowMapResolution, State.ShadowMapResolution), false, true);
        }

        private Matrix4 getLightViewMatrix(Camera camera, Light light)
        {
            var ld = light.Direction.Normalized();
            var pitch = MathF.Acos(ld.Xz.Length);
            var yaw = MathF.Atan(ld.X / ld.Z);

            yaw = ld.Z > 0 ? yaw - MathF.PI : yaw;

            return Matrix4.CreateTranslation(-camera.Look) *
                Matrix4.CreateRotationY(-yaw) *
                Matrix4.CreateRotationX(pitch);
        }

        public void Update(Camera camera, Light light)
        {
            var near = State.Near;

            for (var i = 0; i < CascadeCount; i ++) {
                ViewMatrix[i] = getLightViewMatrix(camera, light);

                var projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(MathF.PI / 4f, State.Window.Aspect, near, shadowFarPlanes[i]);
                var frustumCorners = Frustum.GetFrustumCorners(Frustum.ExtractFrustum(ViewMatrix[i], projectionMatrix));

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

                min *= 2.0f;
                max *= 2.0f;

                ProjectionMatrix[i] = Matrix4.Identity;
                ProjectionMatrix[i].M11 =  2.0f / (max.X - min.X);
                ProjectionMatrix[i].M22 =  2.0f / (max.Y - min.Y);
                ProjectionMatrix[i].M33 = -2.0f / (max.Z - min.Z);
                ProjectionMatrix[i].M44 =  1.0f;

                ShadowMatrix[i] = ViewMatrix[i] * ProjectionMatrix[i] *
                    Matrix4.CreateScale(new Vector3(0.5f)) *
                    Matrix4.CreateTranslation(new Vector3(0.5f));

                ShadowFrustumPlanes[i] = new ShadowFrustum(Frustum.ExtractFrustum(ViewMatrix[i], ProjectionMatrix[i]));
            }
        }

        public void RefreshBuffers()
        {
            for (var i = 0; i < CascadeCount; i ++)
                ShadowBuffer[i].RefreshBuffers();
        }
    }
}