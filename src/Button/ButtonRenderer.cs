using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Larx.Button
{
    public class ButtonRenderer
    {
        enum State
        {
            Default = 0,
            Hover = 1,
            Pressed = 2
        }

        public ButtonShader Shader { get; }
        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }

        private int vertexBuffer;
        private int textureBuffer;
        private Texture texture;
        private State state;

        public ButtonRenderer(string texturePath, Vector2 size)
        {
            Size = size;

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

        public void Render(Matrix4 pMatrix)
        {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.UseProgram(Shader.Program);

            GL.UniformMatrix4(Shader.Matrix, false, ref pMatrix);

            GL.BindTexture(TextureTarget.Texture2D, texture.TextureId);
            GL.Uniform1(Shader.Texture, 0);
            GL.Uniform2(Shader.Position, Position);
            GL.Uniform2(Shader.Size, Size);
            GL.Uniform1(Shader.State, (int)state);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, textureBuffer);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }

        public bool MouseIntersect(Point mousePos, ButtonState leftButton)
        {
            if (mousePos.X >= Position.X && mousePos.X < Position.X + Size.X &&
                mousePos.Y >= Position.Y && mousePos.Y < Position.Y + Size.Y) {
                state = leftButton == ButtonState.Pressed ? State.Pressed : State.Hover;
                return true;
            }

            state = State.Default;
            return false;
        }
    }
}