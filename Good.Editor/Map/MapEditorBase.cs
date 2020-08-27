using Good.Core;
using Good.Editor.Map.Commands;
using Microsoft.Xna.Framework;
using System;

namespace Good.Editor.Map
{
    public class MapEditorBase : MainGameState
    {
        public override bool IsTranscendent => true;
        public override bool IsTransparent => true;
        public override Matrix? Transformation => Layout.Current.Transformation;

        private readonly MapEditorTilePicker tilePicker = new MapEditorTilePicker();

        private EditorBase editorBase;

        public override void Enter()
        {
            editorBase = MainGame.Instance.GetState<EditorBase>() ?? throw new InvalidOperationException("The map editor must be stacked on top of the editor base.");
            MainGame.Instance.Push(tilePicker);
        }

        public override void Draw()
        {
            if (InputManager.IsMouseMiddleDown())
                Layout.Current.Position -= InputManager.GetMouseMotion();

            var mousePosition = InputManager.GetMousePosition();
            var map = Layout.Current.Map;

            {
                int mapHeight = map.Data.GetLength(0);
                int mapWidth = map.Data.GetLength(1);

                for (int i = 0; i < mapHeight; i++)
                    Renderer.Instance.DrawRectangle(0, i * LayoutMap.TileSize, mapWidth * LayoutMap.TileSize, 1, new Color(50, 50, 50, 50));

                for (int i = 0; i < mapWidth; i++)
                    Renderer.Instance.DrawRectangle(i * LayoutMap.TileSize, 0, 1, mapHeight * LayoutMap.TileSize, new Color(50, 50, 50, 50));
            }

            {
                int mapWidth = map.Data.GetLength(1);
                int mapHeight = map.Data.GetLength(0);
                int x = (int)Math.Floor(mousePosition.X / LayoutMap.TileSize);
                int y = (int)Math.Floor(mousePosition.Y / LayoutMap.TileSize);

                if (x >= 0 && x < mapWidth && y >= 0 && y < mapHeight)
                {
                    if (InputManager.IsMouseRightDown() && map.Data[y, x] != -1)
                        editorBase.CommandQueue.Insert(new PaintTileCommand(x, y, -1, map.Data));
                    else if (InputManager.IsMouseLeftDown() && map.Data[y, x] != tilePicker.SelectedTile)
                        editorBase.CommandQueue.Insert(new PaintTileCommand(x, y, tilePicker.SelectedTile, map.Data));
                    else
                    {
                        var tileSource = map.Tileset.GetIndexSource(tilePicker.SelectedTile);
                        mousePosition.X = (float)Math.Floor(mousePosition.X / LayoutMap.TileSize) * LayoutMap.TileSize;
                        mousePosition.Y = (float)Math.Floor(mousePosition.Y / LayoutMap.TileSize) * LayoutMap.TileSize;
                        Renderer.Instance.Draw(map.Tileset.Texture, tileSource, mousePosition, new Color(200, 200, 200, 200));
                    }
                }
            }
        }
    }
}
