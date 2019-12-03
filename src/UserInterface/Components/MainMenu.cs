using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Larx.Assets;
using Larx.Terrain;
using Larx.UserInterface.Widgets;

namespace Larx.UserInterface.Components
{
    public class MainMenu
    {
        private readonly Container root;
        private readonly List<Container> children;
        private string activeTopMenuKey;
        public Container Component { get; }

        public MainMenu()
        {
            root = new Container("TopMenu", Direction.Horizonal,
                new List<IWidget>() {
                    new IconButton(UiKeys.TopMenu.ElevationTools, "ui/terrain.png", true),
                    new IconButton(UiKeys.TopMenu.TerrainPaint, "ui/paint.png", true),
                    new IconButton(UiKeys.TopMenu.Assets, "ui/assets.png", true),
                }
            );

            children = new List<Container>();
            children.Add(new Container(UiKeys.TopMenu.ElevationTools, Direction.Horizonal,
                new List<IWidget> {
                    new IconButton(UiKeys.Terrain.ElevationTool, "ui/raise-lower.png", true),
                    new IconButton(UiKeys.Terrain.SmudgeTool, "ui/smudge.png", true),
                    new IconButton(UiKeys.Terrain.LevelRaise, "ui/level-raise.png"),
                    new IconButton(UiKeys.Terrain.LevelLower, "ui/level-lower.png"),
                    new IconButton(UiKeys.Terrain.StrengthIncrease, "ui/strength-increase.png"),
                    new IconButton(UiKeys.Terrain.StrengthDecrease, "ui/strength-decrease.png"),
                }
            ));

            var textures = TerrainConfig.Textures
                .Select(t => new IconButton(Array.IndexOf(TerrainConfig.Textures, t).ToString(), Path.Combine($"textures/{t}-albedo.png"), true, true))
                .ToList<IWidget>();
            textures.Add(new IconButton(UiKeys.SplatMap.AutoPaint, "ui/auto.png", true));
            textures.Add(new IconButton(UiKeys.SplatMap.AutoPaintGlobal, "ui/auto-global.png"));
            children.Add(new Container(UiKeys.TopMenu.TerrainPaint, Direction.Horizonal, textures));

            var assets = AssetRenderer.AssetKeys
                .Select(a => new IconButton(a, Path.Combine($"ui/assets/{a}.png"), true, true))
                .ToList<IWidget>();
            assets.Add(new IconButton(UiKeys.Assets.Erase, "ui/erase.png", true));
            children.Add(new Container(UiKeys.TopMenu.Assets, Direction.Horizonal, assets));

            Component = new Container("Left", Direction.Horizonal, new List<IWidget>() { root, children.First() }, 0.0f);
            setActiveTopMenuKey(UiKeys.TopMenu.ElevationTools);
            setActiveButtonStates();
        }

        public void Update()
        {
            if (children.Any(x => x.Key == Ui.State.Hover.Key))
                setActiveTopMenuKey(Ui.State.Hover.Key);

            var child = children.First(x => x.Key == activeTopMenuKey).Children.Any(x => x.Key == Ui.State.Hover.Key);
            if (child) setState(Ui.State.Hover as IconButton);
            setActiveButtonStates();
        }

        private void setActiveTopMenuKey(string key)
        {
            activeTopMenuKey = key;
            Component.Children[1] = children.First(x => x.Key == key);
            setState(children.First(x => x.Key == key).Children.First() as IconButton);

            switch(key) {
                case UiKeys.TopMenu.ElevationTools:
                case UiKeys.TopMenu.TerrainPaint:
                    State.ToolRadius = 5f;
                    State.ToolHardness = 5f;
                    break;
                case UiKeys.TopMenu.Assets:
                    State.ToolRadius = 1f;
                    State.ToolHardness = 1f;
                    break;
            }
        }

        private void setState(IconButton child)
        {
            switch(activeTopMenuKey) {
                case UiKeys.TopMenu.ElevationTools:
                    State.SelectedTool = child.Key;
                    break;
                case UiKeys.TopMenu.TerrainPaint:
                    State.SelectedTool = child.Key == UiKeys.SplatMap.AutoPaint || child.Key == UiKeys.SplatMap.AutoPaintGlobal ? child.Key : UiKeys.SplatMap.Paint;
                    State.SelectedToolData = child.Key;
                    break;
                case UiKeys.TopMenu.Assets:
                    State.SelectedTool = child.Key == UiKeys.Assets.Erase ? child.Key : UiKeys.Assets.Asset;
                    State.SelectedToolData = child.Key;
                    break;
            }
        }

        private void setActiveButtonStates()
        {
            foreach(var button in root.Children)
                ((IconButton)button).Active = activeTopMenuKey == button.Key;

            foreach(var button in children.SelectMany(x => x.Children))
                ((IconButton)button).Active = (State.SelectedTool == button.Key || State.SelectedToolData == button.Key);
        }
    }
}