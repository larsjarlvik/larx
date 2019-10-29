using System;
using System.Collections.Generic;
using Larx.GltfModel;
using Larx.Shadows;
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
        private Dictionary<string, int> positionBuffers;
        private Dictionary<string, int> rotationBuffers;


        protected AssetRenderer()
        {
            Shader = new AssetShader();
            ShadowShader = new ShadowShader();
            positionBuffers = new Dictionary<string, int>();
            rotationBuffers = new Dictionary<string, int>();
        }

        public void Refresh(TerrainRenderer terrain)
        {

            foreach(var key in Map.MapData.Assets.Keys)
            {
                var positions = new Vector3[Map.MapData.Assets[key].Count];
                var rotations = new float[Map.MapData.Assets[key].Count];

                if (!positionBuffers.ContainsKey(key)) positionBuffers.Add(key, GL.GenBuffer());
                if (!rotationBuffers.ContainsKey(key)) rotationBuffers.Add(key, GL.GenBuffer());

                for(var i = 0; i < Map.MapData.Assets[key].Count; i++)
                {
                    positions[i] = new Vector3(Map.MapData.Assets[key][i].Position.X, (float)terrain.GetElevationAtPoint(Map.MapData.Assets[key][i].Position), Map.MapData.Assets[key][i].Position.Y);
                    rotations[i] = Map.MapData.Assets[key][i].Rotation;
                }

                GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffers[key]);
                GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, positions.Length * Vector3.SizeInBytes, positions, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, 0, 0);
                GL.VertexAttribDivisor(4, 1);

                GL.BindBuffer(BufferTarget.ArrayBuffer, rotationBuffers[key]);
                GL.BufferData<float>(BufferTarget.ArrayBuffer, rotations.Length * sizeof(float), rotations, BufferUsageHint.StaticDraw);
                GL.VertexAttribPointer(5, 1, VertexAttribPointerType.Float, false, 0, 0);
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

                GL.ActiveTexture(TextureUnit.Texture1);
                GL.BindTexture(TextureTarget.Texture2D, mesh.Material.NormalTexture.TextureId);
                GL.Uniform1(Shader.NormalTexture, 1);

                if (mesh.Material.RoughnessTexture != null) {
                    GL.ActiveTexture(TextureUnit.Texture2);
                    GL.BindTexture(TextureTarget.Texture2D, mesh.Material.RoughnessTexture.TextureId);
                    GL.Uniform1(Shader.RoughnessTexture, 2);
                    GL.Uniform1(Shader.Roughness, -1.0f);
                } else {
                    GL.Uniform1(Shader.Roughness, mesh.Material.Roughness);
                }

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.VertexBuffer);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.TexCoordBuffer);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.NormalBuffer);
                GL.VertexAttribPointer(2, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.TangentBuffer);
                GL.VertexAttribPointer(3, 4, VertexAttribPointerType.Float, false, Vector4.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffers[key]);
                GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, rotationBuffers[key]);
                GL.VertexAttribPointer(5, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.IndexBuffer);
                GL.DrawElementsInstanced(PrimitiveType.Triangles, mesh.IndexCount, DrawElementsType.UnsignedShort, IntPtr.Zero, Map.MapData.Assets[key].Count);
            }

            GL.Enable(EnableCap.CullFace);
        }

        protected void RenderShadowMap(Model model, string key)
        {
            foreach(var mesh in model.Meshes)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, mesh.Material.BaseColorTexture.TextureId);
                GL.Uniform1(ShadowShader.BaseColorTexture, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.VertexBuffer);
                GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, mesh.TexCoordBuffer);
                GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffers[key]);
                GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, rotationBuffers[key]);
                GL.VertexAttribPointer(5, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.IndexBuffer);
                GL.DrawElementsInstanced(PrimitiveType.Triangles, mesh.IndexCount, DrawElementsType.UnsignedShort, IntPtr.Zero, Map.MapData.Assets[key].Count);
            }
        }
    }
}