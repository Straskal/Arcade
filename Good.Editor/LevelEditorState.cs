using Good.Core;

namespace Good.Editor
{
    public class LevelEditorState : GameState 
    {
        public override bool UpdateBelow => false;
        public override bool DrawBelow => true;

        public LevelState Level { get; private set; }

        public override void Enter()
        {
            Level = MainGame.Instance.GetState<LevelState>();
        }

        public override void Update()
        {
            
        }

        public override void Draw()
        {

        }
    }
}
