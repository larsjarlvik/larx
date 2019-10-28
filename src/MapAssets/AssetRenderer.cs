using System;
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

        private int positionBuffer;
        private int rotationBuffer;

        protected AssetRenderer()
        {
            Shader = new AssetShader();
            ShadowShader = new ShadowShader();

            positionBuffer = GL.GenBuffer();
            rotationBuffer = GL.GenBuffer();
        }

        public void Refresh(TerrainRenderer terrain)
        {
            var positions = new Vector3[Map.MapData.Assets.Count];
            var rotations = new float[Map.MapData.Assets.Count];

            for(var i = 0; i < Map.MapData.Assets.Count; i++)
            {
                positions[i] = new Vector3(Map.MapData.Assets[i].Position.X, (float)terrain.GetElevationAtPoint(Map.MapData.Assets[i].Position), Map.MapData.Assets[i].Position.Y);
                rotations[i] = Map.MapData.Assets[i].Rotation;
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, positions.Length * Vector3.SizeInBytes, positions, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.VertexAttribDivisor(4, 1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, rotationBuffer);
            GL.BufferData<float>(BufferTarget.ArrayBuffer, rotations.Length * sizeof(float), rotations, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(5, 1, VertexAttribPointerType.Float, false, 0, 0);
            GL.VertexAttribDivisor(5, 1);
        }

        protected void Render(Model model, int count)
        {
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

                GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
                GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, rotationBuffer);
                GL.VertexAttribPointer(5, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.IndexBuffer);
                GL.DrawElementsInstanced(PrimitiveType.Triangles, mesh.IndexCount, DrawElementsType.UnsignedShort, IntPtr.Zero, count);
            }

            GL.Enable(EnableCap.CullFace);
        }

        protected void RenderShadowMap(Model model, int count)
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

                GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
                GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

                GL.BindBuffer(BufferTarget.ArrayBuffer, rotationBuffer);
                GL.VertexAttribPointer(5, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.IndexBuffer);
                GL.DrawElementsInstanced(PrimitiveType.Triangles, mesh.IndexCount, DrawElementsType.UnsignedShort, IntPtr.Zero, count);
            }
        }
    }
}