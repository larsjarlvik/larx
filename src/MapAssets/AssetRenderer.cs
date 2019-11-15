using System;
using Larx.GltfModel;
using Larx.Storage;
using Larx.Terrain;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.MapAssets
{
    public partial class AssetRenderer
    {
        protected readonly AssetShader Shader;
        protected readonly ShadowShader ShadowShader;


        protected AssetRenderer()
        {
            Shader = new AssetShader();
            ShadowShader = new ShadowShader();
        }

        public void Refresh(Model model, TerrainRenderer terrain)
        {
            var placedAssets = Map.MapData.Assets[model.ModelName];
            var positions = new Vector3[placedAssets.Count];
            var rotations = new float[placedAssets.Count];

            for(var i = 0; i < placedAssets.Count; i++)
            {
                positions[i] = new Vector3(placedAssets[i].Position.X, (float)terrain.HeightMap.GetElevationAtPoint(placedAssets[i].Position), placedAssets[i].Position.Y);
                rotations[i] = placedAssets[i].Rotation;
            }

            foreach(var mesh in model.Meshes) {
                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.AdditionalBuffers[0]);
                GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, positions.Length * Vector3.SizeInBytes, positions, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.AdditionalBuffers[1]);
                GL.BufferData<float>(BufferTarget.ArrayBuffer, rotations.Length * sizeof(float), rotations, BufferUsageHint.StaticDraw);
            }
        }

        protected void AppendBuffers(Model model)
        {
            foreach(var mesh in model.Meshes) {
                GL.BindVertexArray(mesh.VaoId);

                mesh.AdditionalBuffers = new int[2] {
                    GL.GenBuffer(),
                    GL.GenBuffer(),
                };

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.AdditionalBuffers[0]);
                GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
                GL.VertexAttribDivisor(4, 1);

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.AdditionalBuffers[1]);
                GL.VertexAttribPointer(5, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);
                GL.VertexAttribDivisor(5, 1);
            }
        }

        protected void Render(Model model, string key)
        {
            foreach(var mesh in model.Meshes)
            {
                if (mesh.Material.DoubleSided) GL.Disable(EnableCap.CullFace);
                else GL.Enable(EnableCap.CullFace);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, mesh.Material.BaseColorTexture.TextureId);
                GL.Uniform1(Shader.BaseColorTexture, 0);

                if (mesh.Material.NormalTexture != null) {
                    GL.ActiveTexture(TextureUnit.Texture1);
                    GL.BindTexture(TextureTarget.Texture2D, mesh.Material.NormalTexture.TextureId);
                    GL.Uniform1(Shader.NormalTexture, 1);
                }

                if (mesh.Material.RoughnessTexture != null) {
                    GL.ActiveTexture(TextureUnit.Texture2);
                    GL.BindTexture(TextureTarget.Texture2D, mesh.Material.RoughnessTexture.TextureId);
                    GL.Uniform1(Shader.RoughnessTexture, 2);
                    GL.Uniform1(Shader.Roughness, -1.0f);
                } else {
                    GL.Uniform1(Shader.Roughness, mesh.Material.Roughness);
                }

                GL.BindVertexArray(mesh.VaoId);
                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);
                GL.EnableVertexAttribArray(2);
                GL.EnableVertexAttribArray(3);
                GL.EnableVertexAttribArray(4);
                GL.EnableVertexAttribArray(5);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.IndexBuffer);
                GL.DrawElementsInstanced(PrimitiveType.Triangles, mesh.IndexCount, DrawElementsType.UnsignedShort, IntPtr.Zero, Map.MapData.Assets[key].Count);
            }

            GL.Enable(EnableCap.CullFace);
            GL.BindVertexArray(0);
        }

        protected void RenderShadowMap(Model model, string key)
        {
            foreach(var mesh in model.Meshes)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, mesh.Material.BaseColorTexture.TextureId);
                GL.Uniform1(ShadowShader.BaseColorTexture, 0);

                GL.BindVertexArray(mesh.VaoId);
                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);
                GL.EnableVertexAttribArray(4);
                GL.EnableVertexAttribArray(5);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.IndexBuffer);
                GL.DrawElementsInstanced(PrimitiveType.Triangles, mesh.IndexCount, DrawElementsType.UnsignedShort, IntPtr.Zero, Map.MapData.Assets[key].Count);
            }

            GL.BindVertexArray(0);
        }
    }
}