using System;
using System.Collections.Generic;
using System.Linq;
using Larx.Button;
using Larx.UserInterFace;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.UserInterFace
{
    public class Ui : Builder
    {
        public List<ToolbarItem> Tools;
        private List<ToolbarItem> alignRight;

        public Ui() : base()
        {
            Tools = new List<ToolbarItem>();
            alignRight = new List<ToolbarItem>();
            build();
        }

        private void build()
        {
            AddText("title", "Larx Terrain Editor v0.1");
            AddText("size", $"Tool Size: 0");
            AddText("hardness", $"Hardness: 0");
            AddText("position", $"Position: 0 0");

            AddButton(Keys.ElevationTools, "ui/terrain.png");
            AddButton(Keys.TerrainPaint, "ui/paint.png");
            AddButton(Keys.AddAssets, "ui/assets.png");

            alignRight.Add(new ToolbarItem(TopMenu.Terrain, AddButton(Keys.Terrain.SizeIncrease, "ui/terrain-increase.png")));
            alignRight.Add(new ToolbarItem(TopMenu.Terrain, AddButton(Keys.Terrain.SizeDecrease, "ui/terrain-decrease.png")));
            alignRight.Add(new ToolbarItem(TopMenu.Terrain, AddButton(Keys.Terrain.HardnessIncrease, "ui/hardness-increase.png")));
            alignRight.Add(new ToolbarItem(TopMenu.Terrain, AddButton(Keys.Terrain.HardnessDecrease, "ui/hardness-decrease.png")));

            Tools.Add(new ToolbarItem(TopMenu.Paint, AddButton("0", "textures/grass-1-albedo.png")));
            Tools.Add(new ToolbarItem(TopMenu.Paint, AddButton("1", "textures/grass-2-albedo.png")));
            Tools.Add(new ToolbarItem(TopMenu.Paint, AddButton("2", "textures/grass-3-albedo.png")));
            Tools.Add(new ToolbarItem(TopMenu.Paint, AddButton("3", "textures/grass-4-albedo.png")));
            Tools.Add(new ToolbarItem(TopMenu.Paint, AddButton("4", "textures/bare-1-albedo.png")));
            Tools.Add(new ToolbarItem(TopMenu.Paint, AddButton("5", "textures/bare-2-albedo.png")));
            Tools.Add(new ToolbarItem(TopMenu.Paint, AddButton("6", "textures/bare-3-albedo.png")));
            Tools.Add(new ToolbarItem(TopMenu.Paint, AddButton("7", "textures/sand-1-albedo.png")));
            Tools.Add(new ToolbarItem(TopMenu.Paint, AddButton("8", "textures/sand-2-albedo.png")));
            Tools.Add(new ToolbarItem(TopMenu.Paint, AddButton("9", "textures/sand-3-albedo.png")));
            Tools.Add(new ToolbarItem(TopMenu.Paint, AddButton("10", "textures/sand-4-albedo.png")));
            Tools.Add(new ToolbarItem(TopMenu.Paint, AddButton("11", "textures/rock-1-albedo.png")));
            Tools.Add(new ToolbarItem(TopMenu.Paint, AddButton("12", "textures/rock-2-albedo.png")));

            State.ActiveTopMenu = TopMenu.Terrain;
            buttons[Keys.ElevationTools].Active = true;
        }

        public bool Update()
        {
            updateButtonPositions();

            UpdateText("size", $"Tool Size: {State.ToolRadius}");
            UpdateText("hardness", $"Hardness: {MathF.Round(State.ToolHardness, 1)}");

            var uiIntersect = UiIntersect(getVisibleTools(), State.Mouse.Position);
            if (uiIntersect != null) {
                buttons[uiIntersect].State = State.Mouse.LeftButton ? ButtonState.Pressed : ButtonState.Hover;
            }

            return uiIntersect != null;
        }

        public string Click()
        {
            if (!State.Mouse.LeftButton) return null;

            var uiIntersect = UiIntersect(getVisibleTools(), State.Mouse.Position);
            if (uiIntersect == null) return uiIntersect;

            switch(uiIntersect) {
                case Keys.ElevationTools:
                    State.ActiveTopMenu = TopMenu.Terrain;
                    buttons[Keys.ElevationTools].Active = true;
                    buttons[Keys.TerrainPaint].Active = false;
                    buttons[Keys.AddAssets].Active = false;
                    break;
                case Keys.TerrainPaint:
                    State.ActiveTopMenu = TopMenu.Paint;
                    buttons[Keys.ElevationTools].Active = false;
                    buttons[Keys.TerrainPaint].Active = true;
                    buttons[Keys.AddAssets].Active = false;
                    break;
                case Keys.AddAssets:
                    State.ActiveTopMenu = TopMenu.Assets;
                    buttons[Keys.ElevationTools].Active = false;
                    buttons[Keys.TerrainPaint].Active = false;
                    buttons[Keys.AddAssets].Active = true;
                    break;
                case Keys.Terrain.SizeIncrease:
                    State.ToolRadius ++;
                    if (State.ToolRadius > 12.0f) State.ToolRadius = 12.0f;
                    break;
                case Keys.Terrain.SizeDecrease:
                    State.ToolRadius --;
                    if (State.ToolRadius < 0.0f) State.ToolRadius = 0.0f;
                    break;
                case Keys.Terrain.HardnessIncrease:
                    State.ToolHardness += 0.1f;
                    if (State.ToolHardness > 1.0f) State.ToolHardness = 1.0f;
                    break;
                case Keys.Terrain.HardnessDecrease:
                    State.ToolHardness -= 0.1f;
                    if (State.ToolHardness < 0.0f) State.ToolHardness = 0.0f;
                    break;
                default:
                    if (Tools.Any(t => t.Key == uiIntersect)) {
                        State.ActiveToolBarItem = uiIntersect;
                        Tools.ForEach(t => buttons[t.Key].Active = false);
                        buttons[State.ActiveToolBarItem].Active = true;
                    }
                    break;
            }

            return uiIntersect;
        }

        private void updateButtonPositions()
        {
            var position = new Vector2(10.0f, State.Window.Size.Height - 70.0f);

            position = setButtonPosition(Keys.ElevationTools, position);
            position = setButtonPosition(Keys.TerrainPaint, position);
            position = setButtonPosition(Keys.AddAssets, position);
            position.X += 10.0f;

            foreach(var key in Tools.Where(t => t.TopMenu == State.ActiveTopMenu).Select(k => k.Key)) {
                position = setButtonPosition(key, position);
            }

            position = new Vector2(State.Window.Size.Width - 70.0f, State.Window.Size.Height - 70.0f);
            foreach(var key in alignRight.Select(k => k.Key)) {
                position = setButtonPosition(key, position, true);
            }
        }

        private Vector2 setButtonPosition(string key, Vector2 position, bool floatRight = false)
        {
            buttons[key].Position = position;
            position.X += (buttons[key].Size.X + (buttons[key].Size.X / 10)) * (floatRight ? -1 : 1);

            return position;
        }

        public void Render()
        {
            var pMatrix = Matrix4.CreateOrthographicOffCenter(0, State.Window.Size.Width, State.Window.Size.Height, 0f, 0f, -1.0f);

            for(var i = 0; i < texts.Count; i ++)
                texts.Values.ElementAt(i).Render(pMatrix, new Vector2(10, 20 + i * 20), 0.65f, 1.6f);

            foreach(var key in getVisibleTools())
                buttons[key].Render(pMatrix);
        }

        private List<string> getVisibleTools()
        {
            var visble = new List<string> { Keys.ElevationTools, Keys.TerrainPaint, Keys.AddAssets };
            visble.AddRange(Tools.Where(t => t.TopMenu == State.ActiveTopMenu).Select(t => t.Key));

            if (State.ActiveTopMenu != TopMenu.Assets)
                visble.AddRange(alignRight.Select(t => t.Key));

            return visble;
        }
    }
}