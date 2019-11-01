using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Button
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
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public bool Active { get; set; }
        public ButtonState State;

        private int vertexBuffer;
        private int textureBuffer;
        private int vaoId;
        private Texture texture;

        public ButtonRenderer(string texturePath, Vector2 size)
        {
            Size = size;

            Shader = new ButtonShader();
            texture = new Texture();
            texture.LoadTexture(Path.Combine("resources", texturePath), true);

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

        public void Render(Matrix4 pMatrix)
        {
            GL.UseProgram(Shader.Program);

            GL.UniformMatrix4(Shader.Matrix, false, ref pMatrix);

            GL.BindTexture(TextureTarget.Texture2D, texture.TextureId);
            GL.Uniform1(Shader.Texture, 0);
            GL.Uniform2(Shader.Position, Position);
            GL.Uniform2(Shader.Size, Size);
            GL.Uniform1(Shader.State, (int)State);
            GL.Uniform1(Shader.Active, Active ? 1 : 0);

            GL.BindVertexArray(vaoId);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);

            GL.BindVertexArray(0);
        }

        public bool Intersect(Vector2 position)
        {
            if (position.X >= Position.X && position.X < Position.X + Size.X &&
                position.Y >= Position.Y && position.Y < Position.Y + Size.Y) {
                return true;
            }

            State = ButtonState.Default;
            return false;
        }
    }
}