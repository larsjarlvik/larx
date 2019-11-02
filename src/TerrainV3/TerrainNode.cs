using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.TerrainV3
{
    public class TerrainNode
    {
        public float Size;
        public Vector3 Position;
        public TerrainNode[] Children { get; set; }
        public bool IsLeafNode { get; set; }
        public int Lod { get; set; }


        public TerrainNode(Vector3 position, int lod)
        {
            Children = new TerrainNode[4];
            Lod = lod;
            Size = 1.0f / (TerrainConfig.RootNodes * MathF.Pow(2.0f, Lod));
            Position = position;
        }

        public void UpdateQuadTree(Camera camera)
        {
            var distance = (camera.Position - (Position * TerrainConfig.ScaleXZ)).Length;

            if (distance < TerrainConfig.LodRange[Lod])
                addChildren(camera);
            else
                removeChildren();
        }

        public void Render(TerrainShader shader)
        {
            if (IsLeafNode) {
                var depth = (float)Lod / (float)TerrainConfig.RootNodes;

                GL.Uniform2(shader.Position, Position.Xz);
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
                    var pos = Position + new Vector3(x * (Size / 2.0f), 0.0f, z * (Size / 2.0f));
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