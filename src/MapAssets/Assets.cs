using System;
using System.Collections.Generic;
using Larx.GltfModel;
using Larx.Shadows;
using Larx.Storage;
using Larx.Terrain;
using Larx.Utils;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.MapAssets
{
    public class Assets : AssetRenderer
    {
        public static readonly string[] AssetKeys = new string[] {
            "tree-0",
            "tree-1",
            "tree-2",
            "tree-3",
            "tree-4",
            "tree-5",
            "rock",
            "rock-1",
            "rock-2",
            "grass"
        };

        private readonly Dictionary<string, Model> models;
        private readonly Random random;

        public Assets()
        {
            models = new Dictionary<string, Model>();
            random = new Random();

            foreach(var asset in AssetKeys) {
                var model = Model.Load(asset);
                AppendBuffers(model);
                models.Add(asset, model);
                Map.MapData.Assets.Add(asset, new List<PlacedAsset>());
            }
        }

        public void Refresh(TerrainRenderer terrain)
        {
            foreach(var model in models.Values) {
                Refresh(model, terrain);
            }
        }

        public void Add(Vector2 position, TerrainRenderer terrain, string key)
        {
            var elev = terrain.HeightMap.GetElevationAtPoint(position);
            if (elev == null) return;

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

                Map.MapData.Assets[key].Add(new PlacedAsset(new Vector2(x, y), MathLarx.DegToRad((float)random.NextDouble() * 360.0f), scale));
            }

            Refresh(models[key], terrain);
        }

        public void Remove(Vector2 position, TerrainRenderer terrain)
        {
            foreach(var assetType in Map.MapData.Assets) {
                assetType.Value.RemoveAll(x => Vector2.Distance(position, x.Position) <= State.ToolRadius);
            }
            Refresh(terrain);
        }

        public void Render(Camera camera, Light light, ShadowBox shadows, TerrainRenderer terrain, ClipPlane clip = ClipPlane.None)
        {
            GL.UseProgram(Shader.Program);

            Shader.ApplyCamera(camera);
            Shader.ApplyLight(light);
            Shader.ApplyShadows(shadows);

            GL.Uniform1(Shader.ClipPlane, (int)clip);
            GL.Uniform4(Shader.FogColor, State.ClearColor);
            GL.Uniform1(Shader.FarPlane, State.Far);

            foreach(var key in models.Keys)
            {
                if (Map.MapData.Assets[key].Count == 0) continue;
                Render(models[key], key);
            }

            GL.DisableVertexAttribArray(4);
            GL.DisableVertexAttribArray(5);
            GL.DisableVertexAttribArray(6);
        }

        public void RenderShadowMap(ShadowBox shadows, TerrainRenderer terrain, ClipPlane clip = ClipPlane.None)
        {
            GL.UseProgram(ShadowShader.Program);

            GL.UniformMatrix4(ShadowShader.ViewMatrix, false, ref shadows.ViewMatrix);
            GL.UniformMatrix4(ShadowShader.ProjectionMatrix, false, ref shadows.ProjectionMatrix);

            GL.Uniform1(ShadowShader.ClipPlane, (int)clip);

            foreach(var key in models.Keys)
            {
                if (Map.MapData.Assets[key].Count == 0) continue;
                RenderShadowMap(models[key], key);
            }

            GL.DisableVertexAttribArray(4);
            GL.DisableVertexAttribArray(5);
            GL.DisableVertexAttribArray(6);
        }
    }
}