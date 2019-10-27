using System;
using OpenTK;

namespace Larx.Utils
{
    public static class MathLarx
    {
        public static float BaryCentric(Vector3 p1, Vector3 p2, Vector3 p3, Vector2 pos)
        {
            var det = (p2.Z - p3.Z) * (p1.X - p3.X) + (p3.X - p2.X) * (p1.Z - p3.Z);
            var l1 = ((p2.Z - p3.Z) * (pos.X - p3.X) + (p3.X - p2.X) * (pos.Y - p3.Z)) / det;
            var l2 = ((p3.Z - p1.Z) * (pos.X - p3.X) + (p1.X - p3.X) * (pos.Y - p3.Z)) / det;
            var l3 = 1.0 - l1 - l2;
            return (float)(l1 * p1.Y + l2 * p2.Y + l3 * p3.Y);
        }

        public static float DegToRad(float input)
        {
            return (MathF.PI / 180) * input;
        }

        public static float RadToDeg(float input)
        {
            return (180 / MathF.PI) * input;
        }

        public static Vector3 CalculateTangent(Vector3 input)
        {
            var c1 = Vector3.Cross(input, new Vector3(0, 0, 1));
            var c2 = Vector3.Cross(input, new Vector3(0, 1, 0));

            return Vector3.Normalize(c1.Length > c2.Length ? c1 : c2);
        }
    }
}