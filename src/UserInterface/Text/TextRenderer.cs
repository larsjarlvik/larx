using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Larx.UserInterface.Text
{
    public struct DisplayText
    {
        public int VaoId { get; }
        public int NumItems { get; }
        public Vector2 Size { get; }

        public DisplayText(int vaoId, int numItems, Vector2 size)
        {
            VaoId = vaoId;
            NumItems = numItems;
            Size = size;
        }
    }

    public class TextRenderer
    {
        private Texture texture;
        private FontData fontData;
        public TextShader Shader { get; }

        public TextRenderer()
        {
            Shader = new TextShader();
            texture = new Texture();
            texture.LoadTexture(Path.Combine("resources", "OpenSans-Regular.png"));
            fontData = JsonConvert.DeserializeObject<FontData>(File.ReadAllText(Path.Combine("resources", "OpenSans-Regular.json")));
        }

        public void Render(DisplayText text, Matrix4 pMatrix, Vector2 position, float buffer, float gamma)
        {
            Render(text, pMatrix, position,buffer, gamma, new Color4(1f, 1f, 1f, 1f));
        }

        public void Render(DisplayText text, Matrix4 pMatrix, Vector2 position, float buffer, float gamma, Color4 color)
        {
            GL.DepthMask(false);
            GL.UseProgram(Shader.Program);

            GL.UniformMatrix4(Shader.Matrix, false, ref pMatrix);
            GL.Uniform2(Shader.Position, new Vector2(position.X, position.Y));

            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texture.TextureId);
            GL.Uniform1(Shader.Texture, 0);
            GL.Uniform2(Shader.TextSize, new Vector2(texture.Size.X, texture.Size.Y));

            GL.Uniform4(Shader.Color, color);
            GL.Uniform1(Shader.Buffer, buffer);

            GL.BindVertexArray(text.VaoId);
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            GL.DrawArrays(PrimitiveType.Triangles, 0, text.NumItems);

            GL.Uniform1(Shader.Buffer, 192f / 256f);
            GL.Uniform1(Shader.Gamma, gamma * 1.4142f / 14.0f);

            GL.DrawArrays(PrimitiveType.Triangles, 0, text.NumItems);
            GL.DepthMask(true);
            GL.BindVertexArray(0);
        }

        private Vector2 drawGlyph(char chr, Vector2 pen, float size, List<Vector2> vertexElements, List<Vector2> textureElements)
        {
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

        private float getWordLength(string word, float size)
        {
            var totalAdvance = 0.0f;
            var scale = size / fontData.Size;

            foreach(var chr in word) {
                var metric = fontData.Chars[chr];
                totalAdvance += metric[4] * scale;
            }

            return totalAdvance;
        }

        private string getRemainingLettersInWord(int index, string text)
        {
            if (text[index] == ' ') return "";
            var remaining = text.Substring(index);
            var nextSpace = remaining.IndexOf(' ');

            if (nextSpace == -1) return remaining;

            return remaining.Substring(0, nextSpace);
        }

        public DisplayText CreateText(string text, float size, float maxWidth = float.MaxValue)
        {
            var vertexElements = new List<Vector2>();
            var textureElements = new List<Vector2>();

            var pen = new Vector2(0, 0);

            for (var i = 0; i < text.Length; i++) {
                if (pen.X + getWordLength(getRemainingLettersInWord(i, text), size) > maxWidth) {
                    pen.X = 0.0f;
                    pen.Y += size * 1.3f;
                }

                var chr = text[i];
                pen = drawGlyph(chr, pen, size, vertexElements, textureElements);
            }

            var numItems = vertexElements.Count;
            var vaoId = GL.GenVertexArray();
            var vertexBuffer = GL.GenBuffer();
            var textureBuffer = GL.GenBuffer();

            GL.BindVertexArray(vaoId);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBuffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, Vector2.SizeInBytes * numItems, vertexElements.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);

            GL.BindBuffer(BufferTarget.ArrayBuffer, textureBuffer);
            GL.BufferData<Vector2>(BufferTarget.ArrayBuffer, Vector2.SizeInBytes * numItems, textureElements.ToArray(), BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0);
            GL.BindVertexArray(0);

            pen.Y += size * 1.3f;
            return new DisplayText(vaoId, numItems, pen);
        }
    }
}
