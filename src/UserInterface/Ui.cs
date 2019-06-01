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

        private List<string> elevationTools;
        private List<string> terrainPaint;
        private List<string> activeTopMenu;


        public Ui() : base()
        {
            elevationTools = new List<string>();
            terrainPaint = new List<string>();
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

            elevationTools.Add(AddButton(Keys.Terrain.Increase, "ui/terrain-increase.bmp"));
            elevationTools.Add(AddButton(Keys.Terrain.Decrease, "ui/terrain-decrease.bmp"));

            terrainPaint.Add(AddButton(Keys.Paint.Grass, "textures/grass-albedo.bmp"));
            terrainPaint.Add(AddButton(Keys.Paint.RoughGrass, "textures/rocky-grass-albedo.bmp"));
            terrainPaint.Add(AddButton(Keys.Paint.Cliff, "textures/cliff-albedo.bmp"));

            activeTopMenu = elevationTools;
        }

        public bool Update(Point mousePos, ButtonState leftButton)
        {
            updateButtonPositions();

            var toCheck = new List<string> { Keys.ElevationTools, Keys.TerrainPaint };
            toCheck.AddRange(activeTopMenu);

            return MouseUiIntersect(toCheck, mousePos, leftButton) != null;
        }

        public string Click(Point mousePos, ButtonState leftButton)
        {
            if (leftButton != ButtonState.Pressed) return null;

            var toCheck = new List<string> { Keys.ElevationTools, Keys.TerrainPaint };
            toCheck.AddRange(activeTopMenu);

            var uiIntersect = MouseUiIntersect(toCheck, mousePos, leftButton);
            switch(uiIntersect) {
                case Keys.ElevationTools:
                    activeTopMenu = elevationTools;
                    break;
                case Keys.TerrainPaint:
                    activeTopMenu = terrainPaint;
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

            foreach(var key in activeTopMenu) {
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

            // Category buttons
            buttons[Keys.ElevationTools].Render(pMatrix);
            buttons[Keys.TerrainPaint].Render(pMatrix);

            // Child menu
            foreach(var key in activeTopMenu) {
                buttons[key].Render(pMatrix);
            }

            GL.Disable(EnableCap.Blend);
        }
    }
}