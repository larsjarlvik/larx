using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx
{
    public class Light
    {
        public Vector3 Ambient { get; private set; }
        public Vector3 Diffuse { get; private set; }
        public Vector3 Specular { get; private set; }
        public Vector3 Position { get; private set; }

        public Light()
        {
            Ambient = new Vector3(0.5f, 0.5f, 0.5f);
            Diffuse = new Vector3(1.0f, 1.0f, 1.0f);
            Specular = new Vector3(1.0f, 1.0f, 1.0f);
            Position = new Vector3(350.0f, 350.0f, 350.0f);
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