using System.IO;

namespace Larx.UserInterface
{
    public class UiElement
    {
        public int Texture { get; set; }
        public bool IsToggle { get; set; }


        public UiElement(string texturePath, bool isToggle = false, bool mipMap = false)
        {
            Texture = addTexture(texturePath, mipMap);
            IsToggle = isToggle;

        }

        private int addTexture(string path, bool mipMap = false)
        {
            var texture = new Texture();
            texture.LoadTexture(Path.Combine("resources", path), mipMap);
            return texture.TextureId;
        }
    }
}