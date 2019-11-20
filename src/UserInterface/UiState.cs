using System.Collections.Generic;
using System.Linq;
using Larx.UserInterface.Text;

namespace Larx.UserInterface
{

    public class UiState
    {
        public Dictionary<string, UiElement> TopMenu { get; set; }
        public Dictionary<string, UiElement> RightMenu { get; set; }
        public Dictionary<string, Dictionary<string, UiElement>> ChildMenus { get; set; }
        public Dictionary<string, DisplayText> Texts { get; set; }

        public string HoverKey { get; set; }
        public string PressedKey { get; set; }
        public string ActiveTopMenuKey { get; set; }
        public string ActiveChildMenuKey { get; set; }
        public bool MousePressed { get; set; }
        public bool MouseRepeat { get; set; }

        public void SetActiveTopMenuKey(string key)
        {
            ActiveTopMenuKey = key;
            ActiveChildMenuKey = ChildMenus[ActiveTopMenuKey].Keys.First();
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

        public void SetControls(string key)
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