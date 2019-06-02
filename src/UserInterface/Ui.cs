using System;
using System.Collections.Generic;
using System.Linq;
using Larx.Button;
using Larx.UserInterFace;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Larx.UserInterFace
{
    public class Ui : Builder
    {
        private List<ToolbarItem> tools;

        public Ui() : base()
        {
            tools = new List<ToolbarItem>();
            build();
        }

        private void build()
        {
            AddText("title", "Larx Terrain Editor v0.1");
            AddText("size", $"Tool Size: 0");
            AddText("hardness", $"Hardness: 0");
            AddText("position", $"Position: 0 0");

            AddButton(Keys.ElevationTools, "ui/terrain.bmp");
            AddButton(Keys.TerrainPaint, "ui/paint.bmp");

            tools.Add(new ToolbarItem(TopMenu.Terrain, AddButton(Keys.Terrain.SizeIncrease, "ui/terrain-increase.bmp")));
            tools.Add(new ToolbarItem(TopMenu.Terrain, AddButton(Keys.Terrain.SizeDecrease, "ui/terrain-decrease.bmp")));
            tools.Add(new ToolbarItem(TopMenu.Terrain, AddButton(Keys.Terrain.HardnessIncrease, "ui/hardness-increase.bmp")));
            tools.Add(new ToolbarItem(TopMenu.Terrain, AddButton(Keys.Terrain.HardnessDecrease, "ui/hardness-decrease.bmp")));

            tools.Add(new ToolbarItem(TopMenu.Paint, AddButton(Keys.Paint.Grass, "textures/grass-albedo.bmp")));
            tools.Add(new ToolbarItem(TopMenu.Paint, AddButton(Keys.Paint.RoughGrass, "textures/rocky-grass-albedo.bmp")));
            tools.Add(new ToolbarItem(TopMenu.Paint, AddButton(Keys.Paint.Cliff, "textures/cliff-albedo.bmp")));
            tools.Add(new ToolbarItem(TopMenu.Paint, AddButton(Keys.Paint.Sand, "textures/sand-albedo.bmp")));


            State.ActiveTopMenu = TopMenu.Terrain;
            buttons[Keys.ElevationTools].Active = true;
        }

        public bool Update(Point mousePos, ButtonState leftButton)
        {
            updateButtonPositions();

            UpdateText("size", $"Tool Size: {State.ToolRadius}");
            UpdateText("hardness", $"Hardness: {MathF.Round(State.ToolHardness, 1)}");

            return MouseUiIntersect(getVisibleTools(), mousePos, leftButton) != null;
        }

        public string Click(Point mousePos, ButtonState leftButton)
        {
            if (leftButton != ButtonState.Pressed) return null;

            var uiIntersect = MouseUiIntersect(getVisibleTools(), mousePos, leftButton);
            switch(uiIntersect) {
                case Keys.ElevationTools:
                    State.ActiveTopMenu = TopMenu.Terrain;
                    break;
                case Keys.TerrainPaint:
                    State.ActiveTopMenu = TopMenu.Paint;
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
                    if(byte.TryParse(uiIntersect, out byte texture)) {
                        State.ActiveTexture = texture;
                        tools.Where(t => t.TopMenu == TopMenu.Paint).ToList().ForEach(t => buttons[t.Key].Active = false);
                        buttons[uiIntersect].Active = true;
                    }
                    break;
            }

            return uiIntersect;
        }

        private void updateButtonPositions()
        {
            var position = new Vector2(10.0f, uiSize.Height - 70.0f);

            position = setButtonPosition(Keys.ElevationTools, position);
            position = setButtonPosition(Keys.TerrainPaint, position);
            position.X += 10.0f;

            foreach(var key in tools.Where(t => t.TopMenu == State.ActiveTopMenu).Select(k => k.Key)) {
                position = setButtonPosition(key, position);
            }
        }

        private Vector2 setButtonPosition(string key, Vector2 position)
        {
            buttons[key].Position = position;
            position.X += buttons[key].Size.X + (buttons[key].Size.X / 10);

            return position;
        }

        public void Render()
        {
            GL.Enable(EnableCap.Blend);
            var pMatrix = Matrix4.CreateOrthographicOffCenter(0, uiSize.Width, uiSize.Height, 0f, 0f, -1.0f);

            for(var i = 0; i < texts.Count; i ++)
                texts.Values.ElementAt(i).Render(pMatrix, new Vector2(10, 20 + i * 20), 0.65f, 1.6f);

            foreach(var key in getVisibleTools())
                buttons[key].Render(pMatrix);

            GL.Disable(EnableCap.Blend);
        }

        private List<string> getVisibleTools()
        {
            var visble = new List<string> { Keys.ElevationTools, Keys.TerrainPaint };
            visble.AddRange(tools.Where(t => t.TopMenu == State.ActiveTopMenu).Select(t => t.Key));
            return visble;
        }
    }
}