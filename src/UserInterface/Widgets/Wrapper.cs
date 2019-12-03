using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace Larx.UserInterface.Widgets
{
    public class Wrapper : IWidget
    {
        public string Key { get; }
        public IWidget Child { get; }

        public Wrapper(IWidget child)
        {
            Child = child;
        }

        public Vector2 GetSize()
        {
            return Child.GetSize();
        }

        public IWidget Intersect(Vector2 mouse, Vector2 position)
        {
            return Child.Intersect(mouse, position);
        }

        public void Render(Matrix4 matrix, Vector2 position)
        {
            Ui.State.PanelRenderer.RenderSolidPanel(matrix, position, Child.GetSize(), new Vector3(0.204f, 0.286f, 0.333f), Panel.PanelState.Default, false);
            Child.Render(matrix, position);
        }
    }
}