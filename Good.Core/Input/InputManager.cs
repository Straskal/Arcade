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

        internal static void Poll()
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
            return View.TransformScreenCoords(state.MouseState.Position.ToVector2());
        }

        public static bool ScrollUp()
        {
            return state.MouseState.ScrollWheelValue > state.PreviousMouseState.ScrollWheelValue;
        }

        public static bool ScrollDown()
        {
            return state.MouseState.ScrollWheelValue < state.PreviousMouseState.ScrollWheelValue;
        }

        public static bool IsMouseMiddleDown()
        {
            return state.MouseState.MiddleButton == ButtonState.Pressed;
        }

        public static bool IsMouseLeftDown()
        {
            return state.MouseState.LeftButton == ButtonState.Pressed;
        }

        public static bool WasMouseLeftPressed()
        {
            return state.MouseState.LeftButton == ButtonState.Pressed && state.PreviousMouseState.LeftButton == ButtonState.Released;
        }

        public static bool IsMouseRightDown()
        {
            return state.MouseState.RightButton == ButtonState.Pressed;
        }

        public static bool WasMouseRightPressed()
        {
            return state.MouseState.RightButton == ButtonState.Pressed && state.PreviousMouseState.RightButton == ButtonState.Released;
        }

        public static Vector2 GetMouseMotion() 
        {
            var current = View.TransformScreenCoords(state.MouseState.Position.ToVector2());
            var previous = View.TransformScreenCoords(state.PreviousMouseState.Position.ToVector2());

            return current - previous;
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
