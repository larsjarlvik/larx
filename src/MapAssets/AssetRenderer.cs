using System;
using Larx.GltfModel;
using Larx.Shadows;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.MapAssets
{
    public partial class AssetRenderer
    {
        protected readonly AssetShader Shader;
        protected readonly ShadowShader ShadowShader;

        public AssetRenderer()
        {
            Shader = new AssetShader();
            ShadowShader = new ShadowShader();
        }

        public void Render(Camera camera, Light light, ShadowRenderer shadows, Model model, Vector3 position, float rotation)
        {
            GL.Uniform3(Shader.Position, position);
            GL.Uniform1(Shader.Rotation, rotation);

            foreach(var mesh in model.Meshes)
            {
                if (mesh.Material.DoubleSided) GL.Disable(EnableCap.CullFace);
                else GL.Enable(EnableCap.CullFace);

                GL.Uniform1(Shader.Roughness, mesh.Material.Roughness);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, mesh.Material.BaseColorTexture.TextureId);
                GL.Uniform1(Shader.BaseColorTexture, 0);

                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, mesh.Material.NormalTexture.TextureId);
                GL.Uniform1(Shader.NormalTexture, 1);

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.VertexBuffer);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.TexCoordBuffer);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.NormalBuffer);
                GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.TangentBuffer);
                GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, Vector4.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.IndexBuffer);
                GL.DrawElements(PrimitiveType.Triangles, mesh.IndexCount, DrawElementsType.UnsignedShort, IntPtr.Zero);
            }

            GL.Enable(EnableCap.CullFace);
        }

        public void RenderShadowMap(Model model, Vector3 position, float rotation)
        {
            GL.Uniform3(ShadowShader.Position, position);
            GL.Uniform1(ShadowShader.Rotation, rotation);

            foreach(var mesh in model.Meshes)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, mesh.Material.BaseColorTexture.TextureId);
                GL.Uniform1(ShadowShader.BaseColorTexture, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.VertexBuffer);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.TexCoordBuffer);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.IndexBuffer);
                GL.DrawElements(PrimitiveType.Triangles, mesh.IndexCount, DrawElementsType.UnsignedShort, IntPtr.Zero);
            }
        }
    }
}