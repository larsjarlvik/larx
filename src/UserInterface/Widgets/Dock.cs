using System.Collections.Generic;
using OpenTK;

namespace Larx.UserInterface.Widgets
{
    public enum DockPosition
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Center,
    }

    public struct Child
    {
        public readonly IWidget Component;
        public readonly DockPosition Position;

        public Child(DockPosition position, IWidget component)
        {
            Component = component;
            Position = position;
        }
    }

    public class Dock : IWidget
    {
        public string Key { get; }

        public Vector2 Size { get; }
        public float Padding { get; }
        public List<Child> Children { get; }

        public Dock(string key, Vector2 size, List<Child> children, float padding = 0.0f)
        {
            Key = key;
            Size = size;
            Padding = padding;
            Children = children;
        }

        public void Render(Matrix4 matrix, Vector2 position)
        {
            foreach(var child in Children) {
                var pos = position + getPosition(child);
                child.Component.Render(matrix, pos);
            }
        }

        public IWidget Intersect(Vector2 mouse, Vector2 position)
        {
            foreach(var child in Children) {
                var pos = position + getPosition(child);
                var intersect = child.Component.Intersect(mouse, pos);
                if (intersect != null) return intersect;
            }

            return null;
        }

        public Vector2 GetSize()
        {
            return Size;
        }

        private Vector2 getPosition(Child child)
        {
            var childSize = child.Component.GetSize();
            switch(child.Position)
            {
                case DockPosition.TopLeft:
                    return new Vector2(Padding);
                case DockPosition.TopRight:
                    return new Vector2(Size.X - Padding - childSize.X, Padding);
                case DockPosition.BottomLeft:
                    return new Vector2(Padding, Size.Y - Padding - childSize.Y);
                case DockPosition.BottomRight:
                    return new Vector2(Size.X - Padding - childSize.X, Size.Y - Padding - childSize.Y);
                case DockPosition.Center:
                    return Size / 2.0f - childSize / 2.0f;
            }

            return Vector2.Zero;
        }
    }
}