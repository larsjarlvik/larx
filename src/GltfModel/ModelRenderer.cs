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

        public ModelRenderer()
        {
            shader = new ModelShader();
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

            return new Model(name, meshes);
        }

        public void Render(Camera camera, Light light, Model model, Vector3 position)
        {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);

            GL.UseProgram(shader.Program);
            GL.Uniform3(shader.Position, position);

            camera.ApplyCamera(shader);
            light.ApplyLight(shader);

            foreach(var mesh in model.Meshes)
            {
                if (mesh.Material.DoubleSided) GL.Disable(EnableCap.CullFace);
                else GL.Enable(EnableCap.CullFace);

                GL.Uniform1(shader.Roughness, mesh.Material.Roughness);

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