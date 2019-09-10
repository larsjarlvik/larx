using System;
using OpenTK;

namespace Larx.Terrain
{
    public partial class TerrainPicker {
        private const float RayRange = 1000;
        private const int MaxRecursions = 300;

        public Vector3 GetPosition(MousePicker picker, TerrainRenderer renderer)
        {
            return findPosition(0, 0, RayRange, picker, renderer);
        }

        private Vector3 findPosition(int count, float start, float finish, MousePicker picker, TerrainRenderer renderer) {
            float half = start + ((finish - start) / 2f);

            if (count >= MaxRecursions) {
                return picker.GetPointOnRay(half);
            }

            if (intersectionInRange(start, half, picker, renderer)) {
                return findPosition(count + 1, start, half, picker, renderer);
            } else {
                return findPosition(count + 1, half, finish, picker, renderer);
            }
        }

        private bool intersectionInRange(float start, float finish, MousePicker picker, TerrainRenderer renderer)
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