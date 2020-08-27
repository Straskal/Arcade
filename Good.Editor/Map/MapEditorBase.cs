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
        private Action currentMode;
        private Vector2 mousePosition;
        private PaintTileCommand paintCommand;

        public override void Enter()
        {
            editorBase = MainGame.Instance.GetState<EditorBase>() ?? throw new InvalidOperationException("The map editor must be stacked on top of the editor base.");
            currentMode = Emulate;

            MainGame.Instance.Push(tilePicker);
        }

        public override void Draw()
        {
            if (InputManager.IsMouseMiddleDown())
                Layout.Current.Position -= InputManager.GetMouseMotion();

            mousePosition = InputManager.GetMousePosition();

            int mapHeight = Layout.Current.Map.Data.GetLength(0);
            int mapWidth = Layout.Current.Map.Data.GetLength(1);

            for (int i = 0; i < mapHeight; i++)
                Renderer.Instance.DrawRectangle(0, i * LayoutMap.TileSize, mapWidth * LayoutMap.TileSize, 1, new Color(50, 50, 50, 50));

            for (int i = 0; i < mapWidth; i++)
                Renderer.Instance.DrawRectangle(i * LayoutMap.TileSize, 0, 1, mapHeight * LayoutMap.TileSize, new Color(50, 50, 50, 50));

            currentMode.Invoke();
        }

        private void Paint() 
        {
            if (InputManager.IsMouseLeftDown())
            {
                if (WithinBounds(out var x, out var y) && Layout.Current.Map.Data[y, x] != tilePicker.SelectedTile)
                    paintCommand.PaintTile(x, y, tilePicker.SelectedTile);
            }
            else 
            {
                PushPaintCommand();
                currentMode = Emulate;
            }
        }

        private void Erase() 
        {
            if (InputManager.IsMouseRightDown())
            {
                if (WithinBounds(out var x, out var y) && Layout.Current.Map.Data[y, x] != LayoutMap.EmptyTile)
                    paintCommand.PaintTile(x, y, LayoutMap.EmptyTile);
            }
            else
            {
                PushPaintCommand();
                currentMode = Emulate;
            }
        }

        private void Emulate()
        {
            if (WithinBounds(out var x, out var y))
            {
                var tileSource = Layout.Current.Map.Tileset.GetIndexSource(tilePicker.SelectedTile);
                mousePosition.X = (float)Math.Floor(mousePosition.X / LayoutMap.TileSize) * LayoutMap.TileSize;
                mousePosition.Y = (float)Math.Floor(mousePosition.Y / LayoutMap.TileSize) * LayoutMap.TileSize;
                Renderer.Instance.Draw(Layout.Current.Map.Tileset.Texture, tileSource, mousePosition, new Color(200, 200, 200, 200));
            }

            if (InputManager.WasMouseLeftPressed())
            {
                paintCommand = new PaintTileCommand(Layout.Current.Map.Data);
                currentMode = Paint;
            }
            else if (InputManager.WasMouseRightPressed()) 
            {
                paintCommand = new PaintTileCommand(Layout.Current.Map.Data);
                currentMode = Erase;
            }
        }

        private bool WithinBounds(out int x, out int y) 
        {
            mousePosition = InputManager.GetMousePosition();
            int mapWidth = Layout.Current.Map.Data.GetLength(1);
            int mapHeight = Layout.Current.Map.Data.GetLength(0);
            x = (int)Math.Floor(mousePosition.X / LayoutMap.TileSize);
            y = (int)Math.Floor(mousePosition.Y / LayoutMap.TileSize);
            return x >= 0 && x < mapWidth && y >= 0 && y < mapHeight;
        }

        private void PushPaintCommand() 
        {
            editorBase.Commands.Insert(paintCommand);
            paintCommand = null;
        }
    }
}
