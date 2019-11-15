using OpenTK.Graphics.OpenGL;
using Larx.Storage;
using OpenTK;
using Larx.TerrainV3.Shaders;
using System;
using System.Linq;

namespace Larx.TerrainV3
{
    public class SplatMap
    {
        public int Texture;

        public SplatMap()
        {
            Texture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2DArray, Texture);
            GL.TexStorage3D(TextureTarget3d.Texture2DArray, 1, SizedInternalFormat.R8, State.SplatDetail, State.SplatDetail, TerrainConfig.Textures.Length);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            build();
            Refresh();
        }

        private void build()
        {
            for (var z = 0; z < State.SplatDetail; z ++)
                for (var x = 0; x < State.SplatDetail; x ++)
                    Map.MapData.SplatMap[0][z, x] = 1.0f;
        }

        public void Refresh()
        {
            for(var n = 0; n < TerrainConfig.Textures.Length; n ++)
                ToTexture(n);
        }

        public void Paint(Vector3 mousePosition)
        {
            if (State.ActiveToolBarItem == null) return;

            var position = (mousePosition.Xz / Map.MapData.MapSize);
            position.X = (position.X + 0.5f) * State.SplatDetail;
            position.Y = (position.Y + 0.5f) * State.SplatDetail;

            update(position, byte.Parse(State.ActiveToolBarItem));
        }

        private void update(Vector2 pos, byte splatId)
        {
            var radius = (int)(State.ToolRadius * State.SplatDetail / Map.MapData.MapSize);
            Func<float, float> calcP = (float t) => MathF.Pow(1f - t, 2) * MathF.Pow(1f + t, 2);
            var hasChanged = new bool[TerrainConfig.Textures.Length];

            for (var z1 = (int)(pos.Y - radius); z1 < pos.Y + radius; z1 ++)
                for (var x1 = (int)(pos.X - radius); x1 < pos.X + radius; x1 ++)
                {
                    if (x1 >= State.SplatDetail || x1 < 0 || z1 >= State.SplatDetail || z1 < 0) continue;

                    var distance = Vector2.Distance(pos, new Vector2(x1, z1));
                    if (distance > radius) continue;

                    var n = calcP(MathF.Min(1.0f, MathF.Sqrt((distance / radius > (State.ToolHardness * 0.1f) ? distance : 0.0f) / radius)));
                    var result = Map.MapData.SplatMap[splatId][z1, x1] + n;

                    Map.MapData.SplatMap[splatId][z1, x1] = result > 1.0f ? 1.0f : result;
                    average(ref hasChanged, z1, x1);
                }

            for(var i = 0; i < TerrainConfig.Textures.Length; i++)
                if (hasChanged[i]) ToTexture(i);
        }

        private void average(ref bool[] hasChanged, int z, int x)
        {
            var split = 1.0f / Map.MapData.SplatMap.Sum(s => s[z, x]);

            for(var i = 0; i < TerrainConfig.Textures.Length; i++) {
                Map.MapData.SplatMap[i][z, x] *= split;
                if ((split > 1.02f || split < 0.98f) && Map.MapData.SplatMap[i][z, x] > 0.0f)
                    hasChanged[i] = true;
            }
        }

        public void ToTexture(int textureId)
        {
            GL.BindTexture(TextureTarget.Texture2DArray, Texture);
            GL.TexSubImage3D<float>(TextureTarget.Texture2DArray, 0, 0, 0, textureId, State.SplatDetail, State.SplatDetail, 1, PixelFormat.Red, PixelType.Float, Map.MapData.SplatMap[textureId]);
        }
    }
}