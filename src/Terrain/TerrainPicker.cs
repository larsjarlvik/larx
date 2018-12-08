using OpenTK;

namespace Larx.Terrain
{
    public partial class TerrainPicker {
        private const float RayRange = 200;
        private const int MaxRecursions = 50;

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

            // TODO: Check actual terrain
            return startPoint.Y > 0.0f && endPoint.Y < 0.0f;
        }
    }
}