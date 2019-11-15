using System;
using Larx.Storage;
using Larx.Terrain.Shaders;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Terrain
{
    public class TerrainNode
    {
        public float Size;
        public Vector2 Position;

        public float Depth { get; }

        private Matrix4 localTransform;
        private Vector3 worldPosition;

        public TerrainNode[] Children { get; set; }
        public bool IsLeafNode { get; set; }
        public int Lod { get; set; }
        public Vector2 Index { get; set; }


        public TerrainNode(Vector2 position, int lod, Vector2 index)
        {
            Children = new TerrainNode[4];
            Lod = lod;
            Index = index;
            Size = 1.0f / (TerrainConfig.RootNodes * MathF.Pow(2.0f, Lod));
            Position = position;
            Depth = (float)Lod / (float)TerrainConfig.RootNodes;

            var localScaling = new Vector3(Size, 0.0f, Size);
            var localTranslation = new Vector3(Position.X, 0.0f, Position.Y);

            localTransform = Matrix4.CreateScale(localScaling) * Matrix4.CreateTranslation(localTranslation);
            worldPosition = new Vector3(Position.X + (Size / 2.0f), 0.0f, Position.Y + (Size / 2.0f)) * Map.MapData.MapSize - new Vector3(Map.MapData.MapSize / 2.0f, 0.0f, Map.MapData.MapSize / 2.0f);
        }

        public void UpdateQuadTree(Camera camera)
        {
            var distance = (camera.Position - worldPosition).Length;

            if (distance < TerrainConfig.LodRange[Lod])
                addChildren(camera);
            else
                removeChildren();
        }

        public void Render(BaseShader shader)
        {
            if (IsLeafNode) {
                GL.UniformMatrix4(shader.LocalMatrix, false, ref localTransform);
                GL.Uniform2(shader.Position, Position);
                GL.Uniform1(shader.Lod, Lod);
                GL.Uniform2(shader.Index, Index);
                GL.Uniform1(shader.Size, Size);
                GL.DrawArrays(PrimitiveType.Patches, 0, 16);
                return;
            }

            foreach(var node in Children)
                if (node != null) node.Render(shader);
        }

        private void addChildren(Camera camera)
        {
            IsLeafNode = false;
            for (var x = 0; x < 2; x ++)
                for (var z = 0; z < 2; z ++)
                {
                    var pos = Position + new Vector2(x * (Size / 2.0f), z * (Size / 2.0f));
                    var node = new TerrainNode(pos, Lod + 1, new Vector2(x, z));
                    node.UpdateQuadTree(camera);
                    Children[z * 2 + x] = node;
                }
        }

        private void removeChildren()
        {
            IsLeafNode = true;
        }
    }
}