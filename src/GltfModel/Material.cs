using System;
using System.IO;
using Gltf = glTFLoader.Schema;

namespace Larx.GltfModel
{
    public class Material
    {
        public readonly Texture NormalTexture;
        public readonly Texture BaseColorTexture;
        public readonly bool DoubleSided;
        public readonly float Roughness;

        public Material(string rootPath, Gltf.Gltf model, Gltf.Mesh mesh)
        {
            var materialId = mesh.Primitives[0].Material;
            if (materialId == null)
                throw new Exception("Mesh does not have a material!");

            var material = model.Materials[(int)materialId];


            var baseColorTextureIndex = material.PbrMetallicRoughness.BaseColorTexture.Index;
            var baseColorTextureName = model.Images[(int)model.Textures[baseColorTextureIndex].Source].Uri;
            BaseColorTexture = new Texture();
            BaseColorTexture.LoadTexture(Path.Combine(rootPath, baseColorTextureName), true);

            if (material.NormalTexture != null)
            {
                var normalTextureIndex = material.NormalTexture.Index;
                var normalTextureName = model.Images[(int)model.Textures[normalTextureIndex].Source].Uri;
                NormalTexture = new Texture();
                NormalTexture.LoadTexture(Path.Combine(rootPath, normalTextureName), true);
            }

            DoubleSided = material.DoubleSided;
            Roughness = material.PbrMetallicRoughness.RoughnessFactor;
        }
    }
}