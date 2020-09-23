using Good.Core;
using Good.Editor.Command;
using Good.Editor.Map;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Good.Editor
{
    // The editor base expects to have the layout-under-edit underneath it and all additional editor states above.
    public class EditorBase : MainGameState
    {
        public override bool IsTranscendent => isPlaying;
        public override bool IsTransparent => true;
        public CommandQueue Commands { get; private set; }
        public SpriteFont Font { get; private set; }

        private SpriteEditorBase spriteEditor;
        private MapEditorBase mapEditor;

        private PressedAction gotoSpriteEditor;
        private PressedAction gotoTileEditor;
        private PressedAction toggleGame;
        private bool isPlaying;

        public override void Enter()
        {
            spriteEditor = new SpriteEditorBase();
            mapEditor = new MapEditorBase();

            gotoSpriteEditor = InputManager.NewPressedAction(Keys.D1);
            gotoTileEditor = InputManager.NewPressedAction(Keys.D2);
            toggleGame = InputManager.NewPressedAction(Keys.OemTilde);

            Commands = new CommandQueue();
            Font = MainGame.Instance.Content.Load<SpriteFont>("Font/BasicFont");

            MainGame.Instance.Push(spriteEditor);
        }

        public override void Update()
        {
            Commands.Update();

            Layout.Current.PumpSpriteOperations();

            if (gotoSpriteEditor.WasPressed()) 
            {
                MainGame.Instance.PopAbove(this);
                MainGame.Instance.Push(spriteEditor);
            }

            if (gotoTileEditor.WasPressed())
            {
                MainGame.Instance.PopAbove(this);
                MainGame.Instance.Push(mapEditor);
            }

            if (toggleGame.WasPressed())
            {
                if (isPlaying = !isPlaying)
                    MainGame.Instance.PopAbove(this);
                else
                    MainGame.Instance.Push(spriteEditor);
            }
        }
    }
}
