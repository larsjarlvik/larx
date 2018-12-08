using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Terrain
{
    public class TerrainRenderer
    {
        private const int mapSize = 32;
        private int indexCount = 0;

        public TerrainShader Shader { get; }

        public TerrainRenderer()
        {
            Shader = new TerrainShader();
            build();
        }

        public void Render(Camera camera)
        {
            camera.ApplyCamera(Shader);
            GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedShort, IntPtr.Zero);
        }

        private void build()
        {
            var triangleArray = GL.GenVertexArray();
            GL.BindVertexArray(triangleArray);

            var halfMapSize = (float)(mapSize / 2);
            var rnd = new Random();

            var vertices = new List<Vector3>();
            var colors = new List<Vector3>();
            var indices = new List<ushort>();
            var i = 0;

            for(var z = -halfMapSize; z <= halfMapSize; z++) {
                for(var x = -halfMapSize; x <= halfMapSize; x++) {
                    vertices.Add(new Vector3(x, (float)rnd.NextDouble(), z));
                    colors.Add(new Vector3((float)rnd.NextDouble(), (float)rnd.NextDouble(), (float)rnd.NextDouble()));

                    if (x < halfMapSize && z < halfMapSize) {
                        indices.AddRange(new ushort[] {
                            (ushort)(i), (ushort)(i + 1), (ushort)(i + mapSize + 1),
                            (ushort)(i + 1), (ushort)(i + mapSize + 1), (ushort)(i + mapSize + 2)
                        });
                    }

                    i ++;
                }
            }

            indexCount = indices.Count;

            var vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Count * Vector3.SizeInBytes, vertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            var colorBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, colorBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, colors.Count * Vector3.SizeInBytes, colors.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);

            var indexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(ushort), indices.ToArray(), BufferUsageHint.StaticDraw);
        }
    }
}