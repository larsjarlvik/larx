using System.Collections.Generic;
using System.IO;

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

        public static Model Load(string name)
        {
            var meshes = new List<Mesh>();
            var rootPath = Path.Combine("resources", "models", name);

            using(var fs = new FileStream(Path.Combine(rootPath, $"{name}.gltf"), FileMode.Open))
            {
                var root = glTFLoader.Interface.LoadModel(fs);

                foreach(var mesh in root.Meshes) {
                    meshes.Add(new Mesh(rootPath, root, mesh));
                }
            }

            return new Model("sky", meshes);
        }
    }
}