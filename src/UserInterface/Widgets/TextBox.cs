using Larx.UserInterface.Text;
using Larx.UserInterface.Panel;
using OpenTK;
using OpenTK.Input;
using System;

namespace Larx.UserInterface.Widgets
{
    public class TextBox : IWidget
    {
        public string Key { get; }
        private const float padding = 10.0f;
        private const float margin = 5.0f;
        private const float textSize = 14.0f;

        private DisplayText displayText;
        private readonly Vector2 size;
        private string text;

        public TextBox(string key, float width)
        {
            Key = key;
            text = "";
            displayText = Ui.State.TextRenderer.CreateText(text, textSize);
            size = new Vector2(width, 30);
        }

        public void KeyPress(Char key, float maxWidth)
        {
            if (key == 27) {
                if (text.Length > 0) text = text.Substring(0, text.Length - 1);
            } else {
                text += key;
            }

            displayText = Ui.State.TextRenderer.CreateText(text, textSize);
            if (displayText.Width > maxWidth - padding * 2)
                KeyPress((char)27, maxWidth);
        }

        public Vector2 GetSize()
        {
            return size + new Vector2(margin * 2.0f);
        }

        public void Render(Matrix4 matrix, Vector2 position)
        {
            var pos = position + new Vector2(margin);
            Ui.State.PanelRenderer.RenderSolidPanel(matrix, pos, size, new Vector3(0.2f, 0.2f, 0.2f), PanelState.Default, false, 1.0f);
            Ui.State.TextRenderer.Render(displayText, matrix, new Vector2(pos.X + padding, pos.Y + textSize * 1.25f), 1.0f, 1.6f);
        }

        public string Intersect(Vector2 mouse, Vector2 position)
        {
            var pos = position + new Vector2(margin);
            return (
                mouse.X >= pos.X && mouse.Y >= pos.Y &&
                mouse.X <= pos.X + size.X && mouse.Y <= pos.Y + size.Y
            ) ? Key : null;
        }
    }
}