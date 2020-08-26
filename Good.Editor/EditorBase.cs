using Good.Core;
using Good.Editor.Command;
using Microsoft.Xna.Framework.Input;

namespace Good.Editor
{
    // The editor base expects to have the layout-under-edit underneath it and all additional editor states above.
    public class EditorBase : MainGameState
    {
        private PressedAction gotoSpriteEditor;
        private PressedAction gotoTileEditor;

        public override bool IsTranscendent => false;
        public override bool IsTransparent => true;

        public CommandQueue CommandQueue { get; private set; }

        public override void Enter()
        {
            gotoSpriteEditor = InputManager.NewPressedAction(Keys.D1);
            gotoTileEditor = InputManager.NewPressedAction(Keys.D2);

            CommandQueue = new CommandQueue();

            MainGame.Instance.Push(new SpriteEditor());
        }

        public override void Update()
        {
            CommandQueue.Update();

            Layout.Current.PumpSpriteOperations();

            if (gotoSpriteEditor.WasPressed()) 
            {
                MainGame.Instance.PopAbove(this);
                MainGame.Instance.Push(new SpriteEditor());
            }

            if (gotoTileEditor.WasPressed())
            {
                MainGame.Instance.PopAbove(this);
                MainGame.Instance.Push(new MapEditor());
            }
        }
    }
}
