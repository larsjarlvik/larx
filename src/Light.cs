using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx
{
    public class Light
    {
        public Vector3 Ambient { get; private set; }
        public Vector3 Diffuse { get; private set; }
        public Vector3 Specular { get; private set; }
        public Vector3 Position { get; set; }

        public Light()
        {
            Ambient = new Vector3(0.1f);
            Diffuse = new Vector3(0.3f);
            Specular = new Vector3(0.8f);
            Position = new Vector3(3500000.0f, 7500000.0f, 3500000.0f);
        }

        public void ApplyLight(Shader shader)
        {
            GL.Uniform3(shader.LightPosition, Position);
            GL.Uniform3(shader.LightAmbient, Ambient);
            GL.Uniform3(shader.LightDiffuse, Diffuse);
            GL.Uniform3(shader.LightSpecular, Specular);
        }
    }
}