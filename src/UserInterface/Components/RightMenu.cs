using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Larx.Assets;
using Larx.Terrain;
using Larx.UserInterface.Widgets;

namespace Larx.UserInterface.Components
{
    public class RightMenu
    {
        public Container Component { get; }


        public RightMenu()
        {
            Component = new Container("RightMenu", Direction.Horizonal,
                new List<IWidget> {
                    new IconButton(UiKeys.Actions.HardnessDecrease, "ui/hardness-decrease.png"),
                    new IconButton(UiKeys.Actions.HardnessIncrease, "ui/hardness-increase.png"),
                    new IconButton(UiKeys.Actions.SizeDecrease, "ui/terrain-decrease.png"),
                    new IconButton(UiKeys.Actions.SizeIncrease, "ui/terrain-increase.png"),
                }
            );
        }

        public void Update()
        {
            if (Component.Children.Any(x => x.Key == Ui.State.HoverKey)) {
                setControls(Ui.State.HoverKey);
                // UpdateText(UiKeys.Texts.Radius, $"Radius: {Larx.State.ToolRadius}");
                // UpdateText(UiKeys.Texts.Hardness, $"Hardness: {Larx.State.ToolHardness}");
            }
        }

        private void setControls(string key)
        {
            switch(key) {
                case UiKeys.Actions.SizeIncrease:
                    State.ToolRadius += State.ToolRadius >= 20 ? 5 : State.ToolRadius >= 10 ? 2 : 1;
                    if (State.ToolRadius > 100f) State.ToolRadius = 100f;
                    break;
                case UiKeys.Actions.SizeDecrease:
                    State.ToolRadius += State.ToolRadius >= 20 ? -5 : State.ToolRadius >= 10 ? -2 : -1;
                    if (State.ToolRadius < 0f) State.ToolRadius = 0f;
                    break;
                case UiKeys.Actions.HardnessIncrease:
                    State.ToolHardness += 1f;
                    if (State.ToolHardness > 10f) State.ToolHardness = 10f;
                    break;
                case UiKeys.Actions.HardnessDecrease:
                    State.ToolHardness -= 1f;
                    if (State.ToolHardness < 0f) State.ToolHardness = 0f;
                    break;
            }
        }
    }
}