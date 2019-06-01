using System.Collections.Generic;
using System.Linq;
using Larx.Button;
using Larx.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Larx.UserInterFace
{
    public partial class Builder
    {
        protected Dictionary<string, TextRenderer> texts;
        protected Dictionary<string, ButtonRenderer> buttons;
        protected SizeF uiSize;

        public Builder()
        {
            texts = new Dictionary<string, TextRenderer>();
            buttons = new Dictionary<string, ButtonRenderer>();
        }

        public void AddText(string key, string text)
        {
            var tr = new TextRenderer();
            tr.CreateText(text, 12.0f);
            texts.Add(key, tr);
        }

        public void UpdateText(string key, string text)
        {
            texts[key].CreateText(text, 12.0f);
        }

        public string AddButton(string key, string texturePath)
        {
            var br = new ButtonRenderer(texturePath, new Vector2(60, 60));
            buttons.Add(key, br);

            return key;
        }

        public void Resize(SizeF uiSize)
        {
            this.uiSize = uiSize;
        }

        public string MouseUiIntersect(List<string> keys, Point mousePos, ButtonState leftButton)
        {
            return buttons
                .Where(x => keys.Contains(x.Key))
                .FirstOrDefault(x => x.Value.MouseIntersect(mousePos, leftButton)).Key;
        }
    }
}