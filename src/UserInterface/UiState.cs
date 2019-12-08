using Larx.UserInterface.Panel;
using Larx.UserInterface.Text;
using Larx.UserInterface.Widgets;

namespace Larx.UserInterface
{

    public class UiState
    {
        public IWidget Hover { get; set; }
        public IWidget Focused { get; set; }
        public IWidget Click { get; set; }
        public bool MousePressed { get; set; }
        public bool MouseRepeat { get; set; }
        public PanelRenderer PanelRenderer { get; }
        public TextRenderer TextRenderer { get; }

        public UiState()
        {
            PanelRenderer = new PanelRenderer();
            TextRenderer = new TextRenderer();
        }
    }
}