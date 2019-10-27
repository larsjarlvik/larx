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
        };
        private readonly Dictionary<string, Model> models;
        private readonly Random random;


        public Assets(Ui ui)
        {
            models = new Dictionary<string, Model>();
            random = new Random();

            foreach(var asset in assets) {
                ui.Tools.Add(new ToolbarItem(TopMenu.Assets, ui.AddButton($"asset_{asset}", $"ui/assets/{asset}.png")));
                models.Add($"asset_{asset}", Model.Load(asset));
            }
        }

        public void Add(Vector2 position, TerrainRenderer terrain)
        {
            if (State.ActiveToolBarItem == null) return;

            var elev = terrain.GetElevationAtPoint(position);
            if (elev == null) return;

            Map.MapData.Assets.Add(new PlacedAsset(State.ActiveToolBarItem, new Vector2(position.X, position.Y), MathLarx.DegToRad((float)random.NextDouble() * 360.0f)));
        }

        public void Render(Camera camera, Light light, ShadowRenderer shadows, TerrainRenderer terrain)
        {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);

            GL.UseProgram(shader.Program);

            shader.ApplyCamera(camera);
            shader.ApplyLight(light);
            shader.ApplyShadows(shadows);

            foreach(var asset in Map.MapData.Assets)
            {
                Render(camera, light, shadows, models[asset.Model], new Vector3(asset.Position.X, (float)terrain.GetElevationAtPoint(asset.Position), asset.Position.Y), asset.Rotation);
            }
        }

        public void RenderShadowMap(Matrix4 projectionMatrix, Matrix4 viewMatrix, TerrainRenderer terrain)
        {
            GL.EnableVertexAttribArray(0);
            GL.UseProgram(shadowShader.Program);

            GL.UniformMatrix4(shadowShader.ViewMatrix, false, ref viewMatrix);
            GL.UniformMatrix4(shadowShader.ProjectionMatrix, false, ref projectionMatrix);

            foreach(var asset in Map.MapData.Assets)
            {
                RenderShadowMap(projectionMatrix, viewMatrix, models[asset.Model], new Vector3(asset.Position.X, (float)terrain.GetElevationAtPoint(asset.Position), asset.Position.Y), asset.Rotation);
            }
        }
    }
}