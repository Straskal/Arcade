using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Good.Core
{
    public class Layout : MainGameState
    {
        public static Layout Current { get; private set; }

        public readonly List<Sprite> sprites;
        private readonly Queue<Action> spriteOps;
        private Action<Sprite> onSpriteAdded;

        public Layout() 
        {
            sprites = new List<Sprite>();
            spriteOps = new Queue<Action>();

            // Before we enter the layout, defer loading.
            onSpriteAdded = sprite => spriteOps.Enqueue(() =>
            {
                sprite.Behaviors.ForEach(behavior => behavior.Load(sprite));
                spriteOps.Enqueue(() => sprite.Behaviors.ForEach(behavior => behavior.Loaded(sprite)));
            });
        }

        public override Matrix? Transformation =>
            Matrix.CreateTranslation((int)Math.Floor(-Position.X), (int)Math.Floor(-Position.Y), 0f) * Matrix.CreateScale(Zoom, Zoom, 1f);

        public Vector2 Position { get; set; }
        public float Zoom { get; set; } = 1f;
        public Tilemap Map { get; set; }

        public override void Enter()
        {
            Current = this;

            // Once we enter the layout, any additionally added sprites should be loaded immediately.
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
            foreach (var sprite in sprites) 
                sprite.Behaviors.ForEach(behavior => behavior.Update(sprite));

            PumpSpriteOperations();
        }

        public override void Draw()
        {
            int mapWidth = Map.Data.GetLength(1);
            int mapHeight = Map.Data.GetLength(0);

            for (int i = 0; i < mapHeight; i++) 
            {
                for (int j = 0; j < mapWidth; j++) 
                {
                    int index = Map.Data[i, j];
                    if (index != -1)
                    {
                        Rectangle source = Map.Tileset.GetIndexSource(index);
                        Vector2 position = new Vector2(j * 16, i * 16);
                        Renderer.Instance.Draw(Map.Tileset.Texture, source, position, Color.White);
                    }
                }
            }

            sprites.Sort((x, y) => x.Priority.CompareTo(y.Priority));
            foreach (var sprite in sprites) 
                Renderer.Instance.Draw(sprite.Texture, sprite.Source, sprite.Position, sprite.Color, sprite.FlipFlags);
        }

        public void PumpSpriteOperations() 
        {
            while (spriteOps.Count > 0)
                spriteOps.Dequeue().Invoke();
        }

        public void Add(Sprite sprite)
        {
            spriteOps.Enqueue(() => sprites.Add(sprite));
            onSpriteAdded(sprite);
        }

        public void Remove(Sprite sprite) 
        {
            spriteOps.Enqueue(() => 
            {
                sprite.Behaviors.ForEach(behavior => behavior.Unloaded(sprite));
                sprites.Remove(sprite);
            });
        }
    }
}
