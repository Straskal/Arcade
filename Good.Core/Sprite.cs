using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Good.Core
{
    public struct SpriteDrawInfo 
    {
        public Texture2D Texture { get; set; }
        public Rectangle Source { get; set; }
        public Color Color { get; set; } = Color.White;
        public SpriteEffects FlipFlags { get; set; } = SpriteEffects.None;
    }

    public class Sprite
    {
        public Vector2 Position { get; set; }
        public Vector2 PreviousPosition { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle Source { get; set; }
        public Color Color { get; set; } = Color.White;
        public SpriteEffects FlipFlags { get; set; } = SpriteEffects.None;
        public int Priority { get; set; } = 0;
        public bool IsSolid { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Rectangle Bounds => new Rectangle((int)Math.Floor(Position.X), (int)Math.Floor(Position.Y), Width, Height);
        public Rectangle PreviousBounds => new Rectangle((int)Math.Floor(PreviousPosition.X), (int)Math.Floor(PreviousPosition.Y), Width, Height);
        public List<Behavior> Behaviors { get; set; } = new List<Behavior>();
        public Dictionary<string, Animation> Animations { get; set; } = new Dictionary<string, Animation>();
        public string CurrentAnimation { get; set; }
        public int CurrentFrame { get; set; }
        public float AnimationTimer { get; set; }

        internal void UpdateBehaviors() => Behaviors.ForEach(behavior => behavior.Update(this));
        internal void Draw() => Renderer.Instance.Draw(Texture, Source, Position, Color, FlipFlags);

        public bool MoveAndCollideX(float x)
        {
            if (Layout.Current.Grid.IsCollidingAtOffset(this, x, 0f, out var overlap))
            {
                Position += new Vector2(x + overlap.Depth.X, 0f);
                return true;
            }
            Position += new Vector2(x, 0f);
            return false;
        }

        public bool MoveAndCollideY(float y)
        {
            if (Layout.Current.Grid.IsCollidingAtOffset(this, 0f, y, out var overlap))
            {
                Position += new Vector2(0f, y + overlap.Depth.Y);
                return true;
            }
            Position += new Vector2(0f, y);
            return false;
        }

        public bool MoveAndCollide(Vector2 direction, out Overlap overlap)
        {
            overlap = new Overlap();
            bool result = false;
            direction = Vector2.Round(direction);

            if (direction == Vector2.Zero)
                return result;

            if (Layout.Current.Grid.IsCollidingAtOffset(this, direction.X, 0f, out var overlapX))
            {
                direction.X += overlapX.Depth.X;
                overlap = overlapX;
                result = true;
            }

            if (Layout.Current.Grid.IsCollidingAtOffset(this, 0f, direction.Y, out var overlapY))
            {
                direction.Y += overlapY.Depth.Y;

                if (!result)
                    overlap = overlapY;

                result = true;
            }
            Position += direction;
            Layout.Current.Grid.Update(this);
            return result;
        }

        public bool MoveAndOverlap(Vector2 direction, out Overlap overlap)
        {
            Position += direction;
            return Layout.Current.Grid.IsOverlapping(this, out overlap);
        }
    }

    public class Animation
    {
        public Rectangle[] Frames { get; set; }
        public float Speed { get; set; }
    }

    public static class SpriteAnimation 
    {
        public static void Animate(this Sprite sprite) 
        {
            var anim = sprite.Animations[sprite.CurrentAnimation];
            if (anim.Speed > 0f && MainGame.Time.TotalGameTime.TotalSeconds > sprite.AnimationTimer) 
            {
                if (++sprite.CurrentFrame > anim.Frames.Length - 1) 
                    sprite.CurrentFrame = 0;

                sprite.Source = anim.Frames[sprite.CurrentFrame];
                sprite.AnimationTimer += anim.Speed;
            }            
        }

        public static void SetAnimation(this Sprite sprite, string name) 
        {
            if (sprite.Animations.TryGetValue(name, out var animation) && name != sprite.CurrentAnimation) 
            {
                sprite.CurrentAnimation = name;
                sprite.CurrentFrame = 0;
                sprite.Source = animation.Frames[0];
                sprite.AnimationTimer = (float)MainGame.Time.TotalGameTime.TotalSeconds + animation.Speed;
            }
        }
    }
}
