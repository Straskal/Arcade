namespace Good.Core
{
    public abstract class InputAction
    {
        protected readonly InputManager.InputState inputState;

        public InputAction(InputManager.InputState state)
        {
            inputState = state;
        }
    }
}
