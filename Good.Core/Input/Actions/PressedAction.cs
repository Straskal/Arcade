using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Good.Core
{
    public class PressedAction : InputAction
    {
        public List<Keys> Keys { get; } = new List<Keys>();

        public PressedAction(InputManager.InputState state) : base(state) { }

        public bool IsDown()
        {
            for (int i = 0; i < Keys.Count; i++)
            {
                if (inputState.KeyState.IsKeyDown(Keys[i]))
                    return true;
            }
            return false;
        }

        public bool WasPressed()
        {
            for (int i = 0; i < Keys.Count; i++)
            {
                if (inputState.KeyState.IsKeyDown(Keys[i]) && inputState.PreviousKeyState.IsKeyUp(Keys[i]))
                    return true;
            }
            return false;
        }
    }
}
