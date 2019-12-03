using System.Collections.Generic;
using Larx.UserInterface.Panel;
using Larx.UserInterface.Text;

namespace Larx.UserInterface
{

    public class UiState
    {
        public string HoverKey { get; set; }
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