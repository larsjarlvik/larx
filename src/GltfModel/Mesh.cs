using OpenTK;
using OpenTK.Graphics.OpenGL;
using Gltf = glTFLoader.Schema;

namespace Larx.GltfModel
{
    public class Mesh
    {
        public readonly int VertexBuffer;
        public readonly int NormalBuffer;
        public readonly int TexCoordBuffer;
        public readonly int IndexBuffer;
        public readonly int IndexCount;
        public readonly Material Material;

        public Mesh(string rootPath, Gltf.Gltf model, Gltf.Mesh mesh)
        {
            var vertices = BufferReader.readVec3(rootPath, model, mesh, "POSITION");
            var normals = BufferReader.readVec3(rootPath, model, mesh, "NORMAL");
            var texCoords = BufferReader.readVec2(rootPath, model, mesh, "TEXCOORD_0");
            var indices = BufferReader.readIndices(rootPath, model, mesh);

            IndexCount = indices.Length;
            Material = new Material(rootPath, model, mesh);

            VertexBuffer = GL.GenBuffer();
            NormalBuffer = GL.GenBuffer();
            TexCoordBuffer = GL.GenBuffer();
            IndexBuffer = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Length * Vector3.SizeInBytes, vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, TexCoordBuffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, texCoords.Length * Vector2.SizeInBytes, texCoords, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, NormalBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, normals.Length * Vector3.SizeInBytes, normals, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBuffer);
            GL.BufferData<ushort>(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(ushort), indices, BufferUsageHint.StaticDraw);
        }
    }
}