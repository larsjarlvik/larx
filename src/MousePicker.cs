using System;
using OpenTK;

namespace Larx
{
    public class MousePicker
    {
        private Camera camera;

        public Vector3 MouseRay;

        public MousePicker(Camera camera)
        {
            this.camera = camera;
        }

        public void Update(int mouseX, int mouseY, int viewportWidth, int viewportHeight)
        {
            var normalizedMouse = new Vector2(
                (2f * mouseX) / viewportWidth - 1f,
                -((2f * mouseY) / viewportHeight - 1f)
            );

            var clipCoords = new Vector4(normalizedMouse.X, normalizedMouse.Y, -1f, 1f);

            var eyeCoords = Vector4.Transform(clipCoords, Matrix4.Invert(camera.ProjectionMatrix));
            var trimmedEyeCoords = new Vector4(eyeCoords.X, eyeCoords.Y, -1, 0);

            var worldCoords = Vector4.Transform(trimmedEyeCoords, Matrix4.Invert(camera.ViewMatrix));
            MouseRay = new Vector3(worldCoords.X, worldCoords.Y, worldCoords.Z);
            MouseRay.Normalize();
        }

        public Vector3 GetPointOnRay(float distance) {
            var scaledRay = new Vector3(MouseRay.X * distance, MouseRay.Y * distance, MouseRay.Z * distance);
            return Vector3.Add(camera.Position, scaledRay);
        }
    }
}