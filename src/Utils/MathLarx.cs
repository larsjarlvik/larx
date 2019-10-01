using System;
using OpenTK;

namespace Larx.Utils
{
    public static class MathLarx
    {
        public static float BaryCentric(Vector3 p1, Vector3 p2, Vector3 p3, Vector2 pos) {
            var det = (p2.Z - p3.Z) * (p1.X - p3.X) + (p3.X - p2.X) * (p1.Z - p3.Z);
            var l1 = ((p2.Z - p3.Z) * (pos.X - p3.X) + (p3.X - p2.X) * (pos.Y - p3.Z)) / det;
            var l2 = ((p3.Z - p1.Z) * (pos.X - p3.X) + (p1.X - p3.X) * (pos.Y - p3.Z)) / det;
            var l3 = 1.0 - l1 - l2;
            return (float)(l1 * p1.Y + l2 * p2.Y + l3 * p3.Y);
        }

        public static float DegToRad(float input) {
            return (MathF.PI / 180) * input;
        }
    }
}