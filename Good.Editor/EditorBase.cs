using Good.Core;
using Good.Editor.Command;
using Good.Editor.Map;
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

        public CommandQueue Commands { get; private set; }

        public override void Enter()
        {
            gotoSpriteEditor = InputManager.NewPressedAction(Keys.D1);
            gotoTileEditor = InputManager.NewPressedAction(Keys.D2);

            Commands = new CommandQueue();

            MainGame.Instance.Push(new SpriteEditorBase());
        }

        public override void Update()
        {
            Commands.Update();

            Layout.Current.PumpSpriteOperations();

            if (gotoSpriteEditor.WasPressed()) 
            {
                MainGame.Instance.PopAbove(this);
                MainGame.Instance.Push(new SpriteEditorBase());
            }

            if (gotoTileEditor.WasPressed())
            {
                MainGame.Instance.PopAbove(this);
                MainGame.Instance.Push(new MapEditorBase());
            }
        }
    }
}
