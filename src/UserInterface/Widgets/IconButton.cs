using System.IO;
using Larx.UserInterface.Panel;
using OpenTK;

namespace Larx.UserInterface.Widgets
{
    public class IconButton : IWidget
    {

        public string Key { get; }

        private const float margin = 2f;
        private Vector2 size;
        public int Texture { get; set; }
        public bool IsToggle { get; set; }
        public bool Active { get; set; }

        public IconButton(string key, string texturePath, bool isToggle = false, bool mipMap = false)
        {
            Key = key;
            Texture = addTexture(texturePath, mipMap);
            IsToggle = isToggle;
            size = new Vector2(50, 50);
        }

        private int addTexture(string path, bool mipMap = false)
        {
            var texture = new Texture();
            texture.LoadTexture(Path.Combine("resources", path), mipMap);
            return texture.TextureId;
        }

        public Vector2 GetSize()
        {
            return size + new Vector2(margin * 2.0f);
        }

        public void Render(Matrix4 matrix, Vector2 position)
        {
            var panelState = Key == Ui.State.Hover?.Key
                ? Ui.State.MousePressed ? PanelState.Active : PanelState.Hover
                : PanelState.Default;

            Ui.State.PanelRenderer.RenderImagePanel(matrix, position + new Vector2(margin), size, Texture, panelState, IsToggle && Active);
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