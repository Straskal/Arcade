using Good.Core;
using Microsoft.Xna.Framework;

namespace Good.Editor
{
    public class TilePicker : MainGameState
    {
        private const int TileSize = 16;
        private const int TilesPerRow = 4;
        private const int TileSpaceBuffer = 5;
        private const int PanelWidth = TileSize * TilesPerRow + TileSpaceBuffer;
        private const int PanelX = Renderer.ResolutionWidth - PanelWidth;
        private const int PanelHeight = Renderer.ResolutionHeight;

        public override bool IsTranscendent => true;
        public override bool IsTransparent => true;
        public int SelectedTile { get; private set; } = 0;

        public override void Draw()
        {
            DrawPanel();
            DrawTiles();
            SelectTile();
        }

        private void DrawPanel() 
        {
            Renderer.Instance.DrawRectangle(new Rectangle(PanelX, 0, PanelWidth, PanelHeight), new Color(Color.Black, 0.9f));
        }

        private void DrawTiles() 
        {
            var numRows = Layout.Current.Map.Tileset.Texture.Height / 16;
            var numColumns = Layout.Current.Map.Tileset.Texture.Width / 16;
            var numCells = numRows * numColumns;
            var x = PanelX + 1;
            var y = 1;

            var selectedColor = new Color(255, 0, 0, 20);

            for (var i = 0; i < numCells; i++)
            {
                var position = new Vector2(x, y);
                var source = Layout.Current.Map.Tileset.GetIndexSource(i);
                Renderer.Instance.Draw(Layout.Current.Map.Tileset.Texture, source, position, Color.White);
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

        private void SelectTile() 
        {
            var max = Layout.Current.Map.Tileset.Texture.Width * Layout.Current.Map.Tileset.Texture.Height;
            max = max / (TileSize * TileSize) - 1;

            if (InputManager.ScrollDown())
                SelectedTile++;
            else if (InputManager.ScrollUp())
                SelectedTile--;

            SelectedTile = MathHelper.Clamp(SelectedTile, 0, max);
        }
    }
}
