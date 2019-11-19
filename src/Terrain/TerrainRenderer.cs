using System;
using System.Collections.Generic;
using System.IO;
using Larx.Shadows;
using Larx.Storage;
using Larx.Terrain.Shaders;
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
        private int vaoId;
        private RenderShader shader;
        private ShadowShader shadowShader;
        private TerrainQuadTree quadTree;
        private Matrix4 worldTransform;
        private Vector3 lastCameraPosition;
        private TerrainPicker picker;
        private readonly Texture texture;
        private readonly TextureNoise textureNoise;
        private readonly Camera camera;
        public readonly NormalMap NormalMap;
        public readonly SplatMap SplatMap;
        public readonly HeightMap HeightMap;
        public Vector3 MousePosition { get; private set; }

        private Vector3[] frustumBox;

        public TerrainRenderer(Camera camera)
        {
            this.camera = camera;
            shader = new RenderShader();
            shadowShader = new ShadowShader();
            quadTree = new TerrainQuadTree();
            worldTransform = Matrix4.CreateScale(Map.MapData.MapSize) * Matrix4.CreateTranslation(-Map.MapData.MapSize / 2.0f, 0.0f, -Map.MapData.MapSize / 2.0f);
            MousePosition = new Vector3();

            texture = new Texture();
            textureNoise = new TextureNoise(12312234);
            NormalMap = new NormalMap();
            HeightMap = new HeightMap(NormalMap);
            SplatMap = new SplatMap();
            picker = new TerrainPicker(camera, HeightMap);
            loadTextures();

            build();
            Update();
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

        public void Update()
        {
            MousePosition = picker.GetPosition();
            frustumBox = camera.GetFrustumBox(State.Near, State.Far);

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

        public void Render(Camera camera, Light light, ShadowBox shadows, ClipPlane clipPlane = ClipPlane.None)
        {
            GL.UseProgram(shader.Program);

            shader.ApplyCamera(camera);
            shader.ApplyLight(light);
            shader.ApplyShadows(shadows);

            GL.UniformMatrix4(shader.WorldMatrix, false, ref worldTransform);
            GL.Uniform1(shader.TessFactor, TerrainConfig.TessFactor);
            GL.Uniform1(shader.TessSlope, TerrainConfig.TessSlope);
            GL.Uniform1(shader.TessShift, TerrainConfig.TessShift);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, HeightMap.Texture);
            GL.Uniform1(shader.HeightMap, 0);
            GL.Uniform1(shader.HeightMapScale, TerrainConfig.HeightMapScale);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, NormalMap.Texture.TextureId);
            GL.Uniform1(shader.NormalMap, 1);

            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2DArray, texture.TextureId);
            GL.Uniform1(shader.Texture, 2);

            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2DArray, SplatMap.Texture);
            GL.Uniform1(shader.SplatMap, 3);
            GL.Uniform1(shader.SplatCount, TerrainConfig.Textures.Length);

            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.Texture2D, textureNoise.Texture);
            GL.Uniform1(shader.TextureNoise, 4);

            GL.Uniform1(shader.ClipPlane, (int)clipPlane);
            GL.Uniform1(shader.GridLines, State.ShowGridLines ? 1 : 0);
            GL.Uniform3(shader.MousePosition, MousePosition);
            GL.Uniform1(shader.SelectionSize, State.ToolRadius);

            GL.Uniform4(shader.FogColor, Color.FromArgb(255, 124, 151, 185));
            GL.Uniform1(shader.FarPlane, State.Far);

            for (int i = 0; i < 8; i++){
                GL.Uniform1(shader.LodMorphAreas[i], TerrainConfig.LodMorphAreas[i]);
            }

            GL.BindVertexArray(vaoId);
            GL.EnableVertexAttribArray(0);
            quadTree.Render(shader, frustumBox);

            GL.BindVertexArray(0);
        }

        internal void RenderShadowMap(Camera camera, ShadowBox shadows, ClipPlane clipPlane = ClipPlane.None)
        {
            GL.UseProgram(shadowShader.Program);

            GL.UniformMatrix4(shadowShader.ViewMatrix, false, ref shadows.ViewMatrix);
            GL.UniformMatrix4(shadowShader.ProjectionMatrix, false, ref shadows.ProjectionMatrix);
            GL.UniformMatrix4(shadowShader.WorldMatrix, false, ref worldTransform);

            GL.Uniform3(shadowShader.CameraPosition, camera.Position);
            GL.Uniform1(shadowShader.TessFactor, TerrainConfig.TessFactor);
            GL.Uniform1(shadowShader.TessSlope, TerrainConfig.TessSlope);
            GL.Uniform1(shadowShader.TessShift, TerrainConfig.TessShift);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, HeightMap.Texture);
            GL.Uniform1(shadowShader.HeightMap, 0);
            GL.Uniform1(shadowShader.HeightMapScale, TerrainConfig.HeightMapScale);

            GL.Uniform1(shadowShader.ClipPlane, (int)clipPlane);
            for (int i = 0; i < 8; i++){
                GL.Uniform1(shadowShader.LodMorphAreas[i], TerrainConfig.LodMorphAreas[i]);
            }

            GL.BindVertexArray(vaoId);
            GL.EnableVertexAttribArray(0);
            quadTree.Render(shadowShader, frustumBox);

            GL.BindVertexArray(0);
        }
    }
}