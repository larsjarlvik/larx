using System;
using OpenTK;

namespace Larx.Terrain
{
    public class TerrainPicker {
        private const float RayRange = 1000;
        private const int MaxRecursions = 300;
        private readonly TerrainRenderer renderer;
        private readonly Camera camera;

        public TerrainPicker(TerrainRenderer renderer, Camera camera)
        {
            this.renderer = renderer;
            this.camera = camera;
        }

        public Vector3 GetPosition()
        {
            return findPosition(0, 0, RayRange);
        }

        private Vector3 findPosition(int count, float start, float finish) {
            float half = start + ((finish - start) / 2f);

            if (count >= MaxRecursions) {
                return getPointOnRay(half);
            }

            return intersectionInRange(start, half)
                ? findPosition(count + 1, start, half)
                : findPosition(count + 1, half, finish);
        }

        private bool intersectionInRange(float start, float finish)
        {
            var endPoint = getPointOnRay(finish);
            var endElev = renderer.GetElevationAtPoint(endPoint.Xz);

            if (endElev == null) return true;

            return endPoint.Y < (float)endElev;
        }

        private Vector3 getPointOnRay(float distance)
        {
            var ray = new Vector3(
                (2f * State.Mouse.Position.X) / State.Window.Size.Width - 1f,
                -((2f * State.Mouse.Position.Y) / State.Window.Size.Height - 1f),
                distance
            );

            return camera.getPoint(ray);
        }
    }
}