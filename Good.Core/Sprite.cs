using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Good.Core
{
    public class Sprite
    {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle Source { get; set; }
        public Color Color { get; set; } = Color.White;
        public SpriteEffects FlipFlags { get; set; } 
        public int Priority { get; set; } = 0;
        public List<Behavior> Behaviors { get; set; } = new List<Behavior>();
    }
}
