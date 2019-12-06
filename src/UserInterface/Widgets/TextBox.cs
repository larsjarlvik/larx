using Larx.UserInterface.Text;
using Larx.UserInterface.Panel;
using OpenTK;
using OpenTK.Input;
using System;
using OpenTK.Graphics;

namespace Larx.UserInterface.Widgets
{
    public class TextBox : IWidget
    {
        public string Key { get; }
        private const float padding = 10.0f;
        private const float textSize = 14.0f;

        private DisplayText displayText;
        private readonly DisplayText cursor;
        private readonly Vector2 size;
        public string Text { get; private set; }

        public TextBox(string key, float width)
        {
            Key = key;
            Text = "";
            displayText = Ui.State.TextRenderer.CreateText(Text, textSize);
            cursor = Ui.State.TextRenderer.CreateText("|", 16.0f);
            size = new Vector2(width, 30);
        }

        public void KeyPress(Char key)
        {
            if (key == 27) {
                if (Text.Length > 0) Text = Text.Substring(0, Text.Length - 1);
            } else {
                Text += key;
            }

            displayText = Ui.State.TextRenderer.CreateText(Text, textSize);
            if (displayText.Width > size.X - padding * 2)
                KeyPress((char)27);
        }

        public Vector2 GetSize()
        {
            return size;
        }

        public void Render(Matrix4 matrix, Vector2 position)
        {
            Ui.State.PanelRenderer.RenderSolidPanel(matrix, position, size, new Color4(0.2f, 0.2f, 0.2f, 1.0f), PanelState.Default, Ui.State.Focused?.Key == Key, 1.0f);
            Ui.State.TextRenderer.Render(displayText, matrix, new Vector2(position.X + padding - 5.0f, position.Y + textSize * 1.3f), 1.0f, 1.6f);

            if (Larx.State.Time.Total % 1 > 0.5 && Ui.State.Focused?.Key == Key)
                Ui.State.TextRenderer.Render(cursor, matrix, new Vector2(position.X + displayText.Width + 2.0f, position.Y + textSize * 1.3f), 1.0f, 1.6f);
        }

        public IWidget Intersect(Vector2 mouse, Vector2 position)
        {
            var pos = position;
            return (
                mouse.X >= pos.X && mouse.Y >= pos.Y &&
                mouse.X <= pos.X + size.X && mouse.Y <= pos.Y + size.Y
            ) ? this : null;
        }
    }
}