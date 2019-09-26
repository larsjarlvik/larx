using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Gltf = glTFLoader.Schema;

namespace Larx.GltfModel
{
    public class Model
    {
        public string ModelName { get; private set; }
        public IReadOnlyList<Mesh> Meshes { get; private set; }

        public Model(string modelName, List<Mesh> meshes)
        {
            ModelName = modelName;
            Meshes = meshes;
        }
    }
}