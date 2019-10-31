using System;
using Larx.GltfModel;

namespace Larx.MapAssets
{
    [Serializable]
    public class Asset
    {
        public float RenderDistance { get; set; }
        public float RenderDistanceVariation { get; set; }
        public Model Model { get; set; }
    }
}