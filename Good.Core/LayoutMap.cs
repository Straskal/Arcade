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

    public class LayoutMap
    {
        public const int TileSize = 16;

        public Tileset Tileset { get; set; }
        public int[,] Data { get; set; }
        public int HorizontalCells => Data.GetLength(1);
        public int VerticalCells => Data.GetLength(0);
        public int Width => Data.GetLength(1) * TileSize;
        public int Height => Data.GetLength(0) * TileSize;

        internal void Draw()
        {
            int height = Data.GetLength(0);
            int width = Data.GetLength(1);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int index = Data[i, j];
                    if (index != -1)
                    {
                        var source = Tileset.GetIndexSource(index);
                        var position = new Vector2(j * 16, i * 16);
                        Renderer.Instance.Draw(Tileset.Texture, source, position, Color.White);
                    }
                }
            }
        }
    }
}
