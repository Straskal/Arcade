using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Good.Core
{
    public static class View
    {
        public const int ResolutionWidth = 320;
        public const int ResolutionHeight = 224;
        public const float AspectRatio = ResolutionWidth / (float)ResolutionHeight;

        internal static Matrix ResolutionTransform { get; private set; }
        internal static Matrix CurrentTransform { get; private set; }

        internal static void AdjustResolution() 
        {
            int screenWidth = MainGame.Instance.GraphicsDevice.PresentationParameters.BackBufferWidth;
            int screenHeight = MainGame.Instance.GraphicsDevice.PresentationParameters.BackBufferHeight;

            // Time to create those black aspect ratio bars.
            // Set the view port to be as wide as the backbuffer and then clear the screen.
            MainGame.Instance.GraphicsDevice.Viewport = new Viewport
            {
                X = 0,
                Y = 0,
                Width = screenWidth,
                Height = screenHeight
            };

            MainGame.Instance.GraphicsDevice.Clear(Color.Black);

            // Start off the process of assuming letter box is needed.
            int targetWidth = screenWidth;
            int targetHeight = (int)(screenWidth / AspectRatio + 0.5f);

            // Adjust from letter box to pillar pox if need be.
            if (targetHeight > screenHeight)
            {
                targetHeight = screenHeight;
                targetWidth = (int)(targetHeight * AspectRatio + 0.5f);
            }

            MainGame.Instance.GraphicsDevice.Viewport = new Viewport
            {
                X = screenWidth / 2 - targetWidth / 2,
                Y = screenHeight / 2 - targetHeight / 2,
                Width = targetWidth,
                Height = targetHeight
            };

            ResolutionTransform = Matrix.CreateScale(new Vector3
            {
                X = (float)targetWidth / ResolutionWidth,
                Y = (float)targetWidth / ResolutionWidth,
                Z = 1f
            });
        }

        internal static void UpdateTransformation(Matrix? matrix) 
        {
            CurrentTransform = matrix.HasValue ? matrix.Value * ResolutionTransform : ResolutionTransform;
        }

        internal static Vector2 TransformScreenCoords(Vector2 screenPosition)
        {
            screenPosition.X -= MainGame.Instance.GraphicsDevice.Viewport.X;
            screenPosition.Y -= MainGame.Instance.GraphicsDevice.Viewport.Y;

            return Vector2.Transform(screenPosition, Matrix.Invert(CurrentTransform));
        }
    }
}
