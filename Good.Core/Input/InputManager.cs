using Good.Core.Input.Actions;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Good.Core
{
    public static class InputManager
    {
        private static readonly InputState state = new InputState();

        public class InputState
        {
            public KeyboardState KeyState { get; set; }
            public KeyboardState PreviousKeyState { get; set; }
            public GamePadState GamePadState { get; set; }
            public GamePadState PreviousGamePadState { get; set; }
        }

        internal static void Update()
        {
            state.PreviousKeyState = state.KeyState;
            state.PreviousGamePadState = state.GamePadState;
            state.KeyState = Keyboard.GetState();
            state.GamePadState = GamePad.GetState(0);
        }

        public static PressedAction NewPressedAction(params Keys[] keys)
        {
            var action = new PressedAction(state);
            action.Keys.AddRange(keys);
            return action;
        }

        public static ModifiedPressedAction NewModifiedPressedAction(Keys modifier, params Keys[] keys)
        {
            var action = new ModifiedPressedAction(state) { Modifier = modifier };
            action.Keys.AddRange(keys);
            return action;
        }
    }
}
