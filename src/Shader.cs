using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Larx
{
    public class Shader
    {
        public int Program { get; }
        public int ProjectionMatrix { get; private set; }
        public int ViewMatrix { get; private set; }
        public int LightDirection { get; private set; }
        public int LightAmbient { get; private set; }
        public int LightDiffuse { get; private set; }
        public int LightSpecular { get; private set; }

        public Shader(string name)
        {
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShader, System.IO.File.ReadAllText(Path.Combine("shaders", $"{name}.vs.glsl")));
            GL.CompileShader(vertexShader);
            checkCompileStatus($"Vertex Shader: {name}", vertexShader);

            GL.ShaderSource(fragmentShader, System.IO.File.ReadAllText(Path.Combine("shaders", $"{name}.fs.glsl")));
            GL.CompileShader(fragmentShader);
            checkCompileStatus($"Fragment Shader: {name}", fragmentShader);

            Program = GL.CreateProgram();
            GL.AttachShader(Program, fragmentShader);
            GL.AttachShader(Program, vertexShader);
            GL.LinkProgram(Program);
            GL.UseProgram(Program);

            GL.ValidateProgram(Program);

            SetUniformsLocations();
        }

        protected virtual void SetUniformsLocations()
        {
            ProjectionMatrix = GL.GetUniformLocation(Program, "uProjectionMatrix");
            ViewMatrix = GL.GetUniformLocation(Program, "uViewMatrix");
            LightDirection = GL.GetUniformLocation(Program, "uLightDirection");
            LightAmbient = GL.GetUniformLocation(Program, "uLightAmbient");
            LightDiffuse = GL.GetUniformLocation(Program, "uLightDiffuse");
            LightSpecular = GL.GetUniformLocation(Program, "uLightSpecular");
        }

        private void checkCompileStatus(string shaderName, int shader)
        {
            int compileStatus;

            GL.GetShader(shader, ShaderParameter.CompileStatus, out compileStatus);
            if (compileStatus != 1)
                throw new Exception($"Filed to Compiler {shaderName}: {GL.GetShaderInfoLog(shader)}");
        }
    }
}