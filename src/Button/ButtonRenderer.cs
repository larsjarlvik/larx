using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Button
{
    public class ButtonRenderer
    {
        public ButtonShader Shader { get; }
        private int vertexBuffer;
        private int textureBuffer;
        private Texture texture;

        public ButtonRenderer(string texturePath)
        {
            Shader = new ButtonShader();
            texture = new Texture();
            texture.LoadTexture(Path.Combine("resources", texturePath));

            build();
        }

        private void build()
        {
            vertexBuffer = GL.GenBuffer();
            textureBuffer = GL.GenBuffer();

            var vertices = new Vector2[] {
                new Vector2(1.0f, 1.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(0.0f, 1.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(0.0f, 0.0f),
            };

            var textures = new Vector2[] {
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 0.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(0.0f, 0.0f),
                new Vector2(1.0f, 1.0f),
                new Vector2(0.0f, 1.0f),
            };

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, Vector2.SizeInBytes * vertices.Length, vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, textureBuffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, Vector2.SizeInBytes * textures.Length, vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Double, false, Vector2.SizeInBytes, 0);
        }

        public void Render(Matrix4 pMatrix, Vector2 position, Vector2 size)
        {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.UseProgram(Shader.Program);

            GL.UniformMatrix4(Shader.Matrix, false, ref pMatrix);

            GL.BindTexture(TextureTarget.Texture2D, texture.TextureId);
            GL.Uniform1(Shader.Texture, 0);
            GL.Uniform2(Shader.Position, position);
            GL.Uniform2(Shader.Size, size);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, textureBuffer);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }
    }
}