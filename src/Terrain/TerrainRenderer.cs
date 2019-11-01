using System;
using System.Collections.Generic;
using System.IO;
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

        private int vertexBuffer;
        private int coordBuffer;
        private int indexBuffer;
        private int normalBuffer;
        private int tangentBuffer;
        private int vaoId;
        private Vector3[,] vertices;
        private Vector2[,] coords;
        private Vector3[,] normals;
        private Vector3[,] tangents;
        private List<int> indices = new List<int>();

        private readonly SplatMap splatMap;
        private readonly TextureNoise textureNoise;
        private readonly TerrainShader shader;
        private readonly ShadowShader shadowShader;
        private readonly Texture texture;
        public readonly TerrainPicker Picker;

        public Vector3 MousePosition;
        private float halfMapSize;

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
            shadowShader = new ShadowShader();
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
            var toUpdate = getTilesInArea(MousePosition, State.ToolRadius);

            foreach (var i in toUpdate)
            {
                var vertex = vertices[i.X, i.Y];

                Func<float, float> calcP = (float t) => MathF.Pow(1f - t, 2) * MathF.Pow(1f + t, 2);

                var amount = Vector3.Distance(MousePosition, vertex);
                var elev = vertex.Y + calcP(MathF.Min(1f, MathF.Sqrt((amount / State.ToolRadius > (State.ToolHardness * 0.1f) ? amount : 0.0f) / State.ToolRadius))) * offset;

                vertices[i.X, i.Y] = new Vector3(vertex.X, elev, vertex.Z);
                updateNormals(i.X, i.Y);
            }

            updateBuffers();
        }

        private void updateNormals(int x, int z)
        {
            var offX = x > halfMapSize ? -1 : 1;
            var offZ = z > halfMapSize ? -1 : 1;

            var v1 = vertices[x, z];
            var v2 = vertices[x + offX, z];
            var v3 = vertices[x, z + offZ];

            normals[x, z] = Vector3.Cross(
                offX == -1 ? v2 - v1 : v1 - v2,
                offZ == -1 ? v3 - v1 : v1 - v3
            );
            tangents[x, z] = MathLarx.CalculateTangent(normals[x, z]);
        }

        public void Paint()
        {
            if (State.ActiveToolBarItem == null) return;

            var position = (MousePosition.Xz / Map.MapData.MapSize);
            position.X = (position.X + 0.5f) * State.SplatDetail;
            position.Y = (position.Y + 0.5f) * State.SplatDetail;

            splatMap.Update(position, byte.Parse(State.ActiveToolBarItem));
        }

        public float[,] GetTerrainElevations()
        {
            var result = new float[Map.MapData.MapSize + 1, Map.MapData.MapSize + 1];
            for (var z = 0; z <= Map.MapData.MapSize; z++)
                for (var x = 0; x <= Map.MapData.MapSize; x++)
                    result[x, z] = vertices[x, z].Y;

            return result;
        }

        private List<Point> getTilesInArea(Vector3 center, float radius)
        {
            var included = new List<Point>();
            var r = radius + 2;

            for (var z = center.Z - r + halfMapSize; z <= center.Z + r + halfMapSize; z++)
                for (var x = center.X - r + halfMapSize; x <= center.X + r + halfMapSize; x++)
                {
                    if (x > 0 && z > 0 && x <= Map.MapData.MapSize + 1 && z <= Map.MapData.MapSize + 1)
                        included.Add(new Point((int)x, (int)z));
                }

            return included;
        }

        public void Build()
        {
            halfMapSize = Map.MapData.MapSize / 2.0f;
            var rnd = new Random();
            var i = 0;
            var size = Map.MapData.MapSize + 1;

            vertices = new Vector3[size, size];
            coords = new Vector2[size, size];
            normals = new Vector3[size, size];
            tangents = new Vector3[size, size];
            indices.Clear();

            for (var z = 0; z <= Map.MapData.MapSize; z++)
                for (var x = 0; x <= Map.MapData.MapSize; x++)
                {
                    vertices[x, z] = new Vector3(x - halfMapSize, Map.MapData.TerrainElevations[x, z], z - halfMapSize);
                    coords[x, z] = new Vector2((float)x / Map.MapData.MapSize, (float)z / Map.MapData.MapSize);
                    normals[x, z] = new Vector3(0f, 1f, 0f);
                    tangents[x, z] = new Vector3(1f, 0f, 0f);

                    if (x > 0.0f && z > 0.0f)
                        indices.AddRange(new int[] {
                            i - Map.MapData.MapSize - 1, i, i - 1,
                            i - Map.MapData.MapSize - 1, i - 1, i - Map.MapData.MapSize - 2
                        });

                    i++;
                }


            for (var z = 0; z <= Map.MapData.MapSize; z++)
                for (var x = 0; x <= Map.MapData.MapSize; x++)
                    updateNormals(x, z);

            vaoId = GL.GenVertexArray();
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

        public float? GetElevationAtPoint(Vector2 position)
        {
            Vector3 index0;
            Vector3 index1;
            Vector3 index2;

            var x = (int)(position.X + halfMapSize);
            var z = (int)(position.Y + halfMapSize);

            if (x < 0 || x >= Map.MapData.MapSize ||
                z < 0 || z >= Map.MapData.MapSize)
                return 1.0f;

            if ((position.X % 1) + (position.Y % 1) > 1.0f) {
                index0 = vertices[x, z];
                index1 = vertices[x + 1, z];
                index2 = vertices[x, z + 1];
            } else {
                index0 = vertices[x + 1, z];
                index1 = vertices[x, z + 1];
                index2 = vertices[x + 1, z + 1];;
            }

            return MathLarx.BaryCentric(index0, index1, index2, new Vector2(position.X, position.Y));
        }

        private void updateBuffers()
        {
            GL.BindVertexArray(vaoId);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Length * Vector3.SizeInBytes, vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, coordBuffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, coords.Length * Vector2.SizeInBytes, coords, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, normalBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, normals.Length * Vector3.SizeInBytes, normals, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, tangentBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, tangents.Length * Vector3.SizeInBytes, tangents, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.BindVertexArray(0);
        }

        public void Update()
        {
            MousePosition = Picker.GetPosition();
        }

        public void Render(Camera camera, Light light, ShadowRenderer shadows, bool showOverlays, ClipPlane clip = ClipPlane.None)
        {
            GL.UseProgram(shader.Program);

            GL.Uniform3(shader.MousePosition, MousePosition);
            GL.Uniform1(shader.SelectionSize, State.ToolRadius);
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

            GL.BindVertexArray(vaoId);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindVertexArray(0);
        }

        public void RenderShadowMap(ShadowRenderer shadows)
        {
            GL.EnableVertexAttribArray(0);
            GL.UseProgram(shadowShader.Program);

            GL.UniformMatrix4(shadowShader.ViewMatrix, false, ref shadows.ViewMatrix);
            GL.UniformMatrix4(shadowShader.ProjectionMatrix, false, ref shadows.ProjectionMatrix);

            GL.BindVertexArray(vaoId);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.DrawElements(PrimitiveType.Triangles, indices.Count, DrawElementsType.UnsignedInt, IntPtr.Zero);
        }
    }
}