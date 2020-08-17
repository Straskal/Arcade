using Microsoft.Xna.Framework.Graphics;

namespace Good.Core
{
    public class Tileset 
    {
        public Texture2D Texture { get; set; }
    }

    public class Tilemap
    {
        public Tileset Tileset { get; set; }
        public int[,] Data { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
