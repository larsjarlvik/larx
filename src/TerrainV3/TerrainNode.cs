using System;
using System.Collections.Generic;
using Larx.Storage;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.TerrainV3
{
    public class TerrainNode
    {
        public float Size;
        public Vector2 Position;
        private Matrix4 localTransform;
        private Matrix4 worldTransform;
        private Vector3 worldPosition;

        public TerrainNode[] Children { get; set; }
        public bool IsLeafNode { get; set; }
        public int Lod { get; set; }


        public TerrainNode(Vector2 position, int lod)
        {
            Children = new TerrainNode[4];
            Lod = lod;
            Size = 1.0f / (TerrainConfig.RootNodes * MathF.Pow(2.0f, Lod));
            Position = position;

            var localScaling = new Vector3(Size, 0.0f, Size);
            var localTranslation = new Vector3(Position.X, 0.0f, Position.Y);

            localTransform = Matrix4.CreateScale(localScaling) * Matrix4.CreateTranslation(localTranslation);
            worldTransform = Matrix4.CreateScale(Map.MapData.MapSize) * Matrix4.CreateTranslation(-Map.MapData.MapSize / 2.0f, 0.0f, -Map.MapData.MapSize / 2.0f);
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

        public void Render(TerrainShader shader)
        {
            if (IsLeafNode) {
                var depth = (float)Lod / (float)TerrainConfig.RootNodes;

                GL.UniformMatrix4(shader.LocalMatrix, false, ref localTransform);
                GL.UniformMatrix4(shader.WorldMatrix, false, ref worldTransform);
                GL.Uniform2(shader.Position, Position);
                GL.Uniform1(shader.Size, Size);
                GL.Uniform1(shader.Depth, depth);
                GL.DrawArrays(PrimitiveType.Patches, 0, 16);
                return;
            }

            foreach(var node in Children)
                if (node != null) node.Render(shader);
        }

        private void addChildren(Camera camera)
        {
            IsLeafNode = false;
            for(var z = 0; z < 2; z ++)
                for(var x = 0; x < 2; x ++)
                {
                    var pos = Position + new Vector2(x * Size / 2.0f, z * Size / 2.0f);
                    var node = new TerrainNode(pos, Lod + 1);
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