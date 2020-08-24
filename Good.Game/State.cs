using Good.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Good.Game
{
    public class State : MainGameState
    {
        public override bool IsTranscendent => true;
        public override bool IsTransparent => true;

        public override void Draw()
        {
            MainGame.Instance.Renderer.DrawRectangle(new Rectangle(50, 10, 50, 50), Color.White);

            if (Keyboard.GetState().IsKeyDown(Keys.F))
                MainGame.Instance.ToggleFullscreen();
        }
    }
}
