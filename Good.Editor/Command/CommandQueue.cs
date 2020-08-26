using Good.Core;
using Good.Core.Input.Actions;
using Microsoft.Xna.Framework.Input;

namespace Good.Editor.Command
{
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
            if (undo.WasPressed()) 
            {
                queue[--position].Undo();
            }

            if (redo.WasPressed()) 
            {
                queue[position++].Do();
            }
        }

        public void Enqueue(ICommand command) 
        {
            command.Do();
            queue[position++] = command;
        }
    }
}
