using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Good.Core
{
    public class Layout : MainGameState
    {
        public static Layout Current => stack.Peek();

        private static readonly Dictionary<string, Func<Sprite>> factories = new Dictionary<string, Func<Sprite>>();
        private static readonly Stack<Layout> stack = new Stack<Layout>();

        public override Matrix? Transformation => GetTransformation();

        public Vector2 Position { get; set; }
        public float Zoom { get; set; } = 1f;

        public LayoutMap Map { get; set; }
        public LayoutGrid Grid { get; private set; }

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

        public static void Register(string name, Func<Sprite> factory) 
        {
            factories.Add(name, factory);
        }

        public static Sprite Create(string name) 
        {
            return factories[name].Invoke();
        }

        public override void Enter()
        {
            stack.Push(this);

            Grid = new LayoutGrid(Map.Data.GetLength(0), Map.Data.GetLength(1), 128);

            onSpriteAdded = sprite =>
            {
                sprite.Load();
                sprite.Created();
            };

            PumpSpriteOperations();
        }

        public override void Exit()
        {
            stack.Pop();
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
            if (spriteOps.Count > 0) 
            {
                while (spriteOps.Count > 0)
                    spriteOps.Dequeue().Invoke();

                sprites.Sort((x, y) => x.DrawInfo.Priority.CompareTo(y.DrawInfo.Priority));
            }
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
            Position = Vector2.Clamp(position, new Vector2(0, 0), new Vector2(Map.Width - View.ResolutionWidth, Map.Height - View.ResolutionHeight));
        }

        public Matrix GetTransformation() 
        {
            return Matrix.CreateTranslation((int)Math.Floor(-Position.X), (int)Math.Floor(-Position.Y), 0f)
                * Matrix.CreateScale(Zoom, Zoom, 1f);
        }
    }
}
