using System.Collections.Generic;
using System.Linq;
using Larx.UserInterface.Text;

namespace Larx.UserInterface
{
    public static class TopMenuKeys
    {
        public const string ElevationTools = "elevation";
        public const string TerrainPaint = "paint";
        public const string Assets = "assets";
    }

    public static class ActionKeys
    {
        public const string SizeIncrease = "size_increase";
        public const string SizeDecrease = "size_decrease";
        public const string HardnessIncrease = "hardness_increase";
        public const string HardnessDecrease = "hardness_decrease";
        public const string Erase = "erase";
    }

    public static class TextKeys
    {
        public const string Title = "title";
        public const string Position = "position";
        public const string Radius = "radius";
        public const string Hardness = "hardness";
    }

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
        public string ButtonPressed { get; set; }
        public bool MousePressed { get; set; }
        public bool MouseRepeat { get; set; }

        public void SetActiveTopMenuKey(string key)
        {
            ActiveTopMenuKey = key;
            ActiveChildMenuKey = ChildMenus[ActiveTopMenuKey].Keys.First();
            switch(key) {
                case TopMenuKeys.ElevationTools:
                case TopMenuKeys.TerrainPaint:
                    State.ToolRadius = 5f;
                    State.ToolHardness = 5f;
                    break;
                case TopMenuKeys.Assets:
                    State.ToolRadius = 1f;
                    State.ToolHardness = 1f;
                    break;
            }
        }

        public void SetControls(string key)
        {
            switch(key) {
                case ActionKeys.SizeIncrease:
                    State.ToolRadius += State.ToolRadius >= 20 ? 5 : State.ToolRadius >= 10 ? 2 : 1;
                    if (State.ToolRadius > 100f) State.ToolRadius = 100f;
                    break;
                case ActionKeys.SizeDecrease:
                    State.ToolRadius += State.ToolRadius >= 20 ? -5 : State.ToolRadius >= 10 ? -2 : -1;
                    if (State.ToolRadius < 0f) State.ToolRadius = 0f;
                    break;
                case ActionKeys.HardnessIncrease:
                    State.ToolHardness += 1f;
                    if (State.ToolHardness > 10f) State.ToolHardness = 10f;
                    break;
                case ActionKeys.HardnessDecrease:
                    State.ToolHardness -= 1f;
                    if (State.ToolHardness < 0f) State.ToolHardness = 0f;
                    break;
            }
        }

        public void ResetButtonStates()
        {
            HoverKey = null;
            PressedKey = null;
        }
    }
}