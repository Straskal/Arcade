using Good.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Good.Game
{
    public class PlayerBehavior : Behavior
    {
        public override void Update(Sprite sprite)
        {
            var keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.D))
                sprite.Position += new Vector2(1, 0);
        }
    }
}
