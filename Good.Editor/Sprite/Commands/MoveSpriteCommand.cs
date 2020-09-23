using Good.Core;
using Good.Editor.Command;
using Microsoft.Xna.Framework;

namespace Good.Editor
{
    public class MoveSpriteCommand : ICommand
    {
        private readonly Sprite sprite;
        private readonly Vector2 newPosition;
        private readonly Vector2 previousPosition;

        public MoveSpriteCommand(Sprite sprite, Vector2 newPosition, Vector2 previousPosition)
        {
            this.sprite = sprite;
            this.newPosition = newPosition;
            this.previousPosition = previousPosition;
        }

        public void Do()
        {
            sprite.BodyInfo.Position = newPosition;
            Layout.Current.Grid.Update(sprite);
        }

        public void Undo()
        {
            sprite.BodyInfo.Position = previousPosition;
            Layout.Current.Grid.Update(sprite);
        }
    }
}
