using System;
using OpenTK;

namespace Larx.Terrain
{
    public class TerrainPicker {
        private const float RayRange = 2000;
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

            if (intersectionInRange(start, half, picker)) {
                return findPosition(count + 1, start, half, picker);
            } else {
                return findPosition(count + 1, half, finish, picker);
            }
        }

        private bool intersectionInRange(float start, float finish, MousePicker picker)
        {
            var startPoint = picker.GetPointOnRay(start);
            var endPoint = picker.GetPointOnRay(finish);

            var startElev = renderer.GetElevationAtPoint(startPoint);
            var endElev = renderer.GetElevationAtPoint(endPoint);

            if (startElev == null || endElev == null) return true;

            return startPoint.Y > (float)startElev && endPoint.Y < (float)endElev;
        }
    }
}