using Good.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace Good.Editor
{
    public class SpriteEditor : MainGameState
    {
        private readonly Color HoveredSpriteColor = Color.Pink;
        private readonly Color SelectedSpriteColor = Color.White;

        public override bool IsTranscendent => true;
        public override bool IsTransparent => true;
        public override Matrix? Transformation => Layout.Current.Transformation;

        private EditorBase editorBase;
        private SpriteFont font;
        private Sprite selected;
        private Sprite lastHovered;
        private Color lastHoveredColor;
        private Vector2 mousePosition;
        private Vector2 selectedOffset;
        private bool isDragging;
        private Vector2 selectedPreviousPosition;

        private PressedAction deleteSpriteAction;

        public override void Enter()
        {
            editorBase = MainGame.Instance.GetState<EditorBase>() ?? throw new InvalidOperationException("The sprite editor must be stacked on top of the editor base.");
            deleteSpriteAction = InputManager.NewPressedAction(Keys.Delete);

            font = MainGame.Instance.Content.Load<SpriteFont>("Font/BasicFont");
        }

        public override void Update()
        {
            mousePosition = InputManager.GetMousePosition();

            var hovered = Layout.Current.Grid.QueryPoint(mousePosition.ToPoint()).FirstOrDefault();

            // Hovered sprite changes.
            if (hovered != lastHovered)
            {
                if (lastHovered != null) 
                    lastHovered.DrawInfo.Color = lastHoveredColor;

                if (hovered != null)
                {
                    lastHoveredColor = hovered.DrawInfo.Color;
                    hovered.DrawInfo.Color = HoveredSpriteColor;
                }
            }

            // Selected sprite changes.
            if (InputManager.WasMouseLeftPressed())
            {
                selected = hovered;

                if (selected != null)
                    selectedPreviousPosition = selected.BodyInfo.Position;
            }

            // If we're hovered over a sprite and not already dragging, then drag.
            if (InputManager.IsMouseLeftDown() && hovered != null && !isDragging)
            {
                selectedOffset = mousePosition - hovered.BodyInfo.Position;
                isDragging = true;
            }
            // Once we've stopped dragging, enqueue a move command.
            else if (!InputManager.IsMouseLeftDown() && isDragging)
            {
                isDragging = false;
                editorBase.CommandQueue.Insert(new MoveSpriteCommand(selected, selected.BodyInfo.Position, selectedPreviousPosition));
            }

            if (isDragging)
                selected.BodyInfo.Position = Vector2.Round(mousePosition - selectedOffset);

            if (deleteSpriteAction.WasPressed() && selected != null) 
            {
                selected.BodyInfo.Position = selectedPreviousPosition;
                editorBase.CommandQueue.Insert(new RemoveSpriteCommand(selected));
                selected = null;
                isDragging = false;
            }

            lastHovered = hovered;
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
    }
}
