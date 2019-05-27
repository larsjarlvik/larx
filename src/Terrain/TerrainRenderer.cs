using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Terrain
{
    public class TerrainRenderer : TerrainPicker
    {
        private const int mapSize = 128;
        private int indexCount = 0;
        private int vertexBuffer;
        private int coordBuffer;
        private int indexBuffer;
        private int normalBuffer;
        private List<Vector3> vertices = new List<Vector3>();
        private List<Vector2> coords = new List<Vector2>();
        private List<Vector3> normals = new List<Vector3>();
        private List<int> indices = new List<int>();

        private readonly TerrainShader shader;
        private readonly Texture texture;

        public bool ShowGridLines { get; set; }

        public TerrainRenderer()
        {
            shader = new TerrainShader();
            
            texture = new Texture();
            texture.LoadTexture(new [] { 
                Path.Combine("resources", "textures", "grass.bmp"),
                Path.Combine("resources", "textures", "sand.bmp"),
                Path.Combine("resources", "textures", "rocks.bmp")
            }, true);

            build();
        }

        public void ChangeElevation(float offset, float radius, float hardness, MousePicker picker)
        {
            var position = GetPosition(picker);
            var toUpdate = getTilesInArea(position, radius);

            foreach (var i in toUpdate)
            {
                if (i >= indices.Count) continue;

                var vertex = vertices[indices[(int)i]];

                Func<float, float> calcP = (float t) => MathF.Pow(1f - t, 2) * MathF.Pow(1f + t, 2);

                var amount = Vector3.Distance(position, vertex);
                var elev = vertex.Y + calcP(MathF.Min(1f, MathF.Sqrt((amount / radius > hardness ? amount : 0.0f) / radius))) * offset;

                vertices[indices[(int)i]] = new Vector3(vertex.X, elev, vertex.Z);
            }

            updateNormals(position, radius);
            updateBuffers();
        }

        private void updateNormals(Vector3 center, float radius)
        {
            var toUpdate = getTilesInArea(center, radius);

            for (var i = 0; i < toUpdate.Count; i++)
            {
                var ci = toUpdate[i];
                if (ci < 0 || ci >= indices.Count) continue;
                updateNormal(ci);
            }
        }

        private void calculateNormals()
        {
            for (var i = 0; i < indices.Count; i += 3)
                updateNormal(i);

            updateBuffers();
        }

        private List<int> getTilesInArea(Vector3 center, float radius)
        {
            var included = new List<int>();
            var r = radius + 3;

            for (var z = center.Z - r; z <= center.Z + r; z++)
                for (var x = center.X - r; x <= center.X + r; x++)
                {
                    var index = getTileIndex(new Vector3(x, 0, z));
                    if (index == null) continue;
                    included.Add((int)index);
                }

            return included;
        }

        private void updateNormal(int i)
        {
            var v1 = vertices[indices[i]];
            var v2 = vertices[indices[i + 1]];
            var v3 = vertices[indices[i + 2]];

            normals[indices[i]] = Vector3.Cross(v2 - v1, v3 - v1).Normalized() * 100;
            normals[indices[i + 1]] = Vector3.Cross(v2 - v1, v3 - v1).Normalized() * 100;
            normals[indices[i + 2]] = Vector3.Cross(v2 - v1, v3 - v1).Normalized() * 100;
        }

        private void build()
        {
            var halfMapSize = (float)(mapSize / 2);
            var rnd = new Random();
            var i = 0;

            for (var z = -halfMapSize; z <= halfMapSize; z++)
            {   
                for (var x = -halfMapSize; x <= halfMapSize; x++)
                {
                    vertices.Add(new Vector3(x, 0f, z));
                    coords.Add(new Vector2((x + halfMapSize / 6), (z + halfMapSize / 6))); 
                    normals.Add(new Vector3(0f, 1f, 0f));

                    if (x < halfMapSize && z < halfMapSize)
                    {
                        indices.AddRange(new int[] {
                            (i), (i + mapSize + 1), (i + 1),
                            (i + 1), (i + mapSize + 1), (i + mapSize + 2)
                        });
                    }

                    i++;
                }
            }

            indexCount = indices.Count;

            vertexBuffer = GL.GenBuffer();
            coordBuffer = GL.GenBuffer();
            indexBuffer = GL.GenBuffer();
            normalBuffer = GL.GenBuffer();

            calculateNormals();
            updateBuffers();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(int), indices.ToArray(), BufferUsageHint.StaticDraw);
        }

        private int? getTileIndex(Vector3 position)
        {
            var x = (int)Math.Round(position.X + (mapSize / 2));
            if (x < 0 || x > mapSize) return null;

            var z = (int)Math.Round(position.Z + (mapSize / 2));
            if (z < 0 || z > mapSize) return null;

            return ((z * mapSize) + x) * 6;
        }

        private void updateBuffers()
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Count * Vector3.SizeInBytes, vertices.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, coordBuffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, coords.Count * Vector2.SizeInBytes, coords.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, normalBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, normals.Count * Vector3.SizeInBytes, normals.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);
        }

        public void Render(Camera camera, Light light)
        {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.UseProgram(shader.Program);

            GL.BindTexture(TextureTarget.Texture2DArray, texture.TextureId);
            GL.Uniform1(shader.Texture, 0);
            GL.Uniform3(shader.Ambient, 0.3f, 0.3f, 0.3f);
            GL.Uniform3(shader.Diffuse, 0.6f, 0.6f, 0.6f);
            GL.Uniform3(shader.Specular, 0.7f, 0.7f, 0.7f);
            GL.Uniform1(shader.Shininess, 50f);
            GL.Uniform1(shader.GridLines, ShowGridLines ? 1 : 0);

            camera.ApplyCamera(shader);
            light.ApplyLight(shader);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, coordBuffer);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, normalBuffer);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }
}