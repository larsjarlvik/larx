using System.Collections.Generic;
using Larx.GltfModel;
using Larx.Terrain;
using Larx.UserInterFace;
using OpenTK;

namespace Larx.MapAssets
{
    public class Assets
    {
        private struct PlacedAsset
        {
            public Vector3 Position;
            public Model Model;

            public PlacedAsset(Model model, Vector3 position)
            {
                Model = model;
                Position = position;
            }
        }

        private readonly string[] assets = new string[] {
            "tree",
        };
        private readonly AssetRenderer assetRenderer;
        private readonly Dictionary<string, Model> models;
        private readonly List<PlacedAsset> placedAssets;

        public Assets(Ui ui)
        {
            assetRenderer = new AssetRenderer();
            models = new Dictionary<string, Model>();
            placedAssets = new List<PlacedAsset>();

            foreach(var asset in assets) {
                ui.Tools.Add(new ToolbarItem(TopMenu.Assets, ui.AddButton($"asset_{asset}", $"ui/assets/{asset}.png")));
                models.Add($"asset_{asset}", Model.Load(asset));
            }
        }

        public void Add(Vector3 position, TerrainRenderer terrain)
        {
            if (State.ActiveToolBarItem == null) return;

            var elev = terrain.GetElevationAtPoint(position);
            if (elev == null) return;

            placedAssets.Add(new PlacedAsset(models[State.ActiveToolBarItem], new Vector3(position.X, (float)elev, position.Z)));
        }

        public void Render(Camera camera, Light light, TerrainRenderer terrain)
        {
            foreach(var asset in placedAssets)
            {
                assetRenderer.Render(camera, light, asset.Model, new Vector3(asset.Position.X, (float)terrain.GetElevationAtPoint(asset.Position), asset.Position.Z));
            }
        }
    }
}