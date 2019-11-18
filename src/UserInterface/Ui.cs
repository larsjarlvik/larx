using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Larx.MapAssets;
using Larx.Terrain;
using Larx.UserInterface.Button;
using Larx.UserInterface.Text;
using OpenTK;

namespace Larx.UserInterface
{
    public class Ui
    {
        private Matrix4 pMatrix;
        public readonly UiState State;
        private readonly ButtonRenderer buttonRenderer;
        private readonly TextRenderer textRenderer;
        private Vector2 buttonLeftOrigin;
        private Vector2 buttonRightOrigin;
        private const float buttonSize = 45.0f;
        private const float buttonSpacing = 5.0f;
        private const float windowPadding = 10.0f;
        private const float textSize = 13.0f;

        public Ui()
        {
            pMatrix = Matrix4.CreateOrthographicOffCenter(0, Larx.State.Window.Size.Width, Larx.State.Window.Size.Height, 0f, 0f, -1f);
            State = new UiState();
            buttonRenderer = new ButtonRenderer();
            textRenderer = new TextRenderer();

            State.ChildMenus = new Dictionary<string, Dictionary<string, int>>();
            State.TopMenu = new Dictionary<string, int>() {
                { TopMenuKeys.ElevationTools, addTexture("ui/terrain.png") },
                { TopMenuKeys.TerrainPaint, addTexture("ui/paint.png") },
                { TopMenuKeys.Assets, addTexture("ui/assets.png") },
            };

            State.ChildMenus.Add(TopMenuKeys.ElevationTools, new Dictionary<string, int>() {
                { TerrainConfig.ElevationTool, addTexture("ui/raise-lower.png") },
                { TerrainConfig.SmudgeTool, addTexture("ui/smudge.png") },
            });

            State.ChildMenus.Add(TopMenuKeys.TerrainPaint, TerrainConfig.Textures
                .Select(t => new KeyValuePair<string, int>(Array.IndexOf(TerrainConfig.Textures, t).ToString(), addTexture(Path.Combine($"textures/{t}-albedo.png"), true)))
                .ToDictionary(x => x.Key, x => x.Value)
            );

            State.ChildMenus.Add(TopMenuKeys.Assets, Assets.AssetKeys
                .Select(a => new KeyValuePair<string, int>(a , addTexture(Path.Combine($"ui/assets/{a}.png"))))
                .ToDictionary(x => x.Key, x => x.Value)
            );

            State.RightMenu = new Dictionary<string, int>() {
                { RightMenuKeys.SizeIncrease, addTexture("ui/terrain-increase.png") },
                { RightMenuKeys.SizeDecrease, addTexture("ui/terrain-decrease.png") },
                { RightMenuKeys.HardnessIncrease, addTexture("ui/hardness-increase.png") },
                { RightMenuKeys.HardnessDecrease, addTexture("ui/hardness-decrease.png") },
            };

            State.SetActiveTopMenuKey(TopMenuKeys.ElevationTools);
            State.Texts = new Dictionary<string, DisplayText>() {
                { TextKeys.Title, textRenderer.CreateText("Larx Terrain Editor v0.1", textSize) },
                { TextKeys.Radius, textRenderer.CreateText($"Radius: {Larx.State.ToolRadius}", textSize) },
                { TextKeys.Hardness, textRenderer.CreateText($"Hardness: {Larx.State.ToolHardness}", textSize) },
                { TextKeys.Position, textRenderer.CreateText("Position: 0 0", textSize) }
            };
        }

        private int addTexture(string path, bool mipMap = false)
        {
            var texture = new Texture();
            texture.LoadTexture(Path.Combine("resources", path), mipMap);
            return texture.TextureId;
        }

        public bool Update()
        {
            pMatrix = Matrix4.CreateOrthographicOffCenter(0, Larx.State.Window.Size.Width, Larx.State.Window.Size.Height, 0f, 0f, -1f);

            buttonLeftOrigin = new Vector2(windowPadding, Larx.State.Window.Size.Height - buttonSize - windowPadding);
            buttonRightOrigin = new Vector2(Larx.State.Window.Size.Width - buttonSize - windowPadding, buttonLeftOrigin.Y);

            var position = buttonLeftOrigin;
            var size = new Vector2(buttonSize);
            State.MouseRepeat = State.MousePressed;
            State.MousePressed = Larx.State.Mouse.LeftButton;
            State.ResetButtonStates();

            foreach(var button in State.TopMenu)
            {
                if (intersect(button.Key, Larx.State.Mouse.Position, position, size))
                    if (Larx.State.Mouse.LeftButton) State.SetActiveTopMenuKey(button.Key);

                position.X += buttonSize + buttonSpacing;
            }

            position.X += buttonSpacing * 2.0f;
            if (State.ActiveTopMenuKey != null)
                foreach(var button in State.ChildMenus[State.ActiveTopMenuKey])
                {
                    if (intersect(button.Key, Larx.State.Mouse.Position, position, size))
                        if (Larx.State.Mouse.LeftButton) State.ActiveChildMenuKey = button.Key;

                    position.X += buttonSize + buttonSpacing;
                }

            position.X = buttonRightOrigin.X;
            foreach(var button in State.RightMenu)
            {
                if(intersect(button.Key, Larx.State.Mouse.Position, position, size)) {
                    if (Larx.State.Mouse.LeftButton && !State.MouseRepeat) {
                        State.SetControls(button.Key);
                        UpdateText(TextKeys.Radius, $"Radius: {Larx.State.ToolRadius}");
                        UpdateText(TextKeys.Hardness, $"Hardness: {Larx.State.ToolHardness}");
                    }
                }

                position.X -= buttonSize + buttonSpacing;
            }

            return State.HoverKey != null;
        }

        public void UpdateText(string key, string value)
        {
            State.Texts[key] = textRenderer.CreateText(value, textSize);
        }

        private bool intersect(string key, Vector2 mouse, Vector2 position, Vector2 size)
        {
            if (mouse.X >= position.X && mouse.Y >= position.Y &&
                mouse.X <= position.X + size.X && mouse.Y <= position.Y + size.Y) {
                State.HoverKey = key;
                if (Larx.State.Mouse.LeftButton)
                    State.PressedKey = key;

                return true;
            }

            return false;
        }

        public void Render()
        {
            var position = new Vector2(windowPadding, windowPadding + textSize);
            foreach(var text in State.Texts) {
                textRenderer.Render(text.Value, pMatrix, position, 1.0f, 1.6f);
                position.Y += textSize * 1.5f;
            }

            position = buttonLeftOrigin;
            var size = new Vector2(buttonSize);

            foreach(var button in State.TopMenu)
                renderButton(button, ref position, size, buttonSize + buttonSpacing, State.ActiveTopMenuKey);

            if (State.ActiveTopMenuKey != null)
            {
                position.X += buttonSpacing * 2.0f;
                foreach(var button in State.ChildMenus[State.ActiveTopMenuKey])
                    renderButton(button, ref position, size, buttonSize + buttonSpacing, State.ActiveChildMenuKey);
            }

            position.X = buttonRightOrigin.X;
            foreach(var button in State.RightMenu)
                renderButton(button, ref position, size, -(buttonSize + buttonSpacing), State.ActiveChildMenuKey);
        }

        private void renderButton(KeyValuePair<string, int> button, ref Vector2 position, Vector2 size, float advance, string compareKey)
        {
            var buttonState = button.Key == State.PressedKey
                ? ButtonState.Pressed
                : button.Key == State.HoverKey ? ButtonState.Hover : ButtonState.Default;

            buttonRenderer.Render(pMatrix, position, size, button.Value, buttonState, button.Key == compareKey);
            position.X += advance;
        }
    }
}