using System;
using System.IO;
using glTFLoader.Schema;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Mesh
{
    public class MeshRenderer
    {
        private readonly MeshShader shader;
        private readonly Gltf model;
        private string modelName;
        private int vertexBuffer;
        private int indexBuffer;
        private int indexCount;

        public MeshRenderer(string name)
        {
            modelName = name;
            shader = new MeshShader();

            using(var fs = new FileStream(Path.Combine("resources", "models", $"{modelName}.gltf"), FileMode.Open))
                model = glTFLoader.Interface.LoadModel(fs);

            build();
        }


        private void build()
        {
            var vertices = BufferReader.readFloatBuffer(model, "POSITION");
            var indices = BufferReader.readIndices(model);

            indexCount = indices.Length;

            vertexBuffer = GL.GenBuffer();
            indexBuffer = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Length * Vector3.SizeInBytes, vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData<ushort>(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(ushort), indices, BufferUsageHint.StaticDraw);
        }

        public void Render(Camera camera, Vector3 position)
        {
            GL.EnableVertexAttribArray(0);
            GL.UseProgram(shader.Program);
            GL.Uniform3(shader.Position, position.X, position.Y, position.Z);

            camera.ApplyCamera(shader);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedShort, IntPtr.Zero);
        }
    }
}