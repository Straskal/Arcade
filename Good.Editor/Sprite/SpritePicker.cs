using Good.Core;
using Microsoft.Xna.Framework;

namespace Good.Editor
{
    public class SpritePicker : MainGameState
    {
        private const int SpriteSize = 16;
        private const int SpritesPerRow = 4;
        private const int SpriteSpaceBuffer = 5;
        private const int PanelWidth = SpriteSize * SpritesPerRow + SpriteSpaceBuffer;
        private const int PanelX = Renderer.ResolutionWidth - PanelWidth;
        private const int PanelHeight = Renderer.ResolutionHeight;

        public override bool IsTranscendent => true;
        public override bool IsTransparent => true;

        private EditorBase editorBase;

        public override void Enter()
        {
            editorBase = MainGame.Instance.GetState<EditorBase>();
        }

        public override void Draw()
        {
            DrawPanel();
        }

        private void DrawPanel()
        {
            Renderer.Instance.DrawRectangle(new Rectangle(PanelX, 0, PanelWidth, PanelHeight), new Color(Color.Black, 0.9f));
            Renderer.Instance.Print(editorBase.Font, new Vector2(PanelX, 10), "Hey you!");
        }
    }
}
