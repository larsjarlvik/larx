using System.Collections.Generic;
using OpenTK;

namespace Larx.TerrainV3
{
    public class TerrainQuadTree
    {
        public List<TerrainNode> Nodes { get; set; }

        public TerrainQuadTree()
        {
            Nodes = new List<TerrainNode>();

            for (var x = 0.0f; x < TerrainConfig.RootNodes; x ++)
                for (var z = 0.0f; z < TerrainConfig.RootNodes; z ++)
                    Nodes.Add(new TerrainNode(new Vector3((x / TerrainConfig.RootNodes) - 0.5f, 0.0f, (z / TerrainConfig.RootNodes) - 0.5f), 0));
        }

        public void UpdateQuadTree(Camera camera)
        {
            foreach(var node in Nodes)
                node.UpdateQuadTree(camera);
        }

        public void Render(TerrainShader shader)
        {
            foreach(var node in Nodes)
                node.Render(shader);
        }
    }
}