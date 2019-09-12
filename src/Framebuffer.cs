using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx
{
    public class Framebuffer
    {
        private readonly int samples;
        private readonly int framebuffer;
        private readonly int depthBuffer;
        private readonly int colorBuffer;

        public readonly int ColorTexture;
        public readonly int DepthTexture;
        public Size Size { get; set; }

        public Framebuffer(int samples, Size size)
        {
            this.samples = samples;
            this.Size = size;

            GL.Enable(EnableCap.Multisample);

            framebuffer = GL.GenFramebuffer();
            ColorTexture = GL.GenTexture();
            DepthTexture = GL.GenTexture();
            colorBuffer = GL.GenRenderbuffer();
            depthBuffer = GL.GenRenderbuffer();

            buildBuffers();
        }

        private void buildBuffers()
        {
            GL.BindTexture(TextureTarget.Texture2D, ColorTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Size.Width, Size.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

            GL.BindTexture(TextureTarget.Texture2D, DepthTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)All.DepthComponent32, Size.Width, Size.Height, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, framebuffer);
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, ColorTexture, 0);
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, TextureTarget.Texture2D, DepthTexture, 0);
        }

        public void RefreshBuffers()
        {
            GL.BindTexture(TextureTarget.Texture2D, ColorTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Size.Width, Size.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);

            GL.BindTexture(TextureTarget.Texture2D, DepthTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)All.DepthComponent32, Size.Width, Size.Height, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
        }

        public void Draw()
        {
            Draw(new Point(0, 0), Size);
        }

        public void Draw(Point position, Size size)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.BindTexture(TextureTarget.Texture2D, ColorTexture);
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, framebuffer);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.BlitFramebuffer(
                0, 0, Size.Width, Size.Height,
                position.X, position.Y, position.X + size.Width, position.Y + size.Height,
                ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest
            );

            GL.Enable(EnableCap.DepthTest);
        }

        public void Copy(Size size)
        {
            GL.CopyTexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba8, 0, 0, size.Width, size.Height, 0);
        }
    }
}