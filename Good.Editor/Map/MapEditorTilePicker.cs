using Good.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Good.Editor
{
    public class MapEditorTilePicker : MainGameState
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

        private int previousScroll = 0;

        private bool down = false;

        public int SelectedTile = 0;

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

                    if (i == SelectedTile)
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
                    SelectedTile++;
                else if (mouse.ScrollWheelValue > previousScroll)
                    SelectedTile--;

                SelectedTile = MathHelper.Clamp(SelectedTile, 0, max);
            }

            previousScroll = mouse.ScrollWheelValue;
        }
    }
}
