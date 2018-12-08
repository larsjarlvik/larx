using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Larx 
{
    public class Shader 
    {
        public int Program { get; }

        public Shader(string name) 
        {
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShader, System.IO.File.ReadAllText($"shaders\\{name}.vs.glsl"));
            GL.CompileShader(vertexShader);
            checkCompileStatus($"Vertex Shader: {name}", vertexShader);

            GL.ShaderSource(fragmentShader, System.IO.File.ReadAllText($"shaders\\{name}.fs.glsl"));
            GL.CompileShader(fragmentShader);
            checkCompileStatus($"Fragment Shader: {name}", fragmentShader);
            
            Program = GL.CreateProgram();
            GL.AttachShader(Program, fragmentShader);
            GL.AttachShader(Program, vertexShader);
            GL.LinkProgram(Program);
            GL.UseProgram(Program);

            GL.ValidateProgram(Program);
            Console.WriteLine(GL.GetProgramInfoLog(Program));

            SetUniforms();
        }

        protected virtual void SetUniforms() {}

        private void checkCompileStatus(string shaderName, int shader)
        {
            int compileStatus;

            GL.GetShader(shader, ShaderParameter.CompileStatus, out compileStatus);
            if (compileStatus != 1)
                throw new Exception($"Filed to Compiler {shaderName}: {GL.GetShaderInfoLog(shader)}");
        }
    }
}