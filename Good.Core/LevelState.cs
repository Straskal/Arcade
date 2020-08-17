using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Good.Core
{
    public class LevelState : GameState
    {
        public static LevelState Current { get; private set; }

        private readonly HashSet<Sprite> spritesToRemove;
        private Action<Sprite> onSpriteAdded;

        public LevelState() 
        {
            Sprites = new List<Sprite>();
            spritesToRemove = new HashSet<Sprite>();

            // We don't want to load any added sprites before the level state has entered. Use a dummy func.
            onSpriteAdded = sprite => { };
        }

        internal List<Sprite> Sprites { get; set; }
        public Vector2 Position { get; set; }

        public override void Enter()
        {
            Current = this;

            // When we enter the level, we change the behavior of adding sprites so that they are loaded immediately.
            onSpriteAdded = sprite =>
            {
                sprite.Behaviors.ForEach(behavior => behavior.Load(sprite));
                sprite.Behaviors.ForEach(behavior => behavior.Loaded(sprite));
            };

            Array.ForEach(Sprites.ToArray(), sprite => onSpriteAdded(sprite));
        }

        public override void Exit()
        {
            Current = null;
        }

        public override void Update()
        {
            // Iterate over immutable copy of sprites.
            // This allows sprites to be added without corrupting list.
            foreach (Sprite sprite in Sprites.ToArray())
                sprite.Behaviors.ForEach(behavior => behavior.Update(sprite));

            foreach (var sprite in spritesToRemove)
                Sprites.Remove(sprite);

            spritesToRemove.Clear();
        }

        public override void Draw()
        {
            Sprites.Sort(new SpritePriorityComparer());

            foreach (var sprite in Sprites) 
                Renderer.Instance.Draw(sprite.Texture, sprite.Source, sprite.Position, sprite.Color, sprite.FlipFlags);
        }

        public void Add(Sprite sprite) 
        {
            if (Sprites.Contains(sprite))
                throw new InvalidOperationException("Attempting to add an already existent sprite into level.");

            Sprites.Add(sprite);
            onSpriteAdded(sprite);
        }

        public void Remove(Sprite sprite) 
        {
            if (!Sprites.Contains(sprite))
                throw new InvalidOperationException("Attempting to remove an sprite that does not exist.");

            spritesToRemove.Add(sprite);
        }
    }
}
