using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Larx.Buffers
{
    public class Framebuffer
    {
        private readonly int framebuffer;
        private int depthBuffer;
        private int colorBuffer;

        public int ColorTexture;
        public int DepthTexture;
        public Size Size { get; set; }
        private readonly FramebufferRenderer framebufferRenderer;

        public Framebuffer(Size size, bool useColorBuffer = true, bool useDepthBuffer = true)
        {
            this.Size = size;
            framebufferRenderer = new FramebufferRenderer();

            GL.Enable(EnableCap.Multisample);

            framebuffer = GL.GenFramebuffer();

            GL.BindFramebuffer(FramebufferTarget.FramebufferExt, framebuffer);
            if (useColorBuffer) buildColorBuffer();
            if (useDepthBuffer) buildDepthBuffer();
        }

        private void buildColorBuffer()
        {
            colorBuffer = GL.GenRenderbuffer();
            ColorTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, ColorTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Size.Width, Size.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.ColorAttachment0Ext, TextureTarget.Texture2D, ColorTexture, 0);
        }

        private void buildDepthBuffer()
        {
            depthBuffer = GL.GenRenderbuffer();
            DepthTexture = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, DepthTexture);
            GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)All.DepthComponent32, Size.Width, Size.Height, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);
            GL.FramebufferTexture2D(FramebufferTarget.FramebufferExt, FramebufferAttachment.DepthAttachmentExt, TextureTarget.Texture2D, DepthTexture, 0);
        }

        public void RefreshBuffers()
        {
            if (ColorTexture > 0) {
                GL.BindTexture(TextureTarget.Texture2D, ColorTexture);
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, Size.Width, Size.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, IntPtr.Zero);
            }

            if (DepthTexture > 0) {
                GL.BindTexture(TextureTarget.Texture2D, DepthTexture);
                GL.TexImage2D(TextureTarget.Texture2D, 0, (PixelInternalFormat)All.DepthComponent32, Size.Width, Size.Height, 0, PixelFormat.DepthComponent, PixelType.UnsignedInt, IntPtr.Zero);
            }

            framebufferRenderer.UpdateMatrix();
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, framebuffer);
            GL.Viewport(0, 0, Size.Width, Size.Height);
        }

        public void DrawColorBuffer()
        {
            framebufferRenderer.Render(ColorTexture);
        }

        public void DrawDepthBuffer()
        {
            framebufferRenderer.Render(DepthTexture);
        }
    }
}