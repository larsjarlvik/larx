using System;
using OpenTK;

namespace Larx.Utils
{
    public static class MathLarx
    {
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