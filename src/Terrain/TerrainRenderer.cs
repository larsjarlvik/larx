using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Larx.Shadows;
using Larx.Storage;
using Larx.Utils;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Terrain
{
    public enum ClipPlane {
        None = 0,
        ClipBottom = 1,
        ClipTop = 2,
    }

    public class TerrainRenderer
    {
        public static readonly string[] Textures;

        private int indexCount = 0;
        private int vertexBuffer;
        private int coordBuffer;
        private int indexBuffer;
        private int normalBuffer;
        private int tangentBuffer;
        private List<Vector3> vertices = new List<Vector3>();
        private List<Vector2> coords = new List<Vector2>();
        private List<Vector3> normals = new List<Vector3>();
        private List<Vector3> tangents = new List<Vector3>();
        private List<int> indices = new List<int>();

        private readonly SplatMap splatMap;
        private readonly TextureNoise textureNoise;
        private readonly TerrainShader shader;
        private readonly Texture texture;
        public readonly TerrainPicker Picker;

        public Vector3 MousePosition;

        static TerrainRenderer()
        {
            Textures = new [] {
                "grass-1",
                "grass-2",
                "grass-3",
                "grass-4",
                "bare-1",
                "bare-2",
                "bare-3",
                "sand-1",
                "sand-2",
                "sand-3",
                "sand-4",
                "rock-1",
                "rock-2"
            };
        }

        public TerrainRenderer(Camera camera)
        {
            shader = new TerrainShader();
            splatMap = new SplatMap();
            textureNoise = new TextureNoise(12312234);
            Picker = new TerrainPicker(this, camera);
            texture = new Texture();

            loadTextures(Textures);
            Build();
        }

        private void loadTextures(string[] textures)
        {
            var paths = new List<string>();
            foreach(var texture in textures) {
                paths.Add(Path.Combine("resources", "textures", $"{texture}-albedo.png"));
                paths.Add(Path.Combine("resources", "textures", $"{texture}-normal.png"));
                paths.Add(Path.Combine("resources", "textures", $"{texture}-specular.png"));
            }
            texture.LoadTexture(paths.ToArray(), true);
        }

        public void ChangeElevation(float offset)
        {
            var toUpdate = getTilesInArea(MousePosition, State.SelectionCircleRadius);

            foreach (var i in toUpdate)
            {
                if (i >= indices.Count) continue;

                var vertex = vertices[indices[i]];

                Func<float, float> calcP = (float t) => MathF.Pow(1f - t, 2) * MathF.Pow(1f + t, 2);

                var amount = Vector3.Distance(MousePosition, vertex);
                var elev = vertex.Y + calcP(MathF.Min(1f, MathF.Sqrt((amount / State.ToolRadius > State.ToolHardness ? amount : 0.0f) / State.ToolRadius))) * offset;

                vertices[indices[i]] = new Vector3(vertex.X, elev, vertex.Z);
                updateNormals(i);
            }

            updateBuffers();
        }

        private void updateNormals(int i)
        {
            var v1 = vertices[indices[i]];
            var v2 = vertices[indices[i + 1]];
            var v3 = vertices[indices[i + 2]];
            normals[indices[i]] = Vector3.Cross(v2 - v1, v3 - v1);
            tangents[indices[i]] = MathLarx.CalculateTangent(normals[indices[i]]);
        }

        public void Paint()
        {
            if (State.ActiveToolBarItem == null) return;

            var position = (MousePosition.Xz / Map.MapData.MapSize);
            position.X = (position.X + 0.5f) * State.SplatDetail;
            position.Y = (position.Y + 0.5f) * State.SplatDetail;

            splatMap.Update(position, byte.Parse(State.ActiveToolBarItem));
        }

        public float[] GetTerrainElevations()
        {
            return vertices.Select(v => v.Y).ToArray();
        }

        private List<int> getTilesInArea(Vector3 center, float radius)
        {
            var included = new List<int>();
            var r = radius + 2;

            for (var z = center.Z - r; z <= center.Z + r; z++)
                for (var x = center.X - r; x <= center.X + r; x++)
                {
                    var index = getTileIndex(new Vector2(x, z));
                    if (index == null) continue;
                    included.Add((int)index);
                }

            return included;
        }

        public void Build()
        {
            var halfMapSize = Map.MapData.MapSize / 2;
            var rnd = new Random();
            var i = 0;

            vertices.Clear();
            coords.Clear();
            normals.Clear();
            tangents.Clear();
            indices.Clear();

            for (var z = -halfMapSize; z <= halfMapSize; z++)
            {
                for (var x = -halfMapSize; x <= halfMapSize; x++)
                {
                    vertices.Add(new Vector3(x, Map.MapData.TerrainElevations[i], z));
                    coords.Add(new Vector2((float)(x + halfMapSize) / Map.MapData.MapSize, (float)(z + halfMapSize) / Map.MapData.MapSize));
                    normals.Add(new Vector3(0f, 1f, 0f).Normalized());
                    tangents.Add(MathLarx.CalculateTangent(normals.Last()));

                    if (x < halfMapSize && z < halfMapSize)
                    {
                        indices.AddRange(new int[] {
                            i,     i + Map.MapData.MapSize + 1, i + 1,
                            i + 1, i + Map.MapData.MapSize + 1, i + Map.MapData.MapSize + 2
                        });
                    }

                    i++;
                }
            }

            for(var n = 0; n < indices.Count; n += 3) {
                updateNormals(n);
            }

            indexCount = indices.Count;

            vertexBuffer = GL.GenBuffer();
            coordBuffer = GL.GenBuffer();
            indexBuffer = GL.GenBuffer();
            normalBuffer = GL.GenBuffer();
            tangentBuffer = GL.GenBuffer();

            updateBuffers();

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Count * sizeof(int), indices.ToArray(), BufferUsageHint.StaticDraw);

            for(var n = 0; n < Textures.Length; n ++)
                splatMap.ToTexture(n);
        }

        private int? getTileIndex(Vector2 position)
        {
            var x = (int)MathF.Round(position.X + (Map.MapData.MapSize / 2));
            if (x < 0 || x > Map.MapData.MapSize) return null;

            var z = (int)MathF.Round(position.Y + (Map.MapData.MapSize / 2));
            if (z < 0 || z > Map.MapData.MapSize) return null;

            var index = ((z * Map.MapData.MapSize) + x) * 6;
            if (index >= indices.Count) return null;

            return index;
        }

        public float? GetElevationAtPoint(Vector2 position)
        {
            int? index0;
            int? index1;
            int? index2;

            if ((position.X % 1) + (position.Y % 1) > 1.0f) {
                index0 = getTileIndex(new Vector2(MathF.Floor(position.X), MathF.Floor(position.Y)));
                index1 = getTileIndex(new Vector2(MathF.Floor(position.X) + 1, MathF.Floor(position.Y)));
                index2 = getTileIndex(new Vector2(MathF.Floor(position.X), MathF.Floor(position.Y) + 1));
            } else {
                index0 = getTileIndex(new Vector2(MathF.Floor(position.X) + 1, MathF.Floor(position.Y)));
                index1 = getTileIndex(new Vector2(MathF.Floor(position.X), MathF.Floor(position.Y) + 1));
                index2 = getTileIndex(new Vector2(MathF.Floor(position.X) + 1, MathF.Floor(position.Y) + 1));
            }

            if (index0 == null || index1 == null || index2 == null) return null;

            return MathLarx.BaryCentric(vertices[indices[(int)index0]], vertices[indices[(int)index1]], vertices[indices[(int)index2]], new Vector2(position.X, position.Y));
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

            GL.BindBuffer(BufferTarget.ArrayBuffer, tangentBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, tangents.Count * Vector3.SizeInBytes, tangents.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, 0, 0);
        }

        public void Update()
        {
            MousePosition = Picker.GetPosition();
        }

        public void Render(Camera camera, Light light, ShadowRenderer shadows, bool showOverlays, ClipPlane clip = ClipPlane.None)
        {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);

            GL.UseProgram(shader.Program);

            GL.Uniform3(shader.MousePosition, MousePosition);
            GL.Uniform1(shader.SelectionSize, State.SelectionCircleRadius);
            GL.Uniform1(shader.GridLines, State.ShowGridLines ? 1 : 0);
            GL.Uniform1(shader.ShowOverlays, showOverlays ? 1 : 0);

            GL.Uniform1(shader.ClipPlane, (int)clip);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2DArray, texture.TextureId);
            GL.Uniform1(shader.Texture, 0);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2DArray, splatMap.Texture);
            GL.Uniform1(shader.SplatMap, 1);
            GL.Uniform1(shader.SplatCount, TerrainRenderer.Textures.Length);

            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, textureNoise.Texture);
            GL.Uniform1(shader.TextureNoise, 2);

            shader.ApplyCamera(camera);
            shader.ApplyLight(light);
            shader.ApplyShadows(shadows);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, coordBuffer);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, normalBuffer);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, tangentBuffer);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, IntPtr.Zero);

            GL.ActiveTexture(TextureUnit.Texture0);
        }
    }
}