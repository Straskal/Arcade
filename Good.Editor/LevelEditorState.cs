using Good.Core;
using Good.Editor.Tilemap;

namespace Good.Editor
{
    public class LevelEditorState : GameState
    {
        private TilePicker tilePicker;

        public override bool UpdateBelow => false;
        public override bool DrawBelow => true;

        public override void Enter()
        {
            tilePicker = new TilePicker();
        }

        public override void Draw()
        {
            tilePicker.Draw();
        }        
    }
}
