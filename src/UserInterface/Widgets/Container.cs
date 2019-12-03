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


        public Container(string key, Direction direction, List<IWidget> children, float padding = 10.0f)
        {
            Key = key;
            Children = children;
            Direction = direction;
            Padding = padding;
        }

        public Vector2 GetSize()
        {
            var pos = Children.FirstOrDefault()?.GetSize() ?? Vector2.Zero;
            for(var i = 1; i < Children.Count; i ++)
                pos += Direction == Direction.Horizonal ? new Vector2(Children[i].GetSize().X, 0.0f) : new Vector2(0.0f, Children[i].GetSize().Y);

            return pos + new Vector2(Padding * 2.0f);
        }

        public string Intersect(Vector2 mouse, Vector2 position)
        {
            var pos = getInitialPosition(position);
            string key = null;

            foreach(var child in Children) {
                var intersect = child.Intersect(mouse, pos);
                if (intersect != null) key = intersect;
                pos = advance(child, pos);
            }

            return key;
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
                return new Vector2(pos.X + child.GetSize().X, pos.Y);

            return new Vector2(pos.X, pos.Y + child.GetSize().Y);
        }

        private Vector2 getInitialPosition(Vector2 position)
        {
            return position + new Vector2(Padding);
        }
    }
}