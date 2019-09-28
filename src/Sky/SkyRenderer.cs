using System;
using Larx.GltfModel;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Sky
{
    public class SkyRenderer
    {
        private readonly SkyShader shader;
        private readonly Model model;

        public SkyRenderer()
        {
            shader = new SkyShader();
            model = Model.Load("skydome");
        }

        public void Render(Camera camera)
        {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.UseProgram(shader.Program);

            camera.ApplyCamera(shader);
            GL.Uniform3(shader.CameraPosition, new Vector3(camera.Position.X, camera.Position.Y - 150.0f, camera.Position.Z));

            foreach(var mesh in model.Meshes)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, mesh.Material.BaseColorTexture.TextureId);
                GL.Uniform1(shader.BaseColorTexture, 0);

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