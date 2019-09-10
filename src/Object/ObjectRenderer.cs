using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Object
{
    public class ObjectRenderer
    {
        private readonly ObjectShader shader;
        private int vertexBuffer;
        private int indexBuffer;

        public ObjectRenderer()
        {
            shader = new ObjectShader();
            build();
        }

        private void build()
        {
            var vertices = new Vector3[] {
                // Front
                new Vector3(-0.25f,-0.05f, 0.25f),
                new Vector3( 0.25f,-0.05f, 0.25f),
                new Vector3( 0.25f, 0.05f, 0.25f),
                new Vector3(-0.25f, 0.05f, 0.25f),
                // Back
                new Vector3(-0.25f,-0.05f,-0.25f),
                new Vector3( 0.25f,-0.05f,-0.25f),
                new Vector3( 0.25f, 0.05f,-0.25f),
                new Vector3(-0.25f, 0.05f,-0.25f),
            };

            var indices = new int[] {
                0, 1, 2, 2, 3, 0, // front
                1, 5, 6, 6, 2, 1, // right
                7, 6, 5, 5, 4, 7, // back
                4, 0, 3, 3, 7, 4, // left
                4, 5, 1, 1, 0, 4, // bottom
                3, 2, 6, 6, 7, 3, // top
            };

            vertexBuffer = GL.GenBuffer();
            indexBuffer = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Length * Vector3.SizeInBytes, vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);
        }

        public void Render(Camera camera, Vector3 position)
        {
            GL.EnableVertexAttribArray(0);
            GL.UseProgram(shader.Program);

            GL.Uniform3(shader.Position, position.X, position.Y, position.Z);

            camera.ApplyCamera(shader);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }
}