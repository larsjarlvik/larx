using System;
using System.Collections.Generic;
using Larx.Storage;
using OpenTK;

namespace Larx.TerrainV3
{
    public class TerrainQuadTree
    {
        public List<TerrainNode> Nodes { get; set; }

        public TerrainQuadTree()
        {
            Nodes = new List<TerrainNode>();

            for (var x = 0; x < TerrainConfig.RootNodes; x ++)
                for (var z = 0; z < TerrainConfig.RootNodes; z ++)
                    Nodes.Add(new TerrainNode(
                        new Vector2((float)x / TerrainConfig.RootNodes, (float)z / TerrainConfig.RootNodes),
                        0, new Vector2(x, z)
                    ));


            for (var i = 0; i < TerrainConfig.LodRange.Length; i ++) {
                if (TerrainConfig.LodRange[i] == 0)
                    break;

                var morphArea = (Map.MapData.MapSize / TerrainConfig.RootNodes) / (int)Math.Pow(2, i + 1);
                TerrainConfig.LodMorphAreas[i] = (TerrainConfig.LodRange[i] - morphArea);
            }
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