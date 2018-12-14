using System.Collections.Generic;
using System.Linq;
using Larx.Text;
using OpenTK;

namespace Larx.UserInterFace
{
    public class UiBuilder
    {
        private Dictionary<string, TextRenderer> texts;

        public UiBuilder()
        {
            texts = new Dictionary<string, TextRenderer>();
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

        public void Resize(SizeF uiSize)
        {
            foreach(var text in texts)
                text.Value.Resize(uiSize);
        }

        public void Render()
        {
            var trs = texts.Values.GetEnumerator();
            for(var i = 0; i < texts.Count; i ++)
                texts.Values.ElementAt(i).Render(new Vector2(10, 20 + i * 20), 0.65f, 1.6f);
        }
    }
}