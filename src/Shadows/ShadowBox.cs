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

        public ShadowBox()
        {
            ShadowBuffer = new Framebuffer(new Size(State.ShadowMapResolution, State.ShadowMapResolution), false, true);
        }

        public void Update(Camera camera, Light light)
        {
            var shadowFarPlane = camera.Distance * 3.0f + 20.0f;
            var frustumBox = camera.GetFrustumBox(State.Near, shadowFarPlane);

            frustumBox[0].Y *= MathF.Sin(light.Rotation.Y);
            frustumBox[1].Y *= MathF.Sin(light.Rotation.Y);

            ViewMatrix = Matrix4.LookAt(camera.Look - light.Direction, camera.Look, new Vector3(0.0f, 1.0f, 0.0f));
            ProjectionMatrix = Matrix4.Identity;
            ProjectionMatrix.M11 =  2.0f / (frustumBox[1].X - frustumBox[0].X);
            ProjectionMatrix.M22 =  2.0f / (frustumBox[1].Y - frustumBox[0].Y);
            ProjectionMatrix.M33 = -2.0f / (frustumBox[1].Z - frustumBox[0].Z);
            ProjectionMatrix.M44 =  1.0f;

            ShadowMatrix = ViewMatrix * ProjectionMatrix *
                Matrix4.CreateScale(new Vector3(0.5f)) *
                Matrix4.CreateTranslation(new Vector3(0.5f));
        }
    }
}