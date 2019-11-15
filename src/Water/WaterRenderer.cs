using System;
using System.IO;
using Larx.Buffers;
using Larx.Shadows;
using Larx.Storage;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Water
{
    public class WaterRenderer
    {
        private readonly WaterShader shader;
        private int vertexBuffer;
        private int indexBuffer;
        private int vaoId;
        private Texture dudvMap;
        private Texture normalMap;

        public readonly Framebuffer RefractionBuffer;
        public readonly Framebuffer ReflectionBuffer;

        public WaterRenderer()
        {
            shader = new WaterShader();
            RefractionBuffer = new Framebuffer(State.Window.Size);
            ReflectionBuffer = new Framebuffer(State.Window.Size);

            dudvMap = new Texture();
            normalMap = new Texture();
            dudvMap.LoadTexture(Path.Combine("resources", "textures", "water-dudv.png"), true);
            normalMap.LoadTexture(Path.Combine("resources", "textures", "water-normal.png"), true);

            build();
        }

        private void build()
        {
            var halfMapSize = Map.MapData.MapSize / 2;

            var vertices = new Vector3[] {
                new Vector3(-halfMapSize, 0.0f, halfMapSize),
                new Vector3( halfMapSize, 0.0f, halfMapSize),
                new Vector3( halfMapSize, 0.0f,-halfMapSize),
                new Vector3(-halfMapSize, 0.0f,-halfMapSize),
            };

            var indices = new int[] {
                0, 1, 2, 2, 3, 0,
            };

            vaoId = GL.GenVertexArray();
            vertexBuffer = GL.GenBuffer();
            indexBuffer = GL.GenBuffer();

            GL.BindVertexArray(vaoId);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, vertices.Length * Vector3.SizeInBytes, vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(int), indices, BufferUsageHint.StaticDraw);

            GL.BindVertexArray(0);
        }

        public void Render(Camera camera, Light light, ShadowBox shadows)
        {
            GL.UseProgram(shader.Program);

            shader.ApplyCamera(camera);
            shader.ApplyLight(light);
            shader.ApplyShadows(shadows);

            GL.Uniform1(shader.Near, State.Near);
            GL.Uniform1(shader.Far, State.Far);
            GL.Uniform1(shader.TimeOffset, (float)(State.Time.Total * 0.001 % 1));

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, RefractionBuffer.ColorTexture);
            GL.Uniform1(shader.RefractionColorTexture, 0);

            GL.ActiveTexture(TextureUnit.Texture1);
            GL.BindTexture(TextureTarget.Texture2D, RefractionBuffer.DepthTexture);
            GL.Uniform1(shader.RefractionDepthTexture, 1);

            GL.ActiveTexture(TextureUnit.Texture2);
            GL.BindTexture(TextureTarget.Texture2D, ReflectionBuffer.ColorTexture);
            GL.Uniform1(shader.ReflectionColorTexture, 2);

            GL.ActiveTexture(TextureUnit.Texture3);
            GL.BindTexture(TextureTarget.Texture2D, dudvMap.TextureId);
            GL.Uniform1(shader.DuDvMap, 3);

            GL.ActiveTexture(TextureUnit.Texture4);
            GL.BindTexture(TextureTarget.Texture2D, normalMap.TextureId);
            GL.Uniform1(shader.NormalMap, 4);

            GL.BindVertexArray(vaoId);
            GL.EnableVertexAttribArray(0);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer);
            GL.DrawElements(PrimitiveType.Triangles, 6, DrawElementsType.UnsignedInt, IntPtr.Zero);

            GL.BindVertexArray(0);
        }
    }
}