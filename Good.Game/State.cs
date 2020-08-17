using Good.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Good.Game
{
    public class State : GameState
    {
        public override bool UpdateBelow => true;
        public override bool DrawBelow => true;

        public override void Draw()
        {
            MainGame.Instance.Renderer.DrawRectangle(new Rectangle(50, 10, 50, 50), Color.White);

            if (Keyboard.GetState().IsKeyDown(Keys.F))
                MainGame.Instance.ToggleFullscreen();
        }
    }
}
