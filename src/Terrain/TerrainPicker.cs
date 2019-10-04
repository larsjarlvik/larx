using System;
using OpenTK;

namespace Larx.Terrain
{
    public class TerrainPicker {
        private const float RayRange = 1000;
        private const int MaxRecursions = 300;
        private readonly TerrainRenderer renderer;

        public TerrainPicker(TerrainRenderer renderer)
        {
            this.renderer = renderer;
        }

        public Vector3 GetPosition(MousePicker picker)
        {
            return findPosition(0, 0, RayRange, picker);
        }

        private Vector3 findPosition(int count, float start, float finish, MousePicker picker) {
            float half = start + ((finish - start) / 2f);

            if (count >= MaxRecursions) {
                return picker.GetPointOnRay(half);
            }

            return intersectionInRange(start, half, picker)
                ? findPosition(count + 1, start, half, picker)
                : findPosition(count + 1, half, finish, picker);
        }

        private bool intersectionInRange(float start, float finish, MousePicker picker)
        {
            var endPoint = picker.GetPointOnRay(finish);
            var endElev = renderer.GetElevationAtPoint(endPoint.Xz);

            if (endElev == null) return true;

            return endPoint.Y < (float)endElev;
        }
    }
}