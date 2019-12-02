using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Larx.Assets;
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
                { UiKeys.TopMenu.ElevationTools, new UiElement("ui/terrain.png", true) },
                { UiKeys.TopMenu.TerrainPaint, new UiElement("ui/paint.png", true) },
                { UiKeys.TopMenu.Assets, new UiElement("ui/assets.png", true) },
            };

            State.ChildMenus.Add(UiKeys.TopMenu.ElevationTools, new Dictionary<string, UiElement>() {
                { UiKeys.Terrain.ElevationTool, new UiElement("ui/raise-lower.png", true) },
                { UiKeys.Terrain.SmudgeTool, new UiElement("ui/smudge.png", true) },
                { UiKeys.Terrain.LevelRaise, new UiElement("ui/level-raise.png") },
                { UiKeys.Terrain.LevelLower, new UiElement("ui/level-lower.png") },
                { UiKeys.Terrain.StrengthIncrease, new UiElement("ui/strength-increase.png") },
                { UiKeys.Terrain.StrengthDecrease, new UiElement("ui/strength-decrease.png") },
            });

            State.ChildMenus.Add(UiKeys.TopMenu.TerrainPaint, TerrainConfig.Textures
                .Select(t => new KeyValuePair<string, UiElement>(Array.IndexOf(TerrainConfig.Textures, t).ToString(), new UiElement(Path.Combine($"textures/{t}-albedo.png"), true, true)))
                .ToDictionary(x => x.Key, x => x.Value)
            );
            State.ChildMenus[UiKeys.TopMenu.TerrainPaint].Add(UiKeys.SplatMap.AutoPaint, new UiElement("ui/auto.png", true));
            State.ChildMenus[UiKeys.TopMenu.TerrainPaint].Add(UiKeys.SplatMap.AutoPaintGlobal, new UiElement("ui/auto-global.png"));

            State.ChildMenus.Add(UiKeys.TopMenu.Assets, AssetRenderer.AssetKeys
                .Select(a => new KeyValuePair<string, UiElement>(a , new UiElement(Path.Combine($"ui/assets/{a}.png"), true, true)))
                .ToDictionary(x => x.Key, x => x.Value)
            );
            State.ChildMenus[UiKeys.TopMenu.Assets].Add(UiKeys.Assets.Erase, new UiElement("ui/erase.png", true));

            State.RightMenu = new Dictionary<string, UiElement>() {
                { UiKeys.Actions.SizeIncrease, new UiElement("ui/terrain-increase.png") },
                { UiKeys.Actions.SizeDecrease, new UiElement("ui/terrain-decrease.png") },
                { UiKeys.Actions.HardnessIncrease, new UiElement("ui/hardness-increase.png") },
                { UiKeys.Actions.HardnessDecrease, new UiElement("ui/hardness-decrease.png") },
            };

            State.SetActiveTopMenuKey(UiKeys.TopMenu.ElevationTools);
            State.Texts = new Dictionary<string, DisplayText>() {
                { UiKeys.Texts.Title, textRenderer.CreateText("Larx Terrain Editor v0.1", textSize) },
                { UiKeys.Texts.Radius, textRenderer.CreateText($"Radius: {Larx.State.ToolRadius}", textSize) },
                { UiKeys.Texts.Hardness, textRenderer.CreateText($"Hardness: {Larx.State.ToolHardness}", textSize) },
                { UiKeys.Texts.Position, textRenderer.CreateText("Position: 0 0", textSize) }
            };
        }

        public bool Update()
        {
            pMatrix = Matrix4.CreateOrthographicOffCenter(0, Larx.State.Window.Size.Width, Larx.State.Window.Size.Height, 0f, 0f, -1f);

            bottomLeftOrigin = new Vector2(windowPadding, Larx.State.Window.Size.Height - buttonSize - windowPadding);
            bottomRightOrigin = new Vector2(Larx.State.Window.Size.Width - buttonSize - windowPadding, bottomLeftOrigin.Y);

            var position = bottomLeftOrigin;
            var size = new Vector2(buttonSize);
            State.MouseRepeat = State.MousePressed;
            State.MousePressed = Larx.State.Mouse.LeftButton;
            State.HoverKey = null;

            foreach(var button in State.TopMenu) {
                if (intersect(button.Key, Larx.State.Mouse.Position, position, size) && Larx.State.Mouse.LeftButton && !State.MouseRepeat)
                    State.SetActiveTopMenuKey(button.Key);

                position.X += buttonSize + buttonSpacing;
            }

            position.X += buttonSpacing * 2.0f;
            if (State.ActiveTopMenuKey != null)
                foreach(var button in State.ChildMenus[State.ActiveTopMenuKey]) {
                    if (intersect(button.Key, Larx.State.Mouse.Position, position, size) && Larx.State.Mouse.LeftButton && button.Value.IsToggle && !State.MouseRepeat)
                        State.ActiveChildMenuKey = button.Key;

                    position.X += buttonSize + buttonSpacing;
                }

            position.X = bottomRightOrigin.X;
            foreach(var button in State.RightMenu) {
                if (intersect(button.Key, Larx.State.Mouse.Position, position, size) && Larx.State.Mouse.LeftButton && !State.MouseRepeat) {
                    State.SetControls(button.Key);
                    UpdateText(UiKeys.Texts.Radius, $"Radius: {Larx.State.ToolRadius}");
                    UpdateText(UiKeys.Texts.Hardness, $"Hardness: {Larx.State.ToolHardness}");
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
                State.PressedKey = Larx.State.Mouse.LeftButton && (!State.MouseRepeat || State.PressedKey == key) ? key : null;
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

            if (State.ActiveTopMenuKey != null) {
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