using System;
using System.IO;
using System.Text;
using Larx.Shadows;
using OpenTK.Graphics.OpenGL;

namespace Larx
{
    public class Shader
    {
        private int shadowMap;
        private int shadowMatrix;
        private int enableShadows;

        public int Program { get; }
        public int ProjectionMatrix { get; private set; }
        public int ViewMatrix { get; private set; }
        public int CameraPosition { get; private set; }
        public int LightDirection { get; private set; }
        public int LightAmbient { get; private set; }
        public int LightDiffuse { get; private set; }
        public int LightSpecular { get; private set; }

        private void addShader(string name, ShaderType shaderType, string ext, string shaderDescriptor)
        {
            var path = Path.Combine("shaders", $"{name}.{ext}.glsl");

            if (!File.Exists(path))
                return;

            var contents = File.ReadLines(path);
            var finalShader = new StringBuilder();

            foreach(var line in contents)
            {
                if (line.StartsWith("#include")) {
                    var includeName = line.Split(' ')[1];
                    var include = File.ReadAllText(Path.Combine("shaders", "includes", $"{includeName}.glsl"));
                    finalShader.AppendLine(include);
                    continue;
                }

                finalShader.AppendLine(line);
            }

            var shaderSource = finalShader.ToString();

            var shader = GL.CreateShader(shaderType);
            GL.ShaderSource(shader, shaderSource);
            GL.CompileShader(shader);
            checkCompileStatus($"{shaderDescriptor}: {name}", shader);
            GL.AttachShader(Program, shader);

        }

        public Shader(string name)
        {
            Program = GL.CreateProgram();

            addShader(name, ShaderType.VertexShader, "vs", "Vertex Shader");
            addShader(name, ShaderType.FragmentShader, "fs", "Fragment Shader");
            addShader(name, ShaderType.GeometryShader, "gs", "Geometry Shader");
            addShader(name, ShaderType.TessControlShader, "tc", "Tesselation Control Shader");
            addShader(name, ShaderType.TessEvaluationShader, "te", "Tesselation Evaluation Shader");
            addShader(name, ShaderType.ComputeShader, "cs", "Compute Shader");

            GL.LinkProgram(Program);
            GL.UseProgram(Program);
            GL.ValidateProgram(Program);

            SetUniformsLocations();
        }

        protected virtual void SetUniformsLocations()
        {
        }

        protected void SetDefaultUniformLocations()
        {
            ProjectionMatrix = GL.GetUniformLocation(Program, "uProjectionMatrix");
            ViewMatrix = GL.GetUniformLocation(Program, "uViewMatrix");
            CameraPosition = GL.GetUniformLocation(Program, "uCameraPosition");
            LightDirection = GL.GetUniformLocation(Program, "uLightDirection");
            LightAmbient = GL.GetUniformLocation(Program, "uLightAmbient");
            LightDiffuse = GL.GetUniformLocation(Program, "uLightDiffuse");
            LightSpecular = GL.GetUniformLocation(Program, "uLightSpecular");
        }

        protected void SetShadowUniformLocations()
        {
            shadowMatrix = GL.GetUniformLocation(Program, $"uShadowMatrix");
            shadowMap = GL.GetUniformLocation(Program, $"uShadowMap");
            enableShadows = GL.GetUniformLocation(Program, "uEnableShadows");
        }

        private void checkCompileStatus(string shaderName, int shader)
        {
            int compileStatus;

            GL.GetShader(shader, ShaderParameter.CompileStatus, out compileStatus);
            if (compileStatus != 1)
                throw new Exception($"Filed to Compiler {shaderName}: {GL.GetShaderInfoLog(shader)}");
        }

        public void ApplyCamera(Camera camera)
        {
            GL.UniformMatrix4(ViewMatrix, false, ref camera.ViewMatrix);
            GL.UniformMatrix4(ProjectionMatrix, false, ref camera.ProjectionMatrix);
            GL.Uniform3(CameraPosition, camera.Position);
        }

        public void ApplyLight(Light light)
        {
            GL.Uniform3(LightDirection, light.Direction);
            GL.Uniform3(LightAmbient, light.Ambient);
            GL.Uniform3(LightDiffuse, light.Diffuse);
            GL.Uniform3(LightSpecular, light.Specular);
        }

        public void ApplyShadows(ShadowBox shadows)
        {
            if (shadows != null) {
                GL.Uniform1(enableShadows, 1);

                GL.ActiveTexture(TextureUnit.Texture9);
                GL.BindTexture(TextureTarget.Texture2D, shadows.ShadowBuffer.DepthTexture);
                GL.Uniform1(shadowMap, 9);
                GL.UniformMatrix4(shadowMatrix, false, ref shadows.ShadowMatrix);
            } else {
                GL.Uniform1(enableShadows, 0);
            }
        }
    }
}