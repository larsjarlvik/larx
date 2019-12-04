using System.IO;
using Larx.UserInterface.Panel;
using Larx.UserInterface.Text;
using OpenTK;
using OpenTK.Graphics;

namespace Larx.UserInterface.Widgets
{
    public class Button : IWidget
    {
        private const float padding = 10.0f;
        private const float textSize = 14.0f;

        public string Key { get; }
        private Vector2 size;
        private readonly DisplayText displayText;
        public bool Active { get; set; }

        public Button(string key, string text, float width)
        {
            Key = key;
            size = new Vector2(width, 30);
            displayText = Ui.State.TextRenderer.CreateText(text, textSize);
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

            Ui.State.PanelRenderer.RenderSolidPanel(matrix, position, size, new Vector3(0.6f, 0.82f, 0.0f), panelState, Active, 2.0f, true);
            Ui.State.TextRenderer.Render(displayText, matrix, new Vector2(position.X + size.X / 2.0f - displayText.Width / 2.0f, position.Y + textSize * 1.25f), 1.0f, 1.6f, new Color4(0f, 0f, 0f, 1f));
        }

        public IWidget Intersect(Vector2 mouse, Vector2 position)
        {
            return (
                mouse.X >= position.X && mouse.Y >= position.Y &&
                mouse.X <= position.X + size.X && mouse.Y <= position.Y + size.Y
            ) ? this : null;
        }
    }
}