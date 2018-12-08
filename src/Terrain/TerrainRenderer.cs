using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Terrain
{
    public class TerrainRenderer
    {
        public TerrainShader Shader { get; }

        public TerrainRenderer()
        {
            Shader = new TerrainShader();
            build();
        }

        public void Render(Camera camera)
        {
            camera.ApplyCamera(Shader);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, 4);
        }

        private void build()
        {
            var triangleArray = GL.GenVertexArray();
            GL.BindVertexArray(triangleArray);
            var positions = new Vector3[] {
                new Vector3(-0.5f, -0.5f, 0.0f),
                new Vector3(-0.5f,  0.5f, 0.0f),
                new Vector3( 0.5f, -0.5f, 0.0f),
                new Vector3( 0.5f,  0.5f, 0.0f)
            };

            var positionBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, positions.Length * Vector3.SizeInBytes, positions, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            var colors = new Vector3[] {
                new Vector3(1.0f, 0.0f, 0.0f),
                new Vector3(0.0f, 1.0f, 0.0f),
                new Vector3(0.0f, 0.0f, 1.0f),
                new Vector3(1.0f, 0.0f, 0.0f)
            };

            var colorBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, colorBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, colors.Length * Vector3.SizeInBytes, colors, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);
        }
    }
}