

using System;
using System.Collections.Generic;
using System.IO;
using glTFLoader.Schema;
using OpenTK;

namespace Larx.Mesh
{
    public static class BufferReader
    {
        private static Accessor getAccessor(Gltf model, string key)
        {
            if (model.Meshes[0].Primitives[0].Attributes.TryGetValue(key, out var index))
                return model.Accessors[index];

            throw new Exception($"Accessor not found! ({key})");
        }

        private static byte[] readBuffer(Gltf model, Accessor accessor)
        {
            var bufferView = model.BufferViews[(int)accessor.BufferView];
            var buffer = model.Buffers[bufferView.Buffer];

            using(var fs = new FileStream(Path.Combine("resources", "models", buffer.Uri), FileMode.Open))
                using(var reader = new BinaryReader(fs))
                {
                    var byteArray = new byte[bufferView.ByteLength];

                    reader.BaseStream.Seek(bufferView.ByteOffset, SeekOrigin.Begin);
                    return reader.ReadBytes(bufferView.ByteLength);
                }
        }

        public static Vector3[] readFloatBuffer(Gltf model, string key)
        {
            var result = new List<Vector3>();
            var accessor = getAccessor(model, key);

            var buffer = readBuffer(model, accessor);
            var floatArray = new float[accessor.Count * 3];
            System.Buffer.BlockCopy(buffer, 0, floatArray, 0, buffer.Length);

            for(var i = 0; i < accessor.Count * 3; i += 3) {
                result.Add(new Vector3(floatArray[i], floatArray[i + 1], floatArray[i + 2]));
            }

            return result.ToArray();
        }

        public static ushort[] readIndices(Gltf model)
        {
            var accessor = model.Accessors[(int)model.Meshes[0].Primitives[0].Indices];
            var buffer = readBuffer(model, accessor);

            var indexArray = new ushort[accessor.Count];
            System.Buffer.BlockCopy(buffer, 0, indexArray, 0, buffer.Length);

            return indexArray;
        }

    }
}