using System;
using OpenTK;

namespace Larx.Terrain
{
    public class TerrainPicker
    {
        private const float RayRange = 1000;
        private const int MaxRecursions = 300;
        private readonly HeightMap renderer;
        private readonly Camera camera;

        public TerrainPicker(Camera camera, HeightMap renderer)
        {
            this.renderer = renderer;
            this.camera = camera;
        }

        public Vector3 GetPosition()
        {
            return findPosition(0, 0, RayRange);
        }

        private Vector3 findPosition(int count, float start, float finish) {
            var half = start + ((finish - start) / 2.0f);

            if (count >= MaxRecursions) {
                return getPointOnRay(half);
            }

            return elevationAtPoint(half) > getPointOnRay(half).Y
                ? findPosition(count + 1, start, half)
                : findPosition(count + 1, half, finish);
        }

        private float elevationAtPoint(float point)
        {
            return renderer.GetElevationAtPoint(getPointOnRay(point).Xz) ?? 0.0f;
        }

        private Vector3 getPointOnRay(float distance)
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