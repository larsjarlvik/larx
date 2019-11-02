using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.TerrainV3
{
    public class TerrainRenderer
    {
        private int vaoId;
        private TerrainShader shader;
        private TerrainQuadTree quadTree;
        private Vector3 lastCameraPosition;

        public TerrainRenderer(Camera camera)
        {
            shader = new TerrainShader();
            quadTree = new TerrainQuadTree();

            build();
        }

        public void Update(Camera camera)
        {
            if (camera.Position == lastCameraPosition)
                return;

            quadTree.UpdateQuadTree(camera);
            lastCameraPosition = camera.Position;
        }

        private void build()
        {
            var vertices = new Vector2[] {
                new Vector2(0,0),
                new Vector2(0.333f,0),
                new Vector2(0.666f,0),
                new Vector2(1,0),

                new Vector2(0,0.333f),
                new Vector2(0.333f,0.333f),
                new Vector2(0.666f,0.333f),
                new Vector2(1,0.333f),

                new Vector2(0,0.666f),
                new Vector2(0.333f,0.666f),
                new Vector2(0.666f,0.666f),
                new Vector2(1,0.666f),

                new Vector2(0,1),
                new Vector2(0.333f,1),
                new Vector2(0.666f,1),
                new Vector2(1,1),
            };

            vaoId = GL.GenVertexArray();
            GL.BindVertexArray(vaoId);

            var vertexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, vertices.Length * Vector2.SizeInBytes, vertices, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);
            GL.PatchParameter(PatchParameterInt.PatchVertices, 16);

            GL.BindVertexArray(0);
        }

        public void Render(Camera camera)
        {
            GL.UseProgram(shader.Program);
            shader.ApplyCamera(camera);

            GL.BindVertexArray(vaoId);
            GL.EnableVertexAttribArray(0);

            GL.Uniform1(shader.Scale, TerrainConfig.ScaleXZ);
            quadTree.Render(shader);

            GL.BindVertexArray(0);
        }
    }
}