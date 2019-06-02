namespace Larx.UserInterFace
{
    public class ToolbarItem
    {
        public TopMenu TopMenu { get; private set; }

        public string Key { get; private set; }

        public ToolbarItem(TopMenu topMenu, string key)
        {
            TopMenu = topMenu;
            Key = key;
        }
    }
}