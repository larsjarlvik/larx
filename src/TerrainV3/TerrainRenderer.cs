using System.Collections.Generic;
using System.IO;
using Larx.Storage;
using Larx.Terrain;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.TerrainV3
{
    public class TerrainRenderer
    {
        private int vaoId;
        private TerrainShader shader;
        private TerrainQuadTree quadTree;
        private Matrix4 worldTransform;
        private Vector3 lastCameraPosition;
        private TerrainPicker picker;
        private readonly Texture texture;
        private HeightMap heightMap;
        private NormalMap normalMap;
        public Vector3 MousePosition { get; private set; }

        public TerrainRenderer()
        {
            shader = new TerrainShader();
            quadTree = new TerrainQuadTree();
            worldTransform = Matrix4.CreateScale(Map.MapData.MapSize) * Matrix4.CreateTranslation(-Map.MapData.MapSize / 2.0f, 0.0f, -Map.MapData.MapSize / 2.0f);
            picker = new TerrainPicker();
            MousePosition = new Vector3();

            texture = new Texture();
            heightMap = new HeightMap();
            normalMap = new NormalMap();
            normalMap.Generate(heightMap.Texture);
            loadTextures();

            build();
        }

        private void loadTextures()
        {
            var paths = new List<string>();
            foreach(var texture in TerrainConfig.Textures) {
                paths.Add(Path.Combine("resources", "textures", $"{texture}-albedo.png"));
                paths.Add(Path.Combine("resources", "textures", $"{texture}-normal.png"));
                paths.Add(Path.Combine("resources", "textures", $"{texture}-specular.png"));
            }
            texture.LoadTexture(paths.ToArray(), true);
        }

        public void Update(Camera camera)
        {
            MousePosition = picker.GetPosition(camera);

            if (camera.Position == lastCameraPosition)
                return;

            quadTree.UpdateQuadTree(camera);
            lastCameraPosition = camera.Position;
        }

        private void build()
        {
            var vertices = new Vector2[] {
                new Vector2(0,0),
                new Vector2(0.333f,0),
                new Vector2(0.666f,0),
                new Vector2(1,0),

                new Vector2(0,0.333f),
                new Vector2(0.333f,0.333f),
                new Vector2(0.666f,0.333f),
                new Vector2(1,0.333f),

                new Vector2(0,0.666f),
                new Vector2(0.333f,0.666f),
                new Vector2(0.666f,0.666f),
                new Vector2(1,0.666f),

                new Vector2(0,1),
                new Vector2(0.333f,1),
                new Vector2(0.666f,1),
                new Vector2(1,1),
            };

            vaoId = GL.GenVertexArray();
            GL.BindVertexArray(vaoId);

            var vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, vertices.Length * Vector2.SizeInBytes, vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 16);

            GL.BindVertexArray(0);
        }

        public void Render(Camera camera, Light light, ClipPlane clipPlane = ClipPlane.None)
        {
            GL.UseProgram(shader.Program);

            shader.ApplyCamera(camera);
            shader.ApplyLight(light);

            GL.UniformMatrix4(shader.WorldMatrix, false, ref worldTransform);
            GL.Uniform1(shader.TessFactor, TerrainConfig.TessFactor);
            GL.Uniform1(shader.TessSlope, TerrainConfig.TessSlope);
            GL.Uniform1(shader.TessShift, TerrainConfig.TessShift);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, heightMap.Texture);
            GL.Uniform1(shader.HeightMap, 0);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, normalMap.Texture.TextureId);
            GL.Uniform1(shader.NormalMap, 1);

            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2DArray, texture.TextureId);
            GL.Uniform1(shader.Texture, 2);

            GL.Uniform1(shader.ClipPlane, (int)clipPlane);

            for (int i = 0; i < 8; i++){
                GL.Uniform1(shader.LodMorphAreas[i], TerrainConfig.LodMorphAreas[i]);
            }

            GL.BindVertexArray(vaoId);
            GL.EnableVertexAttribArray(0);
            quadTree.Render(shader);

            GL.BindVertexArray(0);
        }
    }
}