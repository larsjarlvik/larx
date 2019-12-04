using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace Larx.UserInterface.Widgets
{
    public enum Direction
    {
        Horizonal,
        Vertical,
    }


    public class Container : IWidget
    {
        public string Key { get; }
        public List<IWidget> Children { get; }
        public Direction Direction { get; }
        public float Padding { get; }
        public float ChildPadding;

        public Container(string key, Direction direction, List<IWidget> children, float padding = 10.0f, float childPadding = 0.0f)
        {
            Key = key;
            Children = children;
            Direction = direction;
            Padding = padding;
            ChildPadding = childPadding;
        }

        public Vector2 GetSize()
        {
            var pos = Vector2.Zero;

            pos.X = Direction == Direction.Horizonal ? Children.First().GetSize().X : Children.Max(x => x.GetSize().X);
            pos.Y = Direction == Direction.Horizonal ? Children.Max(x => x.GetSize().Y) : Children.First().GetSize().Y;

            for(var i = 1; i < Children.Count; i ++)
                pos = advance(Children[i], pos);

            return pos + new Vector2(Padding * 2.0f);
        }

        public IWidget Intersect(Vector2 mouse, Vector2 position)
        {
            var pos = getInitialPosition(position);

            foreach(var child in Children) {
                var intersect = child.Intersect(mouse, pos);
                if (intersect != null) return intersect;
                pos = advance(child, pos);
            }

            return null;
        }

        public void Render(Matrix4 matrix, Vector2 position)
        {
            var pos = getInitialPosition(position);
            foreach(var child in Children) {
                child.Render(matrix, pos);
                pos = advance(child, pos);
            }
        }

        private Vector2 advance(IWidget child, Vector2 pos)
        {
            if (Direction == Direction.Horizonal)
                return new Vector2(pos.X + child.GetSize().X + ChildPadding, pos.Y);

            return new Vector2(pos.X, pos.Y + child.GetSize().Y + ChildPadding);
        }

        private Vector2 getInitialPosition(Vector2 position)
        {
            return position + new Vector2(Padding);
        }
    }
}