using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Larx.Utils;
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

        private readonly SplatMap splatMap;
        private readonly TextureNoise textureNoise;
        private readonly TerrainShader shader;
        private readonly Texture texture;

        public TerrainRenderer()
        {
            var textures = new [] {
                "grass",
                "rocky-grass",
                "cliff",
                "sand",
                "snow",
            };

            shader = new TerrainShader();
            splatMap = new SplatMap(textures.Length);
            textureNoise = new TextureNoise(12312234);

            texture = new Texture();

            loadTextures(textures);
            build();
        }

        private void loadTextures(string[] textures)
        {
            var paths = new List<string>();
            foreach(var texture in textures) {
                paths.Add(Path.Combine("resources", "textures", $"{texture}-albedo.bmp"));
                paths.Add(Path.Combine("resources", "textures", $"{texture}-normal.bmp"));
                paths.Add(Path.Combine("resources", "textures", $"{texture}-rough.bmp"));
            }
            texture.LoadTexture(paths.ToArray(), true);
        }

        public void ChangeElevation(float offset, MousePicker picker)
        {
            var position = GetPosition(picker, this);
            var toUpdate = getTilesInArea(position, State.ToolRadius);

            foreach (var i in toUpdate)
            {
                if (i >= indices.Count) continue;

                var vertex = vertices[indices[(int)i]];

                Func<float, float> calcP = (float t) => MathF.Pow(1f - t, 2) * MathF.Pow(1f + t, 2);

                var amount = Vector3.Distance(position, vertex);
                var elev = vertex.Y + calcP(MathF.Min(1f, MathF.Sqrt((amount / State.ToolRadius > State.ToolHardness ? amount : 0.0f) / State.ToolRadius))) * offset;

                vertices[indices[(int)i]] = new Vector3(vertex.X, elev, vertex.Z);

                var v1 = vertices[indices[i]];
                var v2 = vertices[indices[i + 1]];
                var v3 = vertices[indices[i + 2]];
                normals[indices[i]] = Vector3.Cross(v2 - v1, v3 - v1).Normalized();
            }

            updateBuffers();
        }

        public void Paint(MousePicker picker)
        {
            var position = (GetPosition(picker, this).Xz / mapSize);
            position.X = (position.X + 0.5f) * SplatMap.Detail;
            position.Y = (position.Y + 0.5f) * SplatMap.Detail;

            var splatMapRadius = (int)(State.ToolRadius * SplatMap.Detail / mapSize);

            splatMap.Update(position, splatMapRadius, State.ActiveTexture);
        }

        private List<int> getTilesInArea(Vector3 center, float radius)
        {
            var included = new List<int>();
            var r = radius + 2;

            for (var z = center.Z - r; z <= center.Z + r; z++)
                for (var x = center.X - r; x <= center.X + r; x++)
                {
                    var index = getTileIndex(new Vector3(x, 0, z));
                    if (index == null) continue;
                    included.Add((int)index);
                }

            return included;
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
                    coords.Add(new Vector2((x + halfMapSize) / mapSize, (z + halfMapSize) / mapSize));
                    normals.Add(new Vector3(0f, 1f, 0f).Normalized());

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

            updateBuffers();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(int), indices.ToArray(), BufferUsageHint.StaticDraw);
        }

        private int? getTileIndex(Vector3 position)
        {
            var x = (int)MathF.Round(position.X + (mapSize / 2));
            if (x < 0 || x > mapSize) return null;

            var z = (int)MathF.Round(position.Z + (mapSize / 2));
            if (z < 0 || z > mapSize) return null;

            var index = ((z * mapSize) + x) * 6;
            if (index >= indices.Count) return null;

            return index;
        }

        public float? GetElevationAtPoint(Vector3 position)
        {
            int? index0;
            int? index1;
            int? index2;

            if ((position.X % 1) + (position.Z % 1) > 1.0f) {
                index0 = getTileIndex(new Vector3(MathF.Floor(position.X), position.Y, MathF.Floor(position.Z)));
                index1 = getTileIndex(new Vector3(MathF.Floor(position.X) + 1, position.Y, MathF.Floor(position.Z)));
                index2 = getTileIndex(new Vector3(MathF.Floor(position.X), position.Y, MathF.Floor(position.Z) + 1));
            } else {
                index0 = getTileIndex(new Vector3(MathF.Floor(position.X) + 1, position.Y, MathF.Floor(position.Z)));
                index1 = getTileIndex(new Vector3(MathF.Floor(position.X), position.Y, MathF.Floor(position.Z) + 1));
                index2 = getTileIndex(new Vector3(MathF.Floor(position.X) + 1, position.Y, MathF.Floor(position.Z) + 1));
            }

            if (index0 == null || index1 == null || index2 == null) return null;

            return MathLarx.BaryCentric(vertices[indices[(int)index0]], vertices[indices[(int)index1]], vertices[indices[(int)index2]], new Vector2(position.X, position.Z));
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


            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2DArray, texture.TextureId);
            GL.Uniform1(shader.Texture, 0);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2DArray, splatMap.Texture);
            GL.Uniform1(shader.SplatMap, 1);
            GL.Uniform1(shader.SplatCount, splatMap.SplatCount);

            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, textureNoise.Texture);
            GL.Uniform1(shader.TextureNoise, 2);

            GL.Uniform3(shader.Ambient, 0.3f, 0.3f, 0.3f);
            GL.Uniform3(shader.Diffuse, 0.6f, 0.6f, 0.6f);
            GL.Uniform3(shader.Specular, 0.8f, 0.8f, 0.8f);
            GL.Uniform1(shader.Shininess, 2f);
            GL.Uniform1(shader.GridLines, State.ShowGridLines ? 1 : 0);

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

            GL.ActiveTexture(TextureUnit.Texture0);
        }
    }
}