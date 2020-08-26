using Good.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace Good.Editor
{
    public class MapEditor : MainGameState
    {
        public override bool IsTranscendent => true;
        public override bool IsTransparent => true;

        const int TileSize = 16;
        const int TilesPerRow = 4;
        const int TileSpaceBuffer = 5;
        const int PanelWidth = TileSize * TilesPerRow + TileSpaceBuffer;
        const int PanelX = Renderer.ResolutionWidth - PanelWidth;
        const int PanelHeight = Renderer.ResolutionHeight;

        private SpriteFont font;

        private int selectedTile = 0;
        private int previousScroll = 0;

        private bool down = false;

        public override void Enter()
        {
            font = MainGame.Instance.Content.Load<SpriteFont>("Font/BasicFont");
        }

        public override void Update()
        {
            Layout.Current.PumpSpriteOperations();
        }

        public override void Draw()
        {
            var mouse = Mouse.GetState();
            var mousePosition = Renderer.Instance.TransformScreenCoords(new Vector2(mouse.X, mouse.Y));
            var level = Layout.Current;
            var map = level.Map;

            if (Keyboard.GetState().IsKeyDown(Keys.S) && !down)
            {
                down = true;
                LevelSaver.SaveLevel();
            }
            else down = false;

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Layout.Current.Position += new Vector2(1, 0);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Layout.Current.Position += new Vector2(-1, 0);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                Layout.Current.Zoom += 0.1f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Layout.Current.Zoom -= 0.1f;
            }

            // Grid
            {
                int mapHeight = map.Data.GetLength(0);
                int mapWidth = map.Data.GetLength(1);

                for (int i = 0; i < mapHeight; i++)
                    Renderer.Instance.DrawRectangle(0, i * TileSize, mapWidth * TileSize, 1, new Color(50, 50, 50, 50));

                for (int i = 0; i < mapWidth; i++)
                    Renderer.Instance.DrawRectangle(i * TileSize, 0, 1, mapHeight * TileSize, new Color(50, 50, 50, 50));
            }

            // Panel
            Renderer.Instance.DrawRectangle(new Rectangle(PanelX, 0, PanelWidth, PanelHeight), new Color(Color.Black, 0.9f));

            // Tiles
            {
                var numRows = map.Tileset.Texture.Height / 16;
                var numColumns = map.Tileset.Texture.Width / 16;
                var numCells = numRows * numColumns;
                var x = PanelX + 1;
                var y = 1;

                var selectedColor = new Color(255, 0, 0, 20);

                for (var i = 0; i < numCells; i++)
                {
                    var position = new Vector2(x, y);
                    var source = map.Tileset.GetIndexSource(i);
                    Renderer.Instance.Draw(map.Tileset.Texture, source, position, Color.White);
                    x += TileSize + 1;

                    if (i == selectedTile)
                        Renderer.Instance.DrawRectangle(new Rectangle((int)position.X, (int)position.Y, TileSize, TileSize), selectedColor);

                    if ((i + 1) % 4 == 0)
                    {
                        x = PanelX + 1;
                        y += TileSize + 1;
                    }
                }
            }

            // Select tile on mouse scroll
            {
                var max = map.Tileset.Texture.Width * map.Tileset.Texture.Height;
                max = max / (TileSize * TileSize) - 1;

                if (mouse.ScrollWheelValue < previousScroll)
                    selectedTile++;
                else if (mouse.ScrollWheelValue > previousScroll)
                    selectedTile--;

                selectedTile = MathHelper.Clamp(selectedTile, 0, max);
            }

            // Paint the selected tile
            {
                if (mousePosition.X < PanelX) 
                {
                    int mapWidth = level.Map.Data.GetLength(1);
                    int mapHeight = level.Map.Data.GetLength(0);
                    int x = (int)Math.Floor(mousePosition.X / TileSize);
                    int y = (int)Math.Floor(mousePosition.Y / TileSize);

                    if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                    {
                        if (mouse.RightButton == ButtonState.Pressed)
                            level.Map.Data[y, x] = -1;
                        else if (mouse.LeftButton == ButtonState.Pressed)
                            level.Map.Data[y, x] = selectedTile;
                        else
                        {
                            Rectangle tileSource = level.Map.Tileset.GetIndexSource(selectedTile);
                            mousePosition.X = (float)Math.Floor(mousePosition.X / 16) * 16;
                            mousePosition.Y = (float)Math.Floor(mousePosition.Y / 16) * 16;
                            Renderer.Instance.Draw(level.Map.Tileset.Texture, tileSource, mousePosition, new Color(200, 200, 200, 200));
                        }
                    }
                }
            }

            var hovered = Layout.Current.Grid.QueryPoint(mousePosition.ToPoint()).FirstOrDefault();
            if (hovered != null) 
            {
                hovered.DrawInfo.Color = Color.Pink;
                Renderer.Instance.Print(font, mousePosition, hovered.BodyInfo.Position.ToString());
            }

            previousScroll = mouse.ScrollWheelValue;
        }
    }
}
