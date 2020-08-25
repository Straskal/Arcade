using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Good.Core
{
    public class SpriteDrawInfo 
    {
        public Texture2D Texture { get; set; }
        public Rectangle Source { get; set; }
        public Color Color { get; set; } = Color.White;
        public SpriteEffects FlipFlags { get; set; }
        public int Priority { get; set; }
    }

    public class SpriteBodyInfo
    {
        public Vector2 Position { get; set; }
        public Vector2 PreviousPosition { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsSolid { get; set; }
        public Rectangle Bounds => new Rectangle((int)Math.Floor(Position.X), (int)Math.Floor(Position.Y), Width, Height);
        public Rectangle PreviousBounds => new Rectangle((int)Math.Floor(PreviousPosition.X), (int)Math.Floor(PreviousPosition.Y), Width, Height);
    }

    public class SpriteAnimationInfo
    {
        public class Animation
        {
            public Rectangle[] Frames { get; set; }
            public float Speed { get; set; }
        }

        public Dictionary<string, Animation> Animations { get; set; } = new Dictionary<string, Animation>();
        public string CurrentAnimationName { get; set; }
        public int CurrentFrame { get; set; }
        public float FrameTimer { get; set; }
        public Animation CurrentAnimation => Animations[CurrentAnimationName];
    }

    public class Sprite
    {
        public SpriteBodyInfo BodyInfo { get; set; } = new SpriteBodyInfo();
        public SpriteDrawInfo DrawInfo { get; set; } = new SpriteDrawInfo();
        public SpriteAnimationInfo AnimationInfo { get; set; } = new SpriteAnimationInfo();
        public List<Behavior> Behaviors { get; set; } = new List<Behavior>();

        internal void UpdateBehaviors() => Behaviors.ForEach(behavior => behavior.Update(this));
        internal void Draw() => Renderer.Instance.Draw(DrawInfo.Texture, DrawInfo.Source, BodyInfo.Position, DrawInfo.Color, DrawInfo.FlipFlags);

        public void Animate()
        {
            var currentAnimation = AnimationInfo.CurrentAnimation;
            var elapsedTime = (float)MainGame.Time.ElapsedGameTime.TotalSeconds;

            if ((AnimationInfo.FrameTimer += elapsedTime * currentAnimation.Speed) > 1f)
            {
                if (++AnimationInfo.CurrentFrame > currentAnimation.Frames.Length - 1)
                    AnimationInfo.CurrentFrame = 0;

                DrawInfo.Source = currentAnimation.Frames[AnimationInfo.CurrentFrame];
                AnimationInfo.FrameTimer = 0f;
            }
        }

        public void SetAnimation(string name)
        {
            if (AnimationInfo.Animations.TryGetValue(name, out var animation) && name != AnimationInfo.CurrentAnimationName)
            {
                AnimationInfo.CurrentAnimationName = name;
                AnimationInfo.CurrentFrame = 0;
                DrawInfo.Source = animation.Frames[0];
                AnimationInfo.FrameTimer = 0f;
            }
        }

        public bool MoveAndCollide(Vector2 direction, out Overlap overlap)
        {
            bool result = false;
            overlap = new Overlap();
            direction = Vector2.Round(direction);

            if (direction == Vector2.Zero)
                return result;

            if (Layout.Current.Grid.IsCollidingAtOffset(this, direction.X, 0f, out var overlapX))
            {
                direction.X += overlapX.Depth.X;
                overlap = overlapX;
                result = true;
            }
            else if (Layout.Current.Grid.IsCollidingAtOffset(this, 0f, direction.Y, out var overlapY))
            {
                direction.Y += overlapY.Depth.Y;
                overlap = overlapY;
                result = true;
            }

            BodyInfo.Position += direction;
            Layout.Current.Grid.Update(this);
            return result;
        }
    }
}
