using Good.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace Good.Editor.Tilemap
{
    public class TilePicker
    {
        const int TileSize = 16;
        const int TilesPerRow = 4;
        const int TileSpaceBuffer = 5;
        const int PanelWidth = TileSize * TilesPerRow + TileSpaceBuffer;
        const int PanelX = Renderer.ResolutionWidth - PanelWidth;
        const int PanelHeight = Renderer.ResolutionHeight;

        private int selectedTile = 0;
        private int scroll = 0;

        public void Draw()
        {
            var mouse = Mouse.GetState();
            var mousePos = Renderer.Instance.ScaleScreenCoordinates(new Vector2(mouse.X, mouse.Y));

            // Panel
            Renderer.Instance.DrawRectangle(new Rectangle(PanelX, 0, PanelWidth, PanelHeight), new Color(Color.Black, 0.5f));

            // tiles
            {
                var level = LevelState.Current;
                var numRows = level.Map.Tileset.Texture.Height / 16;
                var numColumns = level.Map.Tileset.Texture.Width / 16;
                var numCells = numRows * numColumns;
                var x = PanelX + 1;
                var y = 1;

                for (var i = 0; i < numCells; i++)
                {
                    var position = new Vector2(x, y);
                    var source = level.Map.Tileset.GetIndexSource(i);
                    Renderer.Instance.Draw(level.Map.Tileset.Texture, source, position, Color.White);
                    x += TileSize + 1;

                    if (i == selectedTile)
                        Renderer.Instance.DrawRectangleLines(new Rectangle((int)position.X, (int)position.Y, TileSize, TileSize), Color.White);

                    if ((i + 1) % 4 == 0)
                    {
                        x = PanelX + 1;
                        y += TileSize + 1;
                    }
                }
            }

            // Select tile on mouse scroll
            {
                var max = LevelState.Current.Map.Tileset.Texture.Width * LevelState.Current.Map.Tileset.Texture.Height;
                max = max / (TileSize * TileSize) - 1;

                if (mouse.ScrollWheelValue < scroll)
                    selectedTile++;
                else if (mouse.ScrollWheelValue > scroll)
                    selectedTile--;

                selectedTile = MathHelper.Clamp(selectedTile, 0, max);
            }

            // Paint the selected tile
            {
                if (mousePos.X < PanelX) 
                {
                    int mapWidth = LevelState.Current.Map.Data.GetLength(1);
                    int mapHeight = LevelState.Current.Map.Data.GetLength(0);
                    int x = (int)Math.Floor(mousePos.X / TileSize);
                    int y = (int)Math.Floor(mousePos.Y / TileSize);

                    if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                    {
                        if (mouse.RightButton == ButtonState.Pressed)
                            LevelState.Current.Map.Data[y, x] = -1;
                        else if (mouse.LeftButton == ButtonState.Pressed)
                            LevelState.Current.Map.Data[y, x] = selectedTile;
                        else
                        {
                            var tileSource = LevelState.Current.Map.Tileset.GetIndexSource(selectedTile);
                            mousePos.X = (float)Math.Floor(mousePos.X / 16) * 16;
                            mousePos.Y = (float)Math.Floor(mousePos.Y / 16) * 16;
                            Renderer.Instance.Draw(LevelState.Current.Map.Tileset.Texture, tileSource, mousePos, new Color(200, 200, 200, 200));
                        }
                    }
                }
            }

            scroll = Mouse.GetState().ScrollWheelValue;
        }
    }
}
