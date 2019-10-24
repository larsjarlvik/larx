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
            Ambient = new Vector3(0.5f);
            Diffuse = new Vector3(0.4f);
            Specular = new Vector3(0.4f);
            Rotation = new Vector2(MathLarx.DegToRad(0), MathLarx.DegToRad(45));
        }

        public void ApplyLight(Shader shader)
        {
            Direction = new Vector3(
                -MathF.Sin(Rotation.X) * MathF.Cos(Rotation.Y),
                -MathF.Sin(Rotation.Y),
                -MathF.Cos(Rotation.X) * MathF.Cos(Rotation.Y)
            );

            GL.Uniform3(shader.LightDirection, Direction);
            GL.Uniform3(shader.LightAmbient, Ambient);
            GL.Uniform3(shader.LightDiffuse, Diffuse);
            GL.Uniform3(shader.LightSpecular, Specular);
        }
    }
}