using Good.Core;
using Good.Editor.Command;

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
}
