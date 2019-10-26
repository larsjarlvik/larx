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

        private string prepareShader(string name)
        {
            var contents = File.ReadLines(Path.Combine("shaders", name));
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

            return finalShader.ToString();
        }

        public Shader(string name)
        {
            var vertexShader = GL.CreateShader(ShaderType.VertexShader);
            var fragmentShader = GL.CreateShader(ShaderType.FragmentShader);

            GL.ShaderSource(vertexShader, prepareShader($"{name}.vs.glsl"));
            GL.CompileShader(vertexShader);
            checkCompileStatus($"Vertex Shader: {name}", vertexShader);

            GL.ShaderSource(fragmentShader, prepareShader($"{name}.fs.glsl"));
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
            CameraPosition = GL.GetUniformLocation(Program, "uCameraPosition");
            LightDirection = GL.GetUniformLocation(Program, "uLightDirection");
            LightAmbient = GL.GetUniformLocation(Program, "uLightAmbient");
            LightDiffuse = GL.GetUniformLocation(Program, "uLightDiffuse");
            LightSpecular = GL.GetUniformLocation(Program, "uLightSpecular");
        }

        protected void SetShadowUniformLocations()
        {
            shadowMap = GL.GetUniformLocation(Program, "uShadowMap");
            shadowMatrix = GL.GetUniformLocation(Program, "uShadowMatrix");
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

        public void ApplyShadows(ShadowRenderer shadows)
        {
            if (shadows != null) {
                GL.ActiveTexture(TextureUnit.Texture9);
                GL.BindTexture(TextureTarget.Texture2D, shadows.ShadowBuffer.DepthTexture);
                GL.Uniform1(shadowMap, 9);
                GL.Uniform1(enableShadows, 1);
                GL.UniformMatrix4(shadowMatrix, false, ref shadows.ShadowMatrix);
            } else {
                GL.Uniform1(enableShadows, 0);
            }
        }
    }
}