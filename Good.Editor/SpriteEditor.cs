using Good.Core;
using Good.Editor.Command;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace Good.Editor
{
    public class RemoveSpriteCommand : ICommand
    {
        private readonly Sprite sprite;

        public RemoveSpriteCommand(Sprite sprite) 
        {
            this.sprite = sprite;
        }

        public void Do()
        {
            Layout.Current.Remove(sprite);
        }

        public void Undo()
        {
            Layout.Current.Add(sprite);
        }
    }

    public class SpriteEditor : MainGameState
    {
        public override bool IsTranscendent => true;
        public override bool IsTransparent => true;

        private EditorBase editor;

        private SpriteFont font;
        private Sprite selected;
        private Sprite hovered;
        private Sprite lastHovered;
        private Vector2 mousePosition;

        public override void Enter()
        {
            editor = MainGame.Instance.GetState<EditorBase>();
            font = MainGame.Instance.Content.Load<SpriteFont>("Font/BasicFont");
        }

        public override void Update()
        {
            var mouse = Mouse.GetState();
            mousePosition = Renderer.Instance.ScaleScreenCoordinates(new Vector2(mouse.X, mouse.Y));

            hovered = Layout.Current.Grid.QueryPoint(mousePosition.ToPoint()).FirstOrDefault();
            if (hovered != null)
            {
                hovered.DrawInfo.Color = Color.Pink;
            }
            else if (lastHovered != null)
            {
                lastHovered.DrawInfo.Color = Color.White;
            }

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                if (selected == null)
                    selected = hovered;
            }
            else
                selected = null;

            if (selected != null) 
            {
                selected.BodyInfo.Position = mousePosition - new Vector2(8, 12);
                Layout.Current.Grid.Update(selected);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Delete) && hovered != null) 
            {
                editor.CommandQueue.Enqueue(new RemoveSpriteCommand(hovered));
                selected = null;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.C))
            {
                Layout.Current.Spawn("player", (int)mousePosition.X, (int)mousePosition.Y);
            }

            Layout.Current.PumpSpriteOperations();

            lastHovered = hovered;
        }

        public override void Draw()
        {
            //if (hovered != null)
            //    Renderer.Instance.Print(font, mousePosition, hovered.BodyInfo.Position.ToString());

            //if (Layout.Current.Grid.TryGetCellPosition(mousePosition, out var col, out var row))
            //    Renderer.Instance.Print(font, mousePosition, $"{row}:{col}");
        }
    }
}
