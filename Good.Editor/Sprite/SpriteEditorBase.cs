using Good.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace Good.Editor
{
    public class SpriteEditorBase : MainGameState
    {
        private readonly Color HoveredSpriteColor = Color.Pink;
        private readonly Color SelectedSpriteColor = Color.White;

        public override bool IsTranscendent => true;
        public override bool IsTransparent => true;
        public override Matrix? Transformation => Layout.Current.Transformation;

        private EditorBase editorBase;
        private SpritePicker spritePicker;

        private Sprite selected;
        private Vector2 selectedOffset;
        private Vector2 selectedPreviousPosition;
        private Sprite previousHovered;
        private Color previousHoveredColor;
        private Vector2 mousePosition;
        private PressedAction deleteSpriteAction;
        private Action currentMode;

        public override void Enter()
        {
            editorBase = MainGame.Instance.GetState<EditorBase>() ?? throw new InvalidOperationException("The sprite editor must be stacked on top of the editor base.");
            spritePicker = new SpritePicker();
            MainGame.Instance.Push(spritePicker);
            deleteSpriteAction = InputManager.NewPressedAction(Keys.Delete);
            currentMode = Select;
        }

        public override void Exit()
        {
            if (previousHovered != null)
                previousHovered.DrawInfo.Color = previousHoveredColor;
        }

        public override void Update()
        {
            mousePosition = InputManager.GetMousePosition();
            currentMode.Invoke();

            if (InputManager.IsMouseMiddleDown())
                Layout.Current.Position -= InputManager.GetMouseMotion();
        }

        public override void Draw()
        {
            if (selected != null)
            {
                var selectedBox = selected.BodyInfo.Bounds;
                selectedBox.X -= 1;
                selectedBox.Y -= 1;
                selectedBox.Width += 2;
                selectedBox.Height += 2;
                Renderer.Instance.DrawRectangleLines(selectedBox, SelectedSpriteColor);
            }
        }

        private void Select()
        {
            var hovered = Layout.Current.Grid.QueryPoint(mousePosition.ToPoint()).FirstOrDefault();

            if (hovered != previousHovered)
            {
                if (previousHovered != null)
                    previousHovered.DrawInfo.Color = previousHoveredColor;

                if (hovered != null)
                {
                    previousHoveredColor = hovered.DrawInfo.Color;
                    hovered.DrawInfo.Color = HoveredSpriteColor;
                }
            }

            previousHovered = hovered;

            if (InputManager.WasMouseLeftPressed())
            {
                selected = hovered;

                if (selected != null)
                    selectedPreviousPosition = selected.BodyInfo.Position;
            }

            if (deleteSpriteAction.WasPressed() && selected != null)
            {
                selected.BodyInfo.Position = selectedPreviousPosition;
                editorBase.Commands.Insert(new RemoveSpriteCommand(selected));
                selected = null;
            }

            if (InputManager.IsMouseLeftDown() && InputManager.GetMouseMotion() != Vector2.Zero && hovered != null) 
            {
                selectedOffset = mousePosition - hovered.BodyInfo.Position;
                currentMode = Drag;
            }
        }

        private void Drag() 
        {
            if (selected == null)
                return;

            selected.BodyInfo.Position = Vector2.Round(mousePosition - selectedOffset);
            Layout.Current.Grid.Update(selected);

            if (!InputManager.IsMouseLeftDown()) 
            {
                if (selected.BodyInfo.Position != selectedPreviousPosition)
                    editorBase.Commands.Insert(new MoveSpriteCommand(selected, selected.BodyInfo.Position, selectedPreviousPosition));

                currentMode = Select;
            }
        }
    }
}
