using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Larx.Text
{
    public class TextRenderer
    {
        private Texture texture;
        private FontData fontData;
        private int vertexBuffer;
        private int textureBuffer;
        private int numItems;
        private float size;

        public TextShader Shader { get; }

        public TextRenderer()
        {
            Shader = new TextShader();
            texture = new Texture();
            texture.LoadTexture(Path.Combine("resources", "OpenSans-Regular.bmp"));
            fontData = JsonConvert.DeserializeObject<FontData>(File.ReadAllText(Path.Combine("resources", "OpenSans-Regular.json")));

            vertexBuffer = GL.GenBuffer();
            textureBuffer = GL.GenBuffer();
        }

        public void Render(Matrix4 pMatrix, Vector2 position, float buffer, float gamma)
        {
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            GL.DepthMask(false);
            GL.UseProgram(Shader.Program);

            GL.UniformMatrix4(Shader.Matrix, false, ref pMatrix);
            GL.Uniform2(Shader.Position, position);

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture.TextureId);
            GL.Uniform1(Shader.Texture, 0);
            GL.Uniform2(Shader.TextSize, new Vector2(texture.Size.X, texture.Size.Y));

            GL.Uniform4(Shader.Color, new Color4(0.2f, 0.2f, 0.2f, 1f));
            GL.Uniform1(Shader.Buffer, buffer);

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, textureBuffer);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.DrawArrays(PrimitiveType.Triangles, 0, numItems);

            GL.Uniform4(Shader.Color, new Color4(1f, 1f, 1f, 1f));
            GL.Uniform1(Shader.Buffer, 192f / 256f);
            GL.Uniform1(Shader.Gamma, gamma * 1.4142f / size);

            GL.DrawArrays(PrimitiveType.Triangles, 0, numItems);
            GL.DepthMask(true);
        }

        private Vector2 drawGlyph(char chr, Vector2 pen, float size, List<Vector2> vertexElements, List<Vector2> textureElements) {
            this.size = size;

            var metric = fontData.Chars[chr];
            var scale = size / fontData.Size;

            var factor = 1;

            var width = metric[0];
            var height = metric[1];
            var horiBearingX = metric[2];
            var horiBearingY = metric[3];
            var horiAdvance = metric[4];
            var posX = metric[5];
            var posY = metric[6];

            if (width > 0 && height > 0) {
                width += fontData.Buffer * 2;
                height += fontData.Buffer * 2;

                vertexElements.AddRange(new Vector2[] {
                    new Vector2(factor * (pen.X + ((horiBearingX - fontData.Buffer + width) * scale)), factor * (pen.Y - horiBearingY * scale)),
                    new Vector2(factor * (pen.X + ((horiBearingX - fontData.Buffer) * scale)), factor * (pen.Y - horiBearingY * scale)),
                    new Vector2(factor * (pen.X + ((horiBearingX - fontData.Buffer) * scale)), factor * (pen.Y + (height - horiBearingY) * scale)),

                    new Vector2(factor * (pen.X + ((horiBearingX - fontData.Buffer + width) * scale)), factor * (pen.Y - horiBearingY * scale)),
                    new Vector2(factor * (pen.X + ((horiBearingX - fontData.Buffer) * scale)), factor * (pen.Y + (height - horiBearingY) * scale)),
                    new Vector2(factor * (pen.X + ((horiBearingX - fontData.Buffer + width) * scale)), factor * (pen.Y + (height - horiBearingY) * scale))
                });

                textureElements.AddRange(new [] {
                    new Vector2(posX + width, posY),
                    new Vector2(posX, posY),
                    new Vector2(posX, posY + height),

                    new Vector2(posX + width, posY),
                    new Vector2(posX, posY + height),
                    new Vector2(posX + width, posY + height)
                });
            }

            pen.X = pen.X + horiAdvance * scale;
            return pen;
        }

        public void CreateText(string text, float size) {
            var vertexArray = GL.GenVertexArray();
            GL.BindVertexArray(vertexArray);

            var vertexElements = new List<Vector2>();
            var textureElements = new List<Vector2>();

            var pen = new Vector2(0, 0);

            for (var i = 0; i < text.Length; i++) {
                var chr = text[i];
                pen = drawGlyph(chr, pen, size, vertexElements, textureElements);
            }

            numItems = vertexElements.Count;

            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, Vector2.SizeInBytes * numItems, vertexElements.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, textureBuffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, Vector2.SizeInBytes * numItems, textureElements.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Double, false, Vector2.SizeInBytes, 0);
        }
    }
}
