using System;
using System.Collections.Generic;
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
        private readonly string[] assets = new string[] {
            "tree",
            "rock",
            "grass"
        };
        private readonly Dictionary<string, Model> models;
        private readonly Random random;

        public Assets(Ui ui)
        {
            models = new Dictionary<string, Model>();
            random = new Random();

            foreach(var asset in assets) {
                var model = Model.Load(asset);
                AppendBuffers(model);

                ui.Tools.Add(new ToolbarItem(TopMenu.Assets, ui.AddButton(asset, $"ui/assets/{asset}.png")));
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

                Map.MapData.Assets[State.ActiveToolBarItem].Add(new PlacedAsset(new Vector2(x, y), MathLarx.DegToRad((float)random.NextDouble() * 360.0f)));
            }

            Refresh(models[State.ActiveToolBarItem], terrain);
        }

        public void Render(Camera camera, Light light, ShadowRenderer shadows, TerrainRenderer terrain, ClipPlane clip = ClipPlane.None)
        {
            GL.UseProgram(Shader.Program);

            Shader.ApplyCamera(camera);
            Shader.ApplyLight(light);
            Shader.ApplyShadows(shadows);

            GL.Uniform1(Shader.ClipPlane, (int)clip);

            foreach(var key in models.Keys)
            {
                if (Map.MapData.Assets[key].Count == 0) continue;
                Render(models[key], key);
            }

            GL.DisableVertexAttribArray(4);
            GL.DisableVertexAttribArray(5);
        }

        public void RenderShadowMap(ShadowRenderer shadows, TerrainRenderer terrain, ClipPlane clip = ClipPlane.None)
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
        }
    }
}