using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Water
{
    public class WaterRenderer
    {
        private readonly WaterShader shader;
        private int vertexBuffer;
        private int indexBuffer;

        public WaterRenderer()
        {
            shader = new WaterShader();
            build();
        }

        private void build()
        {
            var halfMapSize = State.MapSize / 2;

            var vertices = new Vector3[] {
                new Vector3(-halfMapSize, 0.0f, halfMapSize),
                new Vector3( halfMapSize, 0.0f, halfMapSize),
                new Vector3( halfMapSize, 0.0f,-halfMapSize),
                new Vector3(-halfMapSize, 0.0f,-halfMapSize),
            };

            var indices = new int[] {
                0, 1, 2, 2, 3, 0,
            };

            vertexBuffer = GL.GenBuffer();
            indexBuffer = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Length * Vector3.SizeInBytes, vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);
        }

        public void Render(Camera camera, Light light)
        {
            GL.EnableVertexAttribArray(0);
            GL.UseProgram(shader.Program);

            camera.ApplyCamera(shader);
            light.ApplyLight(shader);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }
}