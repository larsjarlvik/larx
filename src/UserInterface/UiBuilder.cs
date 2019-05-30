using System.Collections.Generic;
using System.Linq;
using Larx.Button;
using Larx.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.UserInterFace
{
    public class UiBuilder
    {
        private Dictionary<string, TextRenderer> texts;
        private Dictionary<string, ButtonRenderer> buttons;
        private SizeF uiSize;

        public UiBuilder()
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

        public void AddButton(string key, string texturePath)
        {
            var br = new ButtonRenderer(texturePath);
            buttons.Add(key, br);
        }

        public void Resize(SizeF uiSize)
        {
            this.uiSize = uiSize;
        }


        public void Render()
        {
            GL.Enable(EnableCap.Blend);
            var pMatrix = Matrix4.CreateOrthographicOffCenter(0, uiSize.Width, uiSize.Height, 0f, 0f, -1.0f);

            for(var i = 0; i < texts.Count; i ++)
                texts.Values.ElementAt(i).Render(pMatrix, new Vector2(10, 20 + i * 20), 0.65f, 1.6f);

            for(var i = 0; i < buttons.Count; i ++)
                buttons.Values.ElementAt(i).Render(pMatrix, new Vector2(20 + i * 60, uiSize.Height - 70), new Vector2(50, 50));

            GL.Disable(EnableCap.Blend);
        }
    }
}