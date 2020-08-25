using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Good.Core
{
    public class Layout : MainGameState
    {
        public static Layout Current { get; private set; }

        public const int GridCellSize = 128;

        private readonly List<Sprite> sprites;
        private readonly Queue<Action> spriteOps;
        private Action<Sprite> onSpriteAdded;

        public Layout() 
        {
            sprites = new List<Sprite>();
            spriteOps = new Queue<Action>();
            onSpriteAdded = sprite => spriteOps.Enqueue(() =>
            {
                sprite.Behaviors.ForEach(behavior => behavior.Load(sprite));
                spriteOps.Enqueue(() => sprite.Behaviors.ForEach(behavior => behavior.Loaded(sprite)));
            });
        }

        public override Matrix? Transformation => 
            Matrix.CreateTranslation((int)Math.Floor(-Position.X), (int)Math.Floor(-Position.Y), 0f) 
            * Matrix.CreateScale(Zoom, Zoom, 1f);

        public Vector2 Position { get; set; } = Vector2.Zero;
        public float Zoom { get; set; } = 1f;
        public LayoutMap Map { get; set; }
        public LayoutGrid Grid { get; private set; }

        public override void Enter()
        {
            Current = this;
            Grid = new LayoutGrid(Map.Data.GetLength(0), Map.Data.GetLength(1), GridCellSize);
            onSpriteAdded = sprite =>
            {
                sprite.Behaviors.ForEach(behavior => behavior.Load(sprite));
                sprite.Behaviors.ForEach(behavior => behavior.Loaded(sprite));
            };
            PumpSpriteOperations();
        }

        public override void Exit()
        {
            Current = null;
        }

        public override void Update()
        {
            sprites.ForEach(sprite => sprite.UpdateBehaviors());
            sprites.ForEach(sprite => sprite.Animate());
            sprites.ForEach(sprite => sprite.BodyInfo.PreviousPosition = sprite.BodyInfo.Position);
            PumpSpriteOperations();
        }

        public override void Draw()
        {
            Map.Draw();
            sprites.Sort((x, y) => x.DrawInfo.Priority.CompareTo(y.DrawInfo.Priority));
            sprites.ForEach(sprite => sprite.Draw());
        }

        public void PumpSpriteOperations() 
        {
            while (spriteOps.Count > 0)
                spriteOps.Dequeue().Invoke();
        }

        public void Add(Sprite sprite)
        {
            spriteOps.Enqueue(() => 
            {
                sprites.Add(sprite);
                Grid.Add(sprite);
            });
            onSpriteAdded.Invoke(sprite);
        }

        public void Remove(Sprite sprite) 
        {
            spriteOps.Enqueue(() => 
            {
                sprite.Behaviors.ForEach(behavior => behavior.Unloaded(sprite));
                sprites.Remove(sprite);
                Grid.Remove(sprite);
            });
        }
    }
}
