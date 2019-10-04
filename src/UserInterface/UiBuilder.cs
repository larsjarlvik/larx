using System.Collections.Generic;
using System.Linq;
using Larx.Button;
using Larx.Text;
using OpenTK;

namespace Larx.UserInterFace
{
    public partial class Builder
    {
        protected Dictionary<string, TextRenderer> texts;
        protected Dictionary<string, ButtonRenderer> buttons;

        public Builder()
        {
            texts = new Dictionary<string, TextRenderer>();
            buttons = new Dictionary<string, ButtonRenderer>();
        }

        public void AddText(string key, string text)
        {
            var tr = new TextRenderer();
            tr.CreateText(text, 14.0f);
            texts.Add(key, tr);
        }

        public void UpdateText(string key, string text)
        {
            texts[key].CreateText(text, 14.0f);
        }

        public string AddButton(string key, string texturePath)
        {
            var br = new ButtonRenderer(texturePath, new Vector2(45, 45));
            buttons.Add(key, br);

            return key;
        }

        public string UiIntersect(List<string> keys, Vector2 position)
        {
            return buttons.Where(x => keys.Contains(x.Key) && x.Value.Intersect(position)).FirstOrDefault().Key;
        }
    }
}