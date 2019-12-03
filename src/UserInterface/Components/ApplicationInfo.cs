using System.Collections.Generic;
using Larx.UserInterface.Widgets;

namespace Larx.UserInterface.Components
{
    public class ApplicationInfo
    {
        public Container Component { get; }


        public ApplicationInfo()
        {
            Component = new Container("Texts", Direction.Vertical, new List<IWidget>() {
                new Label(UiKeys.Texts.Title, "Larx Terrain Editor v0.1"),
                new Label(UiKeys.Texts.Radius, $"Radius: {Larx.State.ToolRadius}"),
                new Label(UiKeys.Texts.Hardness, $"Hardness: {Larx.State.ToolHardness}"),
                new Label(UiKeys.Texts.Position, "Position: 0 0")
            });
        }

        public void Update()
        {
            ((Label)Component.Children[1]).UpdateText($"Radius: {Larx.State.ToolRadius}");
            ((Label)Component.Children[2]).UpdateText($"Hardness: {Larx.State.ToolHardness}");
            ((Label)Component.Children[3]).UpdateText($"Position: {State.TerrainMousePosition.X:0.00} {State.TerrainMousePosition.Z:0.00}");
        }
    }
}