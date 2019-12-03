using System;
using Larx.UserInterface.Text;
using OpenTK;

namespace Larx.UserInterface.Widgets
{
    public class Label : IWidget
    {
        public string Key { get; }

        private string currentText;

        public float TextSize { get; }
        private DisplayText displayText;

        public Label(string key, string text, float textSize = 13.0f)
        {
            TextSize = textSize;
            Key = key;

            currentText = text;
            displayText = Ui.State.TextRenderer.CreateText(text, textSize);
        }

        public Vector2 GetSize()
        {
            return new Vector2(displayText.Width, TextSize * 1.25f);
        }

        public string Intersect(Vector2 mouse, Vector2 position)
        {
            return null;
        }

        public void UpdateText(string text)
        {
            if (currentText == text) return;
            currentText = text;
            displayText = Ui.State.TextRenderer.CreateText(text, TextSize);
        }

        public void Render(Matrix4 matrix, Vector2 position)
        {
            var pos = new Vector2(position.X, position.Y + (TextSize / 2.0f) * 1.25f);
            Ui.State.TextRenderer.Render(displayText, matrix, pos, 1.0f, 1.6f);
        }
    }
}