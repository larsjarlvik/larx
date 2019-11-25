using System;
using Larx.Buffers;
using Larx.Utils;
using OpenTK;

namespace Larx.Shadows
{
    public class ShadowBox
    {
        public const int CascadeCount = 3;
        private float[] shadowFarPlanes = new float[] {
            300f,
            900f,
            1800f,
        };

        public readonly Framebuffer[] ShadowBuffer;
        public Matrix4[] ProjectionMatrix;
        public Matrix4[] ViewMatrix;
        public Matrix4[] ShadowMatrix;

        public ShadowBox()
        {
            ProjectionMatrix = new Matrix4[CascadeCount];
            ViewMatrix = new Matrix4[CascadeCount];
            ShadowMatrix = new Matrix4[CascadeCount];
            ShadowBuffer = new Framebuffer[CascadeCount];
            for (var i = 0; i < CascadeCount; i ++)
                ShadowBuffer[i] = new Framebuffer(new Size(State.ShadowMapResolution, State.ShadowMapResolution), false, true);
        }

        public void Update(Camera camera, Light light)
        {
            var near = State.Near;

            for (var i = 0; i < CascadeCount; i ++) {
                var frustumBox = camera.GetFrustumBox(near, shadowFarPlanes[i]);

                frustumBox[0].Y *= MathF.Sin(light.Rotation.Y);
                frustumBox[1].Y *= MathF.Sin(light.Rotation.Y);

                ViewMatrix[i] = Matrix4.LookAt(camera.Look - light.Direction, camera.Look, new Vector3(0.0f, 1.0f, 0.0f));
                ProjectionMatrix[i] = Matrix4.Identity;
                ProjectionMatrix[i].M11 =  2.0f / (frustumBox[1].X - frustumBox[0].X);
                ProjectionMatrix[i].M22 =  2.0f / (frustumBox[1].Y - frustumBox[0].Y);
                ProjectionMatrix[i].M33 = -2.0f / (frustumBox[1].Z - frustumBox[0].Z);
                ProjectionMatrix[i].M44 =  1.0f;

                ShadowMatrix[i] = ViewMatrix[i] * ProjectionMatrix[i] *
                    Matrix4.CreateScale(new Vector3(0.5f)) *
                    Matrix4.CreateTranslation(new Vector3(0.5f));

                near = shadowFarPlanes[i];
            }
        }

        public void RefreshBuffers()
        {
            for (var i = 0; i < CascadeCount; i ++)
                ShadowBuffer[i].RefreshBuffers();
        }
    }
}