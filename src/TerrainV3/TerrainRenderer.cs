using Larx.Storage;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.TerrainV3
{
    public class TerrainRenderer
    {
        private int vaoId;
        private TerrainShader shader;
        private TerrainQuadTree quadTree;
        private Matrix4 worldTransform;
        private Vector3 lastCameraPosition;

        public TerrainRenderer()
        {
            shader = new TerrainShader();
            quadTree = new TerrainQuadTree();
            worldTransform = Matrix4.CreateScale(Map.MapData.MapSize) * Matrix4.CreateTranslation(-Map.MapData.MapSize / 2.0f, 0.0f, -Map.MapData.MapSize / 2.0f);

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

            GL.UniformMatrix4(shader.WorldMatrix, false, ref worldTransform);
            for (int i = 0; i < 8; i++){
                GL.Uniform1(shader.LodMorphAreas[i], TerrainConfig.LodMorphAreas[i]);
            }

            GL.BindVertexArray(vaoId);
            GL.EnableVertexAttribArray(0);
            quadTree.Render(shader);

            GL.BindVertexArray(0);
        }
    }
}