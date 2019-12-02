using System;
using System.Collections.Generic;
using System.Linq;
using Larx.GltfModel;
using Larx.Storage;
using Larx.Terrain;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Assets
{
    public class AssetNode
    {
        private readonly float size;
        private readonly float worldSize;
        private List<PlacedAsset> assetsInNode;
        private Vector2 position;
        private Vector3 worldPosition;
        private AssetNode[] children;
        private readonly int level;
        private readonly int positionBuffer;
        private readonly int rotationBuffer;
        private readonly int scaleBuffer;
        private readonly Model model;

        public AssetNode(Model model, Vector2 position, int level)
        {
            positionBuffer = GL.GenBuffer();
            rotationBuffer = GL.GenBuffer();
            scaleBuffer = GL.GenBuffer();

            this.model = model;
            this.children = new AssetNode[4];
            this.level = level;
            this.size = (1.0f / (AssetConfig.RootNodes * MathF.Pow(2.0f, this.level)));
            this.position = position;
            this.worldSize = size * Map.MapData.MapSize;
            this.worldPosition = new Vector3(this.position.X + (size / 2.0f), 0.0f, this.position.Y + (size / 2.0f)) * Map.MapData.MapSize - new Vector3(Map.MapData.MapSize / 2.0f, 0.0f, Map.MapData.MapSize / 2.0f);
            this.assetsInNode = getAssetsInNode(model.ModelName);
        }

        public void UpdateQuadTree(TerrainRenderer terrain)
        {
            assetsInNode = getAssetsInNode(model.ModelName);

            if (!assetsInNode.Any())
                return;

            if (assetsInNode.Count > AssetConfig.SplitCount) {
                addChildren(terrain);
                return;
            }

            refresh(model, terrain);
        }

        public void Render(AssetShader shader, Vector4[] frustum)
        {
            if (!Frustum.CubeInFrustum(frustum, worldPosition, worldSize)) {
                return;
            }

            if (assetsInNode.Count > AssetConfig.SplitCount) {
                foreach(var node in children)
                    if (node != null) node.Render(shader, frustum);
                return;
            }

            foreach(var model in AssetConfig.Models) {
                var placedAssets = assetsInNode.Where(a => a.Model == model.Key).ToList();
                if (placedAssets.Any()) renderAsset(shader, placedAssets.First().Model, placedAssets.Count);
            }
        }

        private void addChildren(TerrainRenderer terrain)
        {
            for (var x = 0; x < 2; x ++)
                for (var z = 0; z < 2; z ++) {
                    var pos = position + new Vector2(x * (size / 2.0f), z * (size / 2.0f));
                    var node = new AssetNode(model, pos, level + 1);
                    node.UpdateQuadTree(terrain);
                    children[z * 2 + x] = node;
                }
        }

        private List<PlacedAsset> getAssetsInNode(string key)
        {
            if (!Map.MapData.Assets.ContainsKey(key))
                return new List<PlacedAsset>();

            var halfSize = worldSize / 2.0f;
            return Map.MapData.Assets[key].Where(a =>
                a.Position.X >= worldPosition.X - halfSize && a.Position.X < worldPosition.X + halfSize &&
                a.Position.Y >= worldPosition.Z - halfSize && a.Position.Y < worldPosition.Z + halfSize
            ).ToList();
        }

        private void refresh(Model model, TerrainRenderer terrain)
        {
            var placedAssets = assetsInNode.Where(a => a.Model == model.ModelName).ToList();
            var positions = new Vector3[placedAssets.Count];
            var rotations = new float[placedAssets.Count];
            var scales = new float[placedAssets.Count];

            for(var i = 0; i < placedAssets.Count; i++) {
                positions[i] = new Vector3(placedAssets[i].Position.X, (float)terrain.HeightMap.GetElevationAtPoint(placedAssets[i].Position), placedAssets[i].Position.Y);
                rotations[i] = placedAssets[i].Rotation;
                scales[i] = placedAssets[i].Scale;
            }

            foreach(var mesh in model.Meshes) {
                GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
                GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, positions.Length * Vector3.SizeInBytes, positions, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, rotationBuffer);
                GL.BufferData<float>(BufferTarget.ArrayBuffer, rotations.Length * sizeof(float), rotations, BufferUsageHint.StaticDraw);

                GL.BindBuffer(BufferTarget.ArrayBuffer, scaleBuffer);
                GL.BufferData<float>(BufferTarget.ArrayBuffer, scales.Length * sizeof(float), scales, BufferUsageHint.StaticDraw);
            }
        }

        private void renderAsset(AssetShader shader, string key, int count)
        {
            var model = AssetConfig.Models[key];

            foreach(var mesh in model.Meshes) {
                if (mesh.Material.DoubleSided) GL.Disable(EnableCap.CullFace);
                else GL.Enable(EnableCap.CullFace);

                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, mesh.Material.BaseColorTexture.TextureId);
                GL.Uniform1(shader.BaseColorTexture, 0);

                if (mesh.Material.NormalTexture != null) {
                    GL.ActiveTexture(TextureUnit.Texture1);
                    GL.BindTexture(TextureTarget.Texture2D, mesh.Material.NormalTexture.TextureId);
                    GL.Uniform1(shader.NormalTexture, 1);
                }

                if (mesh.Material.RoughnessTexture != null) {
                    GL.ActiveTexture(TextureUnit.Texture2);
                    GL.BindTexture(TextureTarget.Texture2D, mesh.Material.RoughnessTexture.TextureId);
                    GL.Uniform1(shader.RoughnessTexture, 2);
                    GL.Uniform1(shader.Roughness, -1.0f);
                } else {
                    GL.Uniform1(shader.Roughness, mesh.Material.Roughness);
                }

                GL.BindVertexArray(mesh.VaoId);
                GL.EnableVertexAttribArray(0);
                GL.EnableVertexAttribArray(1);
                GL.EnableVertexAttribArray(2);
                GL.EnableVertexAttribArray(3);
                GL.EnableVertexAttribArray(4);
                GL.EnableVertexAttribArray(5);
                GL.EnableVertexAttribArray(6);

                GL.BindBuffer(BufferTarget.ArrayBuffer, positionBuffer);
                GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
                GL.VertexAttribDivisor(4, 1);

                GL.BindBuffer(BufferTarget.ArrayBuffer, rotationBuffer);
                GL.VertexAttribPointer(5, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);
                GL.VertexAttribDivisor(5, 1);

                GL.BindBuffer(BufferTarget.ArrayBuffer, scaleBuffer);
                GL.VertexAttribPointer(6, 1, VertexAttribPointerType.Float, false, sizeof(float), 0);
                GL.VertexAttribDivisor(6, 1);

                GL.BindBuffer(BufferTarget.ElementArrayBuffer, mesh.IndexBuffer);
                GL.DrawElementsInstanced(PrimitiveType.Triangles, mesh.IndexCount, DrawElementsType.UnsignedShort, IntPtr.Zero, count);
            }

            GL.Enable(EnableCap.CullFace);
            GL.BindVertexArray(0);
        }
    }
}