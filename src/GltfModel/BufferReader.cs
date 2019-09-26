

using System;
using System.Collections.Generic;
using System.IO;
using OpenTK;
using Gltf = glTFLoader.Schema;

namespace Larx.GltfModel
{
    public static class BufferReader
    {
        private static Gltf.Accessor getAccessor(Gltf.Gltf gltfModel, Gltf.Mesh gltfMesh, string key)
        {
            if (gltfMesh.Primitives[0].Attributes.TryGetValue(key, out var index))
                return gltfModel.Accessors[index];

            throw new Exception($"Accessor not found! ({key})");
        }

        private static byte[] readBuffer(string rootPath, Gltf.Gltf model, Gltf.Accessor accessor)
        {
            var bufferView = model.BufferViews[(int)accessor.BufferView];
            var buffer = model.Buffers[bufferView.Buffer];

            using(var fs = new FileStream(Path.Combine(rootPath, buffer.Uri), FileMode.Open))
                using(var reader = new BinaryReader(fs))
                {
                    var byteArray = new byte[bufferView.ByteLength];

                    reader.BaseStream.Seek(bufferView.ByteOffset, SeekOrigin.Begin);
                    return reader.ReadBytes(bufferView.ByteLength);
                }
        }

        public static Vector3[] readVec3(string rootPath, Gltf.Gltf model, Gltf.Mesh mesh, string key)
        {
            var result = new List<Vector3>();
            var accessor = getAccessor(model, mesh, key);

            var buffer = readBuffer(rootPath, model, accessor);
            var floatArray = new float[accessor.Count * 3];
            System.Buffer.BlockCopy(buffer, 0, floatArray, 0, buffer.Length);

            for(var i = 0; i < accessor.Count * 3; i += 3) {
                result.Add(new Vector3(floatArray[i], floatArray[i + 1], floatArray[i + 2]));
            }

            return result.ToArray();
        }

        public static Vector2[] readVec2(string rootPath, Gltf.Gltf model, Gltf.Mesh mesh, string key)
        {
            var result = new List<Vector2>();
            var accessor = getAccessor(model, mesh, key);

            var buffer = readBuffer(rootPath, model, accessor);
            var floatArray = new float[accessor.Count * 2];
            System.Buffer.BlockCopy(buffer, 0, floatArray, 0, buffer.Length);

            for(var i = 0; i < accessor.Count * 2; i += 2) {
                result.Add(new Vector2(floatArray[i], floatArray[i + 1]));
            }

            return result.ToArray();
        }

        public static ushort[] readIndices(string rootPath, Gltf.Gltf model, Gltf.Mesh mesh)
        {
            var accessor = model.Accessors[(int)mesh.Primitives[0].Indices];
            var buffer = readBuffer(rootPath, model, accessor);

            var indexArray = new ushort[accessor.Count];
            System.Buffer.BlockCopy(buffer, 0, indexArray, 0, buffer.Length);

            return indexArray;
        }
    }
}