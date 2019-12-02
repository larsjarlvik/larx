using Larx.UserInterface.Text;
using Larx.UserInterface.Panel;
using OpenTK;
using OpenTK.Input;
using System;

namespace Larx.UserInterface
{
    public class TextBox
    {
        private const float padding = 10.0f;
        private const float textSize = 14.0f;

        private readonly PanelRenderer panelRenderer;
        private readonly TextRenderer textRenderer;
        private DisplayText displayText;
        private string text;

        public TextBox(PanelRenderer panelRenderer, TextRenderer textRenderer)
        {
            this.text = "";
            this.panelRenderer = panelRenderer;
            this.textRenderer = textRenderer;
            this.displayText = textRenderer.CreateText(text, textSize);
        }

        public void Render(Matrix4 pMatrix, Vector2 position, Vector2 size, bool active)
        {
            panelRenderer.RenderSolidPanel(pMatrix, position, size, new Vector3(0.2f, 0.2f, 0.2f), PanelState.Default, active, 1.0f);
            textRenderer.Render(displayText, pMatrix, new Vector2(position.X + padding, position.Y + textSize * 1.25f), 1.0f, 1.6f);
        }

        public void KeyPress(Char key, float maxWidth)
        {
            if (key == 27) {
                if (text.Length > 0) text = text.Substring(0, text.Length - 1);
            } else {
                text += key;
            }

            displayText = textRenderer.CreateText(text, textSize);
            if (displayText.Width > maxWidth - padding * 2)
                KeyPress((char)27, maxWidth);
        }
    }
}