using Good.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Good.Game
{
    public class PlayerBehavior : Behavior
    {
        public override void Update(Sprite sprite)
        {
            var keyboard = Keyboard.GetState();

            var direction = Vector2.Zero;
            var anim = sprite.AnimationInfo.CurrentAnimation;
            var speed = 0f;

            if (keyboard.IsKeyDown(Keys.D)) 
            {
                direction.X = 1;
                sprite.DrawInfo.FlipFlags = SpriteEffects.None;
                speed = 0.2f;
            }

            if (keyboard.IsKeyDown(Keys.A))
            {
                direction.X = -1;
                sprite.DrawInfo.FlipFlags = SpriteEffects.FlipHorizontally;
                speed = 0.2f;
            }

            if (keyboard.IsKeyDown(Keys.W))
            {
                anim = "walk_up";
                direction.Y = -1;
                speed = 0.2f;
            }

            if (keyboard.IsKeyDown(Keys.S))
            {
                anim = "walk_down";
                direction.Y = 1;
                speed = 0.2f;
            }

            if (direction.X != 0f)
                anim = "walk_right";

            if (direction.LengthSquared() > 1f)
                direction.Normalize();

            sprite.SetAnimation(anim);
            sprite.AnimationInfo.Animations[sprite.AnimationInfo.CurrentAnimation].Speed = speed;

            if (sprite.MoveAndCollide(direction, out var overlap))
            {

            }

            Layout.Current.MoveAndClamp(sprite.BodyInfo.Position - new Vector2((Renderer.ResolutionWidth/2) - sprite.BodyInfo.Width /2, Renderer.ResolutionHeight/2 - sprite.BodyInfo.Height / 2));
        }
    }
}
