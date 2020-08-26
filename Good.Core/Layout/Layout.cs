using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Good.Core
{
    public class Layout : MainGameState
    {
        private static readonly Dictionary<string, Func<Sprite>> factories = new Dictionary<string, Func<Sprite>>();
        public static void Register(string name, Func<Sprite> factory) => factories.Add(name, factory);
        public static Sprite Create(string name) => factories[name].Invoke();

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
                sprite.Load();
                spriteOps.Enqueue(() => sprite.Created());
            });
        }

        public override Matrix? Transformation => 
            Matrix.CreateTranslation((int)Math.Floor(-Position.X), (int)Math.Floor(-Position.Y), 0f) 
            * Matrix.CreateScale(Zoom, Zoom, 1f);

        public Vector2 Position { get; set; }
        public float Zoom { get; set; } = 1f;
        public LayoutMap Map { get; set; }
        public LayoutGrid Grid { get; private set; }

        public override void Enter()
        {
            Current = this;
            Grid = new LayoutGrid(Map.Data.GetLength(0), Map.Data.GetLength(1), GridCellSize);
            onSpriteAdded = sprite =>
            {
                sprite.Load();
                sprite.Created();
            };
            PumpSpriteOperations();
        }

        public override void Exit()
        {
            Current = null;
        }

        public override void Update()
        {
            sprites.ForEach(sprite => sprite.Update());
            sprites.ForEach(sprite => sprite.Animate());
            sprites.ForEach(sprite => sprite.BodyInfo.PreviousPosition = sprite.BodyInfo.Position);
            PumpSpriteOperations();
        }

        public override void Draw()
        {
            Map.Draw();
            sprites.ForEach(sprite => sprite.Draw());
        }

        public void PumpSpriteOperations() 
        {
            bool needSort = spriteOps.Count > 0;

            while (spriteOps.Count > 0)
                spriteOps.Dequeue().Invoke();

            if (needSort)
                sprites.Sort((x, y) => x.DrawInfo.Priority.CompareTo(y.DrawInfo.Priority));
        }

        public void Spawn(string name, int x, int y) 
        {
            var sprite = Create(name);
            sprite.BodyInfo.Position = new Vector2(x, y);

            spriteOps.Enqueue(() =>
            {
                sprites.Add(sprite);
                Grid.Add(sprite);
            });
            onSpriteAdded.Invoke(sprite);
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
                sprite.Destroyed();
                sprites.Remove(sprite);
                Grid.Remove(sprite);
            });
        }

        public void MoveAndClamp(Vector2 position)
        {
            Position = Vector2.Clamp(position, new Vector2(0, 0), new Vector2(Map.Width - Renderer.ResolutionWidth, Map.Height - Renderer.ResolutionHeight));
        }
    }
}
