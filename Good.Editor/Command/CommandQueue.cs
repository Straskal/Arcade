using Good.Core;
using Good.Core.Input.Actions;
using Microsoft.Xna.Framework.Input;

namespace Good.Editor.Command
{
    /// <summary>
    /// Circular command queue.
    /// </summary>
    public class CommandQueue
    {
        public const int Max = 20;

        private readonly ICommand[] queue = new ICommand[Max];
        private readonly ModifiedPressedAction undo;
        private readonly ModifiedPressedAction redo;

        private int start = 0;
        private int end = 0;
        private int position = 0;

        public CommandQueue()
        {
            undo = InputManager.NewModifiedPressedAction(Keys.LeftControl, Keys.Z);
            redo = InputManager.NewModifiedPressedAction(Keys.LeftControl, Keys.Y);
        }

        public void Update()
        {
            if (undo.WasPressed() && position != start) 
                queue[Wrap(--position)].Undo();

            if (redo.WasPressed() && position != end) 
                queue[Wrap(position++)].Do();
        }

        public void Insert(ICommand command) 
        {
            command.Do();

            if (position == end)
            {
                end = Wrap(++end);
                if (end == start)
                    start = Wrap(++start);
            }

            queue[Wrap(position++)] = command;
        }

        private int Wrap(int position) 
        {
            if (position > Max - 1)
                position = 0;
            else if (position < 0)
                position = Max - 1;
            return position;
        }
    }
}
