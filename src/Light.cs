using System;
using Larx.Utils;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx
{
    public class Light
    {
        public Vector3 Ambient { get; private set; }
        public Vector3 Diffuse { get; private set; }
        public Vector3 Specular { get; private set; }
        public Vector2 Rotation;
        public Vector3 Direction;

        public Light()
        {
            Ambient = new Vector3(0.2f);
            Diffuse = new Vector3(0.7f);
            Specular = new Vector3(0.2f);
            Rotation = new Vector2(MathLarx.DegToRad(0), MathLarx.DegToRad(45));
        }

        internal void Update()
        {
            Direction = new Vector3(
                -MathF.Sin(Rotation.X) * MathF.Cos(Rotation.Y),
                -MathF.Sin(Rotation.Y),
                -MathF.Cos(Rotation.X) * MathF.Cos(Rotation.Y)
            );
        }
    }
}