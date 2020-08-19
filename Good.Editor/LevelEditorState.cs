using Good.Core;
using Microsoft.Xna.Framework;
using System;

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
            var panel = new Rectangle((Renderer.ResolutionWidth - 16 * 4) - 1, 0, (16 * 4) + 1, Renderer.ResolutionHeight);
            Renderer.Instance.DrawRectangleLines(new Rectangle(10, 10, 16, 24), Color.Red);

            int numRows = Level.Map.Tileset.Texture.Height / 16;
            int numColumns = Level.Map.Tileset.Texture.Width / 16;
            int numCells = numRows * numColumns;
            int x = Renderer.ResolutionWidth - 16 * 4;
            int y = 10;

            for (int i = 0; i < numCells; i++) 
            {
                Rectangle source = Level.Map.Tileset.GetIndexSource(i);
                Renderer.Instance.Draw(Level.Map.Tileset.Texture, source, new Vector2(x, y), Color.White);

                x += 16;

                if ((i + 1) % 4 == 0) 
                {
                    x = Renderer.ResolutionWidth - 16 * 4;
                    y += 16;
                }
            }
        }
    }
}
