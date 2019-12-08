using System.IO;
using Larx.UserInterface.Panel;
using Larx.UserInterface.Text;
using OpenTK;
using OpenTK.Graphics;

namespace Larx.UserInterface.Widgets
{
    public enum ButtonStyle
    {
        Default,
        Action,
        Dismiss,
    }

    public enum Align
    {
        Left,
        Center,
    }

    public class Button : IWidget
    {
        private const float padding = 10.0f;
        private const float textSize = 14.0f;

        public string Key { get; }
        private Vector2 size;
        private readonly DisplayText displayText;
        private readonly ButtonStyle buttonStyle;
        private readonly float borderWidth;
        private readonly Align textAlign;

        public bool Active { get; set; }

        public Button(string key, string text, float width, ButtonStyle style = ButtonStyle.Default, float border = 2.0f, Align align = Align.Center)
        {
            Key = key;
            size = new Vector2(width, 30);
            displayText = Ui.State.TextRenderer.CreateText(text, textSize);
            buttonStyle = style;
            borderWidth = border;
            textAlign = align;
        }

        private int addTexture(string path, bool mipMap = false)
        {
            var texture = new Texture();
            texture.LoadTexture(Path.Combine("resources", path), mipMap);
            return texture.TextureId;
        }

        public Vector2 GetSize()
        {
            return size;
        }

        public void Render(Matrix4 matrix, Vector2 position)
        {
            var panelState = Key == Ui.State.Hover?.Key
                ? Ui.State.MousePressed ? PanelState.Active : PanelState.Hover
                : PanelState.Default;

            var textPosition = textAlign == Align.Center
                ? position.X + size.X / 2.0f - displayText.Size.X / 2.0f
                : position.X + 10.0f;

            Ui.State.PanelRenderer.RenderSolidPanel(matrix, position, size, getBackgroundColor(), panelState, Active, borderWidth, true);
            Ui.State.TextRenderer.Render(displayText, matrix, new Vector2(textPosition, position.Y + textSize * 1.3f), 1.0f, 1.6f, getForegroungColor());
        }

        public IWidget Intersect(Vector2 mouse, Vector2 position)
        {
            return (
                mouse.X >= position.X && mouse.Y >= position.Y &&
                mouse.X <= position.X + size.X && mouse.Y <= position.Y + size.Y
            ) ? this : null;
        }

        private Color4 getForegroungColor()
        {
            switch(buttonStyle) {
                case ButtonStyle.Action:
                case ButtonStyle.Dismiss:
                    return new Color4(0f, 0f, 0f, 1f);
                default:
                    return new Color4(1f, 1f, 1f, 1f);
            }
        }

        private Color4 getBackgroundColor()
        {
            switch(buttonStyle) {
                case ButtonStyle.Action:
                    return new Color4(0.6f, 0.82f, 0.0f, 1.0f);
                case ButtonStyle.Dismiss:
                    return new Color4(1.0f, 0.29f, 0.33f, 1.0f);
                default:
                    return new Color4(1.0f, 1.0f, 1.0f, 0.1f);
            }
        }
    }
}