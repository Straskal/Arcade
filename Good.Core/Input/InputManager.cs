using Good.Core.Input.Actions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Good.Core
{
    public static class InputManager
    {
        private static readonly InputState state = new InputState();

        public class InputState
        {
            public MouseState MouseState { get; set; }
            public MouseState PreviousMouseState { get; set; }
            public KeyboardState KeyState { get; set; }
            public KeyboardState PreviousKeyState { get; set; }
            public GamePadState GamePadState { get; set; }
            public GamePadState PreviousGamePadState { get; set; }
        }

        internal static void Update()
        {
            state.PreviousMouseState = state.MouseState;
            state.PreviousKeyState = state.KeyState;
            state.PreviousGamePadState = state.GamePadState;
            state.MouseState = Mouse.GetState();
            state.KeyState = Keyboard.GetState();
            state.GamePadState = GamePad.GetState(0);
        }

        public static Vector2 GetMousePosition() 
        {
            return Renderer.Instance.TransformScreenCoords(state.MouseState.Position.ToVector2());
        }

        public static bool IsMouseLeftDown()
        {
            return state.MouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool WasMouseLeftPressed()
        {
            return state.MouseState.LeftButton == ButtonState.Pressed && state.PreviousMouseState.LeftButton == ButtonState.Released;
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
