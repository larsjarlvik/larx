using System;
using OpenTK;

namespace Larx.TerrainV3
{
    public class TerrainPicker
    {
        private const float RayRange = 1000;
        private const int MaxRecursions = 300;

        public Vector3 GetPosition(Camera camera)
        {
            return findPosition(camera, 0, 0, RayRange);
        }

        private Vector3 findPosition(Camera camera, int count, float start, float finish) {
            var half = start + ((finish - start) / 2.0f);

            if (count >= MaxRecursions) {
                return getPointOnRay(camera, half);
            }

            return 0.0f > getPointOnRay(camera, half).Y
                ? findPosition(camera, count + 1, start, half)
                : findPosition(camera, count + 1, half, finish);
        }

        private Vector3 getPointOnRay(Camera camera, float distance)
        {
            var ray = new Vector3(
                (2f * State.Mouse.Position.X) / State.Window.Size.Width - 1.0f,
                -((2f * State.Mouse.Position.Y) / State.Window.Size.Height - 1.0f),
                distance
            );

            return camera.GetPoint(ray);
        }
    }
}