using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Larx.GltfModel;
using Larx.Shadows;
using Larx.Storage;
using Larx.Terrain;
using Larx.UserInterFace;
using Larx.Utils;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.MapAssets
{
    public class Assets : AssetRenderer
    {
        private readonly string[] modelNames = new string[] {
            "tree",
            "rock",
            "grass"
        };
        private readonly Dictionary<string, Asset> models;
        private readonly Random random;

        public Assets(Ui ui)
        {
            models = new Dictionary<string, Asset>();
            random = new Random();

            foreach(var modelName in modelNames) {
                ui.Tools.Add(new ToolbarItem(TopMenu.Assets, ui.AddButton(modelName, $"ui/assets/{modelName}.png")));

                var config = File.ReadAllText($"resources/models/{modelName}/{modelName}.json");
                var asset = JsonSerializer.Deserialize<Asset>(config, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                asset.Model = Model.Load(modelName);
                models.Add(modelName, asset);
                Map.MapData.Assets.Add(modelName, new List<PlacedAsset>());
            }
        }

        public void Add(Vector2 position, TerrainRenderer terrain)
        {
            if (State.ActiveToolBarItem == null) return;

            var elev = terrain.GetElevationAtPoint(position);
            if (elev == null) return;

            var count = (State.ToolHardness - 1) * State.ToolHardness + 1;

            for(var i = 0; i < count; i ++) {
                var r = (float)(random.NextDouble() * (State.ToolRadius - 1.0f));
                var angle = (float)(random.NextDouble() * 2 * MathF.PI);
                var x = position.X + r * MathF.Cos(angle);
                var y = position.Y + r * MathF.Sin(angle);
                var variation = (float)random.NextDouble() * models[State.ActiveToolBarItem].RenderDistanceVariation;
                Console.WriteLine(variation);

                Map.MapData.Assets[State.ActiveToolBarItem].Add(
                    new PlacedAsset(new Vector2(x, y), MathLarx.DegToRad((float)random.NextDouble() * 360.0f), variation));
            }

            Refresh(terrain);
        }

        public void Render(Camera camera, Light light, ShadowRenderer shadows, TerrainRenderer terrain, ClipPlane clip = ClipPlane.None)
        {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);
            GL.EnableVertexAttribArray(4);
            GL.EnableVertexAttribArray(5);
            GL.EnableVertexAttribArray(6);

            GL.UseProgram(Shader.Program);

            Shader.ApplyCamera(camera);
            Shader.ApplyLight(light);
            Shader.ApplyShadows(shadows);

            GL.Uniform1(Shader.ClipPlane, (int)clip);

            foreach(var key in models.Keys)
            {
                if (Map.MapData.Assets[key].Count == 0) continue;

                GL.Uniform1(Shader.RenderDistance, models[key].RenderDistance);
                Render(models[key].Model, key);
            }

            GL.DisableVertexAttribArray(4);
            GL.DisableVertexAttribArray(5);
            GL.DisableVertexAttribArray(6);
        }

        public void RenderShadowMap(ShadowRenderer shadows, TerrainRenderer terrain, ClipPlane clip = ClipPlane.None)
        {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(4);
            GL.EnableVertexAttribArray(5);
            GL.EnableVertexAttribArray(6);

            GL.UseProgram(ShadowShader.Program);

            GL.UniformMatrix4(ShadowShader.ViewMatrix, false, ref shadows.ViewMatrix);
            GL.UniformMatrix4(ShadowShader.ProjectionMatrix, false, ref shadows.ProjectionMatrix);

            GL.Uniform1(ShadowShader.ClipPlane, (int)clip);

            foreach(var key in models.Keys)
            {
                if (Map.MapData.Assets[key].Count == 0) continue;

                GL.Uniform1(ShadowShader.RenderDistance, models[key].RenderDistance);
                RenderShadowMap(models[key].Model, key);
            }

            GL.DisableVertexAttribArray(4);
            GL.DisableVertexAttribArray(5);
            GL.DisableVertexAttribArray(6);
        }
    }
}