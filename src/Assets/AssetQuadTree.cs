using System.Collections.Generic;
using Larx.Terrain;
using OpenTK;

namespace Larx.Assets
{
    public class AssetQuadTree
    {
        public List<AssetNode> Nodes { get; set; }

        public AssetQuadTree()
        {
            Nodes = new List<AssetNode>();

            foreach(var key in AssetConfig.Models.Keys)
                for (var x = 0; x < AssetConfig.RootNodes; x ++)
                    for (var z = 0; z < AssetConfig.RootNodes; z ++)
                        Nodes.Add(new AssetNode(
                            AssetConfig.Models[key],
                            new Vector2((float)x / AssetConfig.RootNodes, (float)z / AssetConfig.RootNodes), 0
                        ));
        }

        public void UpdateQuadTree(TerrainRenderer terrain)
        {
            foreach(var node in Nodes)
                node.UpdateQuadTree(terrain);
        }

        public void Render(Vector4[] frustumPlanes, AssetShader shader)
        {
            foreach(var node in Nodes)
                node.Render(shader, frustumPlanes);
        }
    }
}