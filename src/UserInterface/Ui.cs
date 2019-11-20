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
        private Vector2 bottomLeftOrigin;
        private Vector2 bottomRightOrigin;
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

            State.ChildMenus = new Dictionary<string, Dictionary<string, UiElement>>();
            State.TopMenu = new Dictionary<string, UiElement>() {
                { TopMenuKeys.ElevationTools, new UiElement("ui/terrain.png", true) },
                { TopMenuKeys.TerrainPaint, new UiElement("ui/paint.png", true) },
                { TopMenuKeys.Assets, new UiElement("ui/assets.png", true) },
            };

            State.ChildMenus.Add(TopMenuKeys.ElevationTools, new Dictionary<string, UiElement>() {
                { TerrainConfig.ElevationTool, new UiElement("ui/raise-lower.png", true) },
                { TerrainConfig.SmudgeTool, new UiElement("ui/smudge.png", true) },
                { TerrainConfig.LevelRaise, new UiElement("ui/level-raise.png") },
                { TerrainConfig.LevelLower, new UiElement("ui/level-lower.png") },
                { TerrainConfig.StrengthIncrease, new UiElement("ui/strength-increase.png") },
                { TerrainConfig.StrengthDecrease, new UiElement("ui/strength-decrease.png") },
            });

            State.ChildMenus.Add(TopMenuKeys.TerrainPaint, TerrainConfig.Textures
                .Select(t => new KeyValuePair<string, UiElement>(Array.IndexOf(TerrainConfig.Textures, t).ToString(), new UiElement(Path.Combine($"textures/{t}-albedo.png"), true, true)))
                .ToDictionary(x => x.Key, x => x.Value)
            );

            State.ChildMenus.Add(TopMenuKeys.Assets, Assets.AssetKeys
                .Select(a => new KeyValuePair<string, UiElement>(a , new UiElement(Path.Combine($"ui/assets/{a}.png"), true, true)))
                .ToDictionary(x => x.Key, x => x.Value)
            );
            State.ChildMenus[TopMenuKeys.Assets].Add(ActionKeys.Erase, new UiElement("ui/erase.png"));

            State.RightMenu = new Dictionary<string, UiElement>() {
                { ActionKeys.SizeIncrease, new UiElement("ui/terrain-increase.png") },
                { ActionKeys.SizeDecrease, new UiElement("ui/terrain-decrease.png") },
                { ActionKeys.HardnessIncrease, new UiElement("ui/hardness-increase.png") },
                { ActionKeys.HardnessDecrease, new UiElement("ui/hardness-decrease.png") },
            };

            State.SetActiveTopMenuKey(TopMenuKeys.ElevationTools);
            State.Texts = new Dictionary<string, DisplayText>() {
                { TextKeys.Title, textRenderer.CreateText("Larx Terrain Editor v0.1", textSize) },
                { TextKeys.Radius, textRenderer.CreateText($"Radius: {Larx.State.ToolRadius}", textSize) },
                { TextKeys.Hardness, textRenderer.CreateText($"Hardness: {Larx.State.ToolHardness}", textSize) },
                { TextKeys.Position, textRenderer.CreateText("Position: 0 0", textSize) }
            };
        }

        public bool Update()
        {
            pMatrix = Matrix4.CreateOrthographicOffCenter(0, Larx.State.Window.Size.Width, Larx.State.Window.Size.Height, 0f, 0f, -1f);

            bottomLeftOrigin = new Vector2(windowPadding, Larx.State.Window.Size.Height - buttonSize - windowPadding);
            bottomRightOrigin = new Vector2(Larx.State.Window.Size.Width - buttonSize - windowPadding, bottomLeftOrigin.Y);

            var position = bottomLeftOrigin;
            var size = new Vector2(buttonSize);
            State.PressedKey = null;
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
                        if (Larx.State.Mouse.LeftButton) {
                            if(button.Value.IsToggle) {
                                State.ActiveChildMenuKey = button.Key;
                            }
                        }

                    position.X += buttonSize + buttonSpacing;
                }

            position.X = bottomRightOrigin.X;
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

            position = bottomLeftOrigin;
            var size = new Vector2(buttonSize);

            foreach(var uiElement in State.TopMenu)
                renderButton(uiElement, ref position, size, buttonSize + buttonSpacing, State.ActiveTopMenuKey);

            if (State.ActiveTopMenuKey != null)
            {
                position.X += buttonSpacing * 2.0f;
                foreach(var uiElement in State.ChildMenus[State.ActiveTopMenuKey])
                    renderButton(uiElement, ref position, size, buttonSize + buttonSpacing, State.ActiveChildMenuKey);
            }

            position.X = bottomRightOrigin.X;
            foreach(var uiElement in State.RightMenu)
                renderButton(uiElement, ref position, size, -(buttonSize + buttonSpacing), State.ActiveChildMenuKey);
        }

        private void renderButton(KeyValuePair<string, UiElement> uiElement, ref Vector2 position, Vector2 size, float advance, string compareKey)
        {
            var buttonState = uiElement.Key == State.PressedKey
                ? ButtonState.Pressed
                : uiElement.Key == State.HoverKey ? ButtonState.Hover : ButtonState.Default;

            buttonRenderer.Render(pMatrix, position, size, uiElement.Value.Texture, buttonState, uiElement.Key == compareKey);
            position.X += advance;
        }
    }
}