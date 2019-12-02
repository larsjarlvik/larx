using System;
using System.Collections.Generic;
using System.IO;
using Larx.GltfModel;
using Larx.Shadows;
using Larx.Storage;
using Larx.Terrain;
using Larx.Utils;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Assets
{
    public class AssetRenderer
    {
        private readonly AssetShader shader;
        private readonly ShadowShader shadowShader;
        public static string[] AssetKeys;
        private readonly Random random;
        private readonly AssetQuadTree assetQuadTree;

        public AssetRenderer()
        {
            AssetConfig.Models = new Dictionary<string, Model>();
            initializeModels();

            shader = new AssetShader();
            shadowShader = new ShadowShader();
            random = new Random();
            assetQuadTree = new AssetQuadTree();
        }

        private void initializeModels()
        {
            var assets = Directory.GetDirectories("resources/assets");
            AssetKeys = new string[assets.Length];

            for (var i = 0; i < assets.Length; i ++) {
                var directoryName = new DirectoryInfo(assets[i]).Name;
                var model = Model.Load("assets", directoryName);

                AssetKeys[i] = model.ModelName;
                AssetConfig.Models.Add(model.ModelName, model);
            }
        }

        public void Add(Vector2 position, TerrainRenderer terrain, string key)
        {
            var elev = terrain.HeightMap.GetElevationAtPoint(position);
            if (elev == null) return;

            if (!Map.MapData.Assets.ContainsKey(key))
                Map.MapData.Assets.Add(key, new List<PlacedAsset>());

            var count = (State.ToolHardness - 1) * State.ToolHardness + 1;
            var half = Map.MapData.MapSize / 2.0f;

            for(var i = 0; i < count; i ++) {
                var r = (float)(random.NextDouble() * (State.ToolRadius - 1.0f));
                var angle = (float)(random.NextDouble() * 2 * MathF.PI);
                var x = position.X + r * MathF.Cos(angle);
                var y = position.Y + r * MathF.Sin(angle);
                var scale = 1.0f + ((float)(random.NextDouble() * 0.5f) - 0.25f);

                if (x < -half || x >= half - 1 ||
                    y < -half || y >= half - 1)
                    continue;

                Map.MapData.Assets[key].Add(new PlacedAsset(key, new Vector2(x, y), MathLarx.DegToRad((float)random.NextDouble() * 360.0f), scale));
            }

            assetQuadTree.UpdateQuadTree(terrain);
        }

        public void Refresh(TerrainRenderer terrain)
        {
            assetQuadTree.UpdateQuadTree(terrain);
        }

        public void Remove(Vector2 position, TerrainRenderer terrain)
        {
            foreach(var assetType in Map.MapData.Assets)
                assetType.Value.RemoveAll(x => Vector2.Distance(position, x.Position) <= State.ToolRadius);

            assetQuadTree.UpdateQuadTree(terrain);
        }

        public void Render(Camera camera, Light light, ShadowBox shadows, TerrainRenderer terrain, ClipPlane clip = ClipPlane.None)
        {
            GL.UseProgram(shader.Program);

            shader.ApplyCamera(camera);
            shader.ApplyLight(light);
            shader.ApplyShadows(shadows);

            GL.Uniform1(shader.ClipPlane, (int)clip);
            GL.Uniform4(shader.FogColor, State.ClearColor);
            GL.Uniform1(shader.FarPlane, State.Far);

            assetQuadTree.Render(camera.FrustumPlanes, shader);

            GL.DisableVertexAttribArray(4);
            GL.DisableVertexAttribArray(5);
            GL.DisableVertexAttribArray(6);
        }

        public void RenderShadowMap(ShadowBox shadows, TerrainRenderer terrain, ClipPlane clip = ClipPlane.None)
        {
            GL.UseProgram(shadowShader.Program);

            GL.UniformMatrix4(shadowShader.ViewMatrix, false, ref shadows.ViewMatrix);
            GL.UniformMatrix4(shadowShader.ProjectionMatrix, false, ref shadows.ProjectionMatrix);

            GL.Uniform1(shadowShader.ClipPlane, (int)clip);

            assetQuadTree.Render(shadows.ShadowFrustumPlanes, shader);

            GL.DisableVertexAttribArray(4);
            GL.DisableVertexAttribArray(5);
            GL.DisableVertexAttribArray(6);
        }
    }
}