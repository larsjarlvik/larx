using System;
using System.Collections.Generic;
using System.IO;
using glTFLoader.Schema;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.GltfModel
{
    public class ModelRenderer
    {
        private readonly ModelShader shader;
        private readonly Gltf model;
        private string modelName;
        private List<Mesh> meshes;

        public ModelRenderer(string name)
        {
            modelName = name;
            meshes = new List<Mesh>();
            shader = new ModelShader();

            using(var fs = new FileStream(Path.Combine("resources", "models", $"{modelName}.gltf"), FileMode.Open))
                model = glTFLoader.Interface.LoadModel(fs);

            build();
        }


        private void build()
        {
            foreach(var mesh in model.Meshes) {
                meshes.Add(new Mesh(model, mesh));
            }
        }

        public void Render(Camera camera, Light light, Vector3 position)
        {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.UseProgram(shader.Program);
            GL.Uniform3(shader.Position, position);
            GL.Disable(EnableCap.CullFace);

            camera.ApplyCamera(shader);
            light.ApplyLight(shader);

            foreach(var mesh in meshes)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, mesh.Material.BaseColorTexture.TextureId);
                GL.Uniform1(shader.BaseColorTexture, 0);

                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, mesh.Material.NormalTexture.TextureId);
                GL.Uniform1(shader.NormalTexture, 1);

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.VertexBuffer);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.TexCoordBuffer);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.NormalBuffer);
                GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.IndexBuffer);
                GL.DrawElements(PrimitiveType.Triangles, mesh.IndexCount, DrawElementsType.UnsignedShort, IntPtr.Zero);
            }

            GL.Enable(EnableCap.CullFace);
        }
    }
}