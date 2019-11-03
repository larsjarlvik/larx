using OpenTK.Graphics.OpenGL;

namespace Larx.TerrainV3
{
    public class TerrainShader : Shader
    {
        public TerrainShader() : base("terrain-v2") { }

        public int Position { get; private set; }
        public int Size { get; private set; }
        public int Depth { get; private set; }
        public int LocalMatrix { get; private set; }
        public int WorldMatrix { get; private set; }
        public int Lod { get; private set; }
        public int Index { get; private set; }
        public int[] LodMorphAreas { get; private set; }
        public int TessFactor { get; private set; }
        public int TessSlope { get; private set; }
        public int TessShift { get; private set; }
        public int HeightMap { get; private set; }

        protected override void SetUniformsLocations()
        {
            base.SetUniformsLocations();
            Position = GL.GetUniformLocation(Program, "uPosition");
            Size = GL.GetUniformLocation(Program, "uSize");
            Depth = GL.GetUniformLocation(Program, "uDepth");
            LocalMatrix = GL.GetUniformLocation(Program, "uLocalMatrix");
            WorldMatrix = GL.GetUniformLocation(Program, "uWorldMatrix");
            Lod = GL.GetUniformLocation(Program, "uLod");
            Index = GL.GetUniformLocation(Program, "uIndex");

            LodMorphAreas = new int[8];
            for (var i = 0; i < 8; i ++)
                LodMorphAreas[i] = GL.GetUniformLocation(Program, $"uLodMorphAreas[{i}]");

            TessFactor = GL.GetUniformLocation(Program, "uTessFactor");
            TessSlope = GL.GetUniformLocation(Program, "uTessSlope");
            TessShift = GL.GetUniformLocation(Program, "uTessShift");
            HeightMap = GL.GetUniformLocation(Program, "uHeightMap");
        }
    }
}