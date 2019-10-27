using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Buffers
{
    public class FramebufferRenderer
    {
        private readonly FramebufferShader shader;
        private Matrix4 pMatrix;
        private int vertexBuffer;
        private int textureBuffer;
        private Vector2 size;
        private Vector2 position;

        public FramebufferRenderer()
        {
            shader = new FramebufferShader();
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

        public void UpdateMatrix()
        {
            size = new Vector2(State.Window.Size.Width, State.Window.Size.Height) / 4.0f;
            position = new Vector2(State.Window.Size.Width - State.Window.Size.Width / 4.0f, 0.0f);
            pMatrix = Matrix4.CreateOrthographicOffCenter(0, State.Window.Size.Width, State.Window.Size.Height, 0f, 0f, -1.0f);
        }

        public void Render(int textureId)
        {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.UseProgram(shader.Program);

            GL.UniformMatrix4(shader.Matrix, false, ref pMatrix);
            GL.Uniform2(shader.Size, size);
            GL.Uniform2(shader.Position, position);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, textureId);
            GL.Uniform1(shader.Texture, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, textureBuffer);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }
    }
}
