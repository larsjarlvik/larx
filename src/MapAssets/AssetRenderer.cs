using System;
using Larx.GltfModel;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.MapAssets
{
    public class AssetRenderer
    {
        private readonly AssetShader shader;
        private readonly ShadowShader shadowShader;

        public AssetRenderer()
        {
            shader = new AssetShader();
            shadowShader = new ShadowShader();
        }

        public void Render(Camera camera, Light light, Model model, Vector3 position, float rotation)
        {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.EnableVertexAttribArray(2);
            GL.EnableVertexAttribArray(3);

            GL.UseProgram(shader.Program);
            GL.Uniform3(shader.Position, position);
            GL.Uniform1(shader.Rotation, rotation);

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

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.TangentBuffer);
                GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, Vector4.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.IndexBuffer);
                GL.DrawElements(PrimitiveType.Triangles, mesh.IndexCount, DrawElementsType.UnsignedShort, IntPtr.Zero);
            }

            GL.Enable(EnableCap.CullFace);
        }

        public void RenderShadowMap(Matrix4 projectionMatrix, Matrix4 viewMatrix, Model model, Vector3 position, float rotation)
        {
            GL.EnableVertexAttribArray(0);
            GL.UseProgram(shadowShader.Program);

            GL.UniformMatrix4(shadowShader.ViewMatrix, false, ref viewMatrix);
            GL.UniformMatrix4(shadowShader.ProjectionMatrix, false, ref projectionMatrix);
            GL.Uniform3(shadowShader.Position, position);
            GL.Uniform1(shadowShader.Rotation, rotation);

            foreach(var mesh in model.Meshes)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.VertexBuffer);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.IndexBuffer);
                GL.DrawElements(PrimitiveType.Triangles, mesh.IndexCount, DrawElementsType.UnsignedShort, IntPtr.Zero);
            }
        }
    }
}