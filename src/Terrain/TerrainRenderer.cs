using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Terrain
{
    public class TerrainRenderer : TerrainPicker
    {
        private const int mapSize = 32;
        private int indexCount = 0;
        private int vertexBuffer;
        private int colorBuffer;
        private int indexBuffer;
        private List<Vector3> vertices = new List<Vector3>();
        private List<Vector3> colors = new List<Vector3>();

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

        public void ChangeElevation(float offset, MousePicker picker) {
            var position = GetPosition(picker);
            var x = (int)Math.Round(position.X + (mapSize / 2));
            var z = (int)Math.Round(position.Z + (mapSize / 2));

            var index = (z * (mapSize + 1)) + x;
            if (index < 0 || index > vertices.Count) return;

            var elev = vertices[index].Y + offset;

            vertices[index] = new Vector3(vertices[index].X, elev, vertices[index].Z);
            colors[index] = elev > 0f
                ? new Vector3(1f, 1f - (elev / 2f), 1f - (elev / 2f))
                : new Vector3(1f + (elev / 2f), 1f + (elev / 2f), 1f);

            updateBuffers();
        }

        private void build()
        {
            var triangleArray = GL.GenVertexArray();
            GL.BindVertexArray(triangleArray);

            var halfMapSize = (float)(mapSize / 2);
            var rnd = new Random();

            var indices = new List<ushort>();
            var i = 0;

            for(var z = -halfMapSize; z <= halfMapSize; z++) {
                for(var x = -halfMapSize; x <= halfMapSize; x++) {
                    vertices.Add(new Vector3(x, 0f, z));
                    colors.Add(new Vector3(1f, 1f, 1f));

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

            vertexBuffer = GL.GenBuffer();
            colorBuffer = GL.GenBuffer();
            indexBuffer = GL.GenBuffer();

            updateBuffers();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(ushort), indices.ToArray(), BufferUsageHint.StaticDraw);
        }

        private void updateBuffers()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Count * Vector3.SizeInBytes, vertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, colorBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, colors.Count * Vector3.SizeInBytes, colors.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(1);
        }
    }
}