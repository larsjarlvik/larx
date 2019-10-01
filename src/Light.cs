using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx
{
    public class Light
    {
        public Vector3 Ambient { get; private set; }
        public Vector3 Diffuse { get; private set; }
        public Vector3 Specular { get; private set; }
        public Vector3 Direction { get; set; }

        public Light()
        {
            Ambient = new Vector3(0.6f);
            Diffuse = new Vector3(0.6f);
            Specular = new Vector3(0.3f);
            Direction = new Vector3(0.5f, -1.0f, -0.5f);
        }

        public void ApplyLight(Shader shader)
        {
            GL.Uniform3(shader.LightDirection, Direction);
            GL.Uniform3(shader.LightAmbient, Ambient);
            GL.Uniform3(shader.LightDiffuse, Diffuse);
            GL.Uniform3(shader.LightSpecular, Specular);
        }
    }
}