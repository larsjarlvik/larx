using OpenTK;

namespace Larx.UserInterface.Widgets
{
    public interface IWidget
    {
        string Key { get; }

        void Render(Matrix4 matrix, Vector2 position);

        string Intersect(Vector2 mouse, Vector2 position);

        Vector2 GetSize();
    }
}