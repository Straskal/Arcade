using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Good.Core
{
    public class Renderer
    {
        public static Renderer Instance { get; private set; }

        public const int ResolutionWidth = 320;
        public const int ResolutionHeight = 224;
        public const float AspectRatio = ResolutionWidth / (float)ResolutionHeight;

        private readonly GraphicsDevice graphics;
        private readonly SpriteBatch batch;
        private readonly Texture2D texture;
        private Effect currentEffect;
        private Matrix transformation;

        internal Renderer(GraphicsDevice graphicsDevice)
        {
            Instance = this;
            graphics = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            batch = new SpriteBatch(graphics);
            texture = new Texture2D(graphics, 1, 1, false, SurfaceFormat.Color);
            texture.SetData(new[] { Color.White });
        }

        internal void BeginDraw()
        {
            currentEffect = null;

            int screenWidth = graphics.PresentationParameters.BackBufferWidth;
            int screenHeight = graphics.PresentationParameters.BackBufferHeight;

            // Set the view port to be as wide as the backbuffer and then clear the screen.
            graphics.Viewport = new Viewport
            {
                X = 0,
                Y = 0,
                Width = screenWidth,
                Height = screenHeight
            };

            graphics.Clear(Color.Black);

            // Time to create those black aspect ratio bars.
            // Start off the process of assuming letter box is needed.
            int targetWidth = screenWidth;
            int targetHeight = (int)(screenWidth / AspectRatio + 0.5f);

            // Adjust from letter box to pillar pox if need be.
            if (targetHeight > screenHeight)
            {
                targetHeight = screenHeight;
                targetWidth = (int)(targetHeight * AspectRatio + 0.5f);
            }

            graphics.Viewport = new Viewport
            {
                X = screenWidth / 2 - targetWidth / 2,
                Y = screenHeight / 2 - targetHeight / 2,
                Width = targetWidth,
                Height = targetHeight
            };

            // Create the resolution scale transformation.
            transformation = 
                Matrix.CreateScale(new Vector3
                {
                    X = (float)targetWidth / ResolutionWidth,
                    Y = (float)targetWidth / ResolutionWidth,
                    Z = 1f
                });

            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default,
                       RasterizerState.CullCounterClockwise, currentEffect, transformation);
        }

        internal void EndDraw()
        {
            batch.End();
        }

        internal void Unload()
        {
            batch.Dispose();
        }

        public void Draw(Texture2D texture, Vector2 position, Color color)
        {
            HandleEffectChange(null);
            batch.Draw(texture, position, color);
        }

        public void Draw(Texture2D texture, Rectangle source, Vector2 position, Color color, SpriteEffects flipFlags = SpriteEffects.None, Effect effect = null)
        {
            HandleEffectChange(effect);
            batch.Draw(texture, position, source, color, 0, Vector2.Zero, Vector2.One, flipFlags, 0);
        }

        public void DrawRectangle(Rectangle rectangle, Color color)
        {
            HandleEffectChange(null);
            batch.Draw(texture, rectangle, null, color);
        }

        public void DrawRectangleLines(Rectangle rectangle, Color color)
        {
            HandleEffectChange(null);
            Rectangle temp = Rectangle.Empty;
            DrawLine(ref temp, rectangle.X, rectangle.Y, rectangle.Width, 1, color);
            DrawLine(ref temp, rectangle.X, rectangle.Y + rectangle.Height - 1, rectangle.Width, 1, color);
            DrawLine(ref temp, rectangle.X, rectangle.Y, 1, rectangle.Height, color);
            DrawLine(ref temp, rectangle.X + rectangle.Width - 1, rectangle.Y, 1, rectangle.Height, color);
        }

        private void DrawLine(ref Rectangle rect, int x0, int y0, int x1, int y1, Color color) 
        {
            rect.X = x0; rect.Width = x1;
            rect.Y = y0; rect.Height = y1;
            batch.Draw(texture, rect, null, color);
        }

        private void HandleEffectChange(Effect effect)
        {
            if (currentEffect != effect)
            {
                currentEffect = effect;
                batch.End();
                batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default,
                       RasterizerState.CullCounterClockwise, currentEffect, transformation);
            }
        }
    }
}
