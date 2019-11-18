using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.UserInterface.Button
{
    public enum ButtonState
    {
        Default = 0,
        Hover = 1,
        Pressed = 2,
    }

    public class ButtonRenderer
    {
        public ButtonShader Shader { get; }

        private int vertexBuffer;
        private int textureBuffer;
        private int vaoId;

        public ButtonRenderer()
        {
            Shader = new ButtonShader();
            build();
        }

        private void build()
        {
            vaoId = GL.GenVertexArray();
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

            GL.BindVertexArray(vaoId);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, Vector2.SizeInBytes * vertices.Length, vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, textureBuffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, Vector2.SizeInBytes * textures.Length, vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindVertexArray(0);
        }

        public void Render(Matrix4 pMatrix, Vector2 position, Vector2 size, int textureId, ButtonState state, bool active)
        {
            GL.UseProgram(Shader.Program);

            GL.UniformMatrix4(Shader.Matrix, false, ref pMatrix);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.Uniform1(Shader.Texture, 0);
            GL.Uniform2(Shader.Position, position);
            GL.Uniform2(Shader.Size, size);
            GL.Uniform1(Shader.State, (int)state);
            GL.Uniform1(Shader.Active, active ? 1 : 0);

            GL.BindVertexArray(vaoId);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.BindVertexArray(0);
        }

        public bool Intersect(Vector2 mouse, Vector2 position, Vector2 size)
        {
            if (mouse.X >= position.X && mouse.X < position.X + size.X &&
                mouse.Y >= position.Y && mouse.Y < position.Y + size.Y) {
                return true;
            }

            return false;
        }
    }
}