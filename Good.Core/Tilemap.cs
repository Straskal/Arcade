using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Good.Core
{
    public class Tileset 
    {
        public Texture2D Texture { get; set; }

        public Rectangle GetIndexSource(int i) 
        {
            int tilesetColumns = Texture.Width / 16;
            int row = (int)Math.Floor((double)i / tilesetColumns);
            int column = i % tilesetColumns;
            return new Rectangle(column * 16, row * 16, 16, 16);
        }
    }

    public class Tilemap
    {
        public Tileset Tileset { get; set; }
        public int[,] Data { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
