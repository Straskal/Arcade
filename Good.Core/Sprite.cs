using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Good.Core
{
    public struct SpritePriorityComparer : IComparer<Sprite>
    {
        public int Compare(Sprite x, Sprite y)
        {
            return x.Priority > y.Priority ? 1 : 0;
        }
    }

    public class Animation
    {
        public Rectangle[] Frames { get; set; }
        public int Speed { get; set; }
    }

    public class AnimationList
    {
        public bool IsEnabled { get; set; }
        public Dictionary<string, Animation> Animations { get; set; } = new Dictionary<string, Animation>();
        public Animation CurrentAnimation { get; set; }
        public int CurrentFrame { get; set; }
        public int Timer { get; set; }

        public void SetAnimation(string name)
        {
            if (Animations.TryGetValue(name, out var animation) && animation != CurrentAnimation) 
            {
                CurrentAnimation = animation;
                CurrentFrame = 0;
                Timer = MainGame.Time.TotalGameTime.Seconds + animation.Speed;
            }
        }
    }

    public class Sprite
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle Source { get; set; }
        public Color Color { get; set; } = Color.White;
        public SpriteEffects FlipFlags { get; set; } 
        public int Priority { get; set; } = 0;
        public List<Behavior> Behaviors { get; set; } = new List<Behavior>();

        public Behavior Get<T>() where T : Behavior
        {
            return Behaviors.SingleOrDefault(behavior => behavior.GetType() == typeof(T));
        }
    }
}
