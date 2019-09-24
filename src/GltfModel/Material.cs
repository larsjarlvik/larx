using System;
using System.IO;
using Gltf = glTFLoader.Schema;

namespace Larx.GltfModel
{
    public class Material
    {
        public readonly Texture NormalTexture;
        public readonly Texture BaseColorTexture;

        public Material(Gltf.Gltf model, Gltf.Mesh mesh)
        {
            var materialId = mesh.Primitives[0].Material;
            if (materialId == null)
                throw new Exception("Mesh does not have a material!");

            var material = model.Materials[(int)materialId];

            var normalTextureIndex = material.NormalTexture.Index;
            var baseColorTextureIndex = material.PbrMetallicRoughness.BaseColorTexture.Index;

            var normalTextureName = model.Images[(int)model.Textures[normalTextureIndex].Source].Uri;
            var baseColorTextureName = model.Images[(int)model.Textures[baseColorTextureIndex].Source].Uri;

            NormalTexture = new Texture();
            NormalTexture.LoadTexture(Path.Combine("resources", "models", normalTextureName), true);

            BaseColorTexture = new Texture();
            BaseColorTexture.LoadTexture(Path.Combine("resources", "models", baseColorTextureName), true);
        }
    }
}