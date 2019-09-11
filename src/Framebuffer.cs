using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx
{
    public class Framebuffer
    {
        private int samples;

        private int texture;
        private int framebuffer;
        private int depthBuffer;
        private int colorBuffer;

        public Size Size { get; set; }

        public Framebuffer(int samples, Size size)
        {
            this.samples = samples;
            this.Size = size;

            GL.Enable(EnableCap.Multisample);

            framebuffer = GL.GenFramebuffer();
            texture = GL.GenTexture();
            colorBuffer = GL.GenRenderbuffer();
            depthBuffer = GL.GenRenderbuffer();
        }

        private void createFramebuffer()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
        }

        private void createTexture()
        {
            if (samples > 0) {
                GL.BindTexture(TextureTarget.Texture2DMultisample, texture);
                GL.TexImage2DMultisample(TextureTargetMultisample.Texture2DMultisample, samples, PixelInternalFormat.Rgb, Size.Width, Size.Height, true);
                GL.BindTexture(TextureTarget.Texture2DMultisample, 0);
            } else {
                GL.BindTexture(TextureTarget.Texture2D, texture);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb, Size.Width, Size.Height, 0, PixelFormat.Bgra, PixelType.Float, IntPtr.Zero);
                GL.BindTexture(TextureTarget.Texture2DMultisample, 0);
            }
        }

        private void createColorBuffer()
        {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, colorBuffer);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, samples, RenderbufferStorage.Rgba8, Size.Width, Size.Height);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, RenderbufferTarget.Renderbuffer, colorBuffer);
        }

        private void createDepthBuffer()
        {
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthBuffer);
            GL.RenderbufferStorageMultisample(RenderbufferTarget.Renderbuffer, samples, RenderbufferStorage.DepthComponent, Size.Width, Size.Height);
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, RenderbufferTarget.Renderbuffer, depthBuffer);
        }

        public void RefreshBuffers()
        {
            createTexture();
            createFramebuffer();
            createColorBuffer();
            createDepthBuffer();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Bind()
        {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
        }

        public void Draw()
        {
            Draw(new Point(0, 0), Size);
        }

        public void Draw(Point position, Size size)
        {
            GL.Disable(EnableCap.DepthTest);
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, framebuffer);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, 0);
            GL.BlitFramebuffer(
                0, 0, Size.Width, Size.Height,
                position.X, position.Y, position.X + size.Width, position.Y + size.Height,
                ClearBufferMask.ColorBufferBit, BlitFramebufferFilter.Nearest
            );
            GL.Enable(EnableCap.DepthTest);
        }
    }
}