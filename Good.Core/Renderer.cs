using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Good.Core
{
    public class Renderer
    {
        public static Renderer Instance { get; private set; }

        private readonly GraphicsDevice graphics;
        private readonly SpriteBatch batch;
        private readonly Texture2D texture;

        private Effect currentEffect;
        private Matrix currentTransformation;

        internal Renderer(GraphicsDevice graphicsDevice)
        {
            Instance = this;
            graphics = graphicsDevice ?? throw new ArgumentNullException(nameof(graphicsDevice));
            batch = new SpriteBatch(graphics);
            texture = new Texture2D(graphics, 1, 1, false, SurfaceFormat.Color);
            texture.SetData(new[] { Color.White });
        }

        internal void BeginDraw(Matrix transform)
        {
            currentEffect = null;
            currentTransformation = transform;

            batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default,
                       RasterizerState.CullCounterClockwise, currentEffect, currentTransformation);
        }

        internal void EndDraw()
        {
            batch.End();
        }

        internal void Unload()
        {
            batch.Dispose();
        }

        public void Print(SpriteFont font, Vector2 position, string text) 
        {
            HandleEffectChange(null);
            batch.DrawString(font, text, position, Color.White, 0, Vector2.Zero, 0.3f, SpriteEffects.None, 0);
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

        public void DrawRectangle(int x, int y, int w, int h, Color color)
        {
            HandleEffectChange(null);
            var rect = new Rectangle { X = x, Y = y, Width = w, Height = h };
            batch.Draw(texture, rect, null, color);
        }

        public void DrawRectangle(Rectangle rectangle, Color color)
        {
            HandleEffectChange(null);
            batch.Draw(texture, rectangle, null, color);
        }

        public void DrawRectangleLines(Rectangle rectangle, Color color)
        {
            HandleEffectChange(null);
            DrawRectangle(rectangle.X, rectangle.Y, rectangle.Width, 1, color);
            DrawRectangle(rectangle.X, rectangle.Y + rectangle.Height - 1, rectangle.Width, 1, color);
            DrawRectangle(rectangle.X, rectangle.Y, 1, rectangle.Height, color);
            DrawRectangle(rectangle.X + rectangle.Width - 1, rectangle.Y, 1, rectangle.Height, color);
        }

        private void HandleEffectChange(Effect effect)
        {
            if (currentEffect != effect)
            {
                currentEffect = effect;

                batch.End();
                batch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default,
                       RasterizerState.CullCounterClockwise, currentEffect, currentTransformation);
            }
        }
    }
}
