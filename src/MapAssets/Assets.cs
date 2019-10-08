using System;
using System.Collections.Generic;
using Larx.GltfModel;
using Larx.Storage;
using Larx.Terrain;
using Larx.UserInterFace;
using Larx.Utils;
using OpenTK;

namespace Larx.MapAssets
{
    public class Assets
    {
        private readonly string[] assets = new string[] {
            "tree",
        };
        private readonly AssetRenderer assetRenderer;
        private readonly Dictionary<string, Model> models;
        private readonly Random random;


        public Assets(Ui ui)
        {
            assetRenderer = new AssetRenderer();
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

        public void Render(Camera camera, Light light, TerrainRenderer terrain)
        {
            foreach(var asset in Map.MapData.Assets)
            {
                assetRenderer.Render(camera, light, models[asset.Model], new Vector3(asset.Position.X, (float)terrain.GetElevationAtPoint(asset.Position), asset.Position.Y), asset.Rotation);
            }
        }

        public void RenderShadowMap(Matrix4 viewMatrix, Matrix4 projectionMatrix, TerrainRenderer terrain)
        {
            foreach(var asset in Map.MapData.Assets)
            {
                assetRenderer.RenderShadowMap(viewMatrix, projectionMatrix, models[asset.Model], new Vector3(asset.Position.X, (float)terrain.GetElevationAtPoint(asset.Position), asset.Position.Y), asset.Rotation);
            }
        }
    }
}