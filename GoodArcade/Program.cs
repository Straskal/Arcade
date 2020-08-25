using Good.Core;
using Good.Editor;
using Good.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace GoodArcade
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new MainGame();

            var level = new Layout
            {
                Map = new LayoutMap
                {
                    Tileset = new Tileset
                    {
                        Texture = game.Content.Load<Texture2D>("art/LOTP")
                    },
                    Data = new[,]
                    {
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    }
                }
            };

            level.Add(new Sprite
            {
                Texture = game.Content.Load<Texture2D>("Art/peasant"),
                Source = new Rectangle(0, 0, 16, 24),
                Position = new Vector2(10, 10),
                Width = 16,
                Height = 24,
                Priority = 10,
                Behaviors = new List<Behavior>
                {
                    new PlayerBehavior()
                },
                CurrentAnimation = "idle",
                Animations = new Dictionary<string, Animation> 
                {
                    { 
                        "idle", 
                        new Animation
                        {
                            Frames = new[]
                            {
                                new Rectangle(0, 0, 16, 24),
                            },
                            Speed = 0.5f
                        }
                    },
                    { 
                        "walk_down", 
                        new Animation 
                        {
                            Frames = new[] 
                            {
                                new Rectangle(16, 0, 16, 24),
                                new Rectangle(32, 0, 16, 24)
                            },
                            Speed = 0.5f
                        } 
                    },
                    {
                        "walk_up",
                        new Animation
                        {
                            Frames = new[]
                            {
                                new Rectangle(64, 0, 16, 24),
                                new Rectangle(80, 0, 16, 24)
                            },
                            Speed = 0.5f
                        }
                    },
                    {
                        "walk_right",
                        new Animation
                        {
                            Frames = new[]
                            {
                                new Rectangle(96, 0, 16, 24),
                                new Rectangle(112, 0, 16, 24)
                            },
                            Speed = 0.5f
                        }
                    }
                }
            });

            level.Add(new Sprite
            {
                Texture = game.Content.Load<Texture2D>("Art/peasant"),
                Source = new Rectangle(0, 0, 16, 24),
                Position = new Vector2(40, 10),
                Width = 16,
                Height = 24,
                IsSolid = true,
                CurrentAnimation = "walk",
                Animations = new Dictionary<string, Animation>
                {
                    { "walk", new Animation
                        {
                            Frames = new[]
                            {
                                new Rectangle(16, 0, 16, 24),
                                new Rectangle(32, 0, 16, 24)
                            },
                            Speed = 0.5f
                        }
                    }
                }
            });

            game.Push(level);
            //game.Push(new TilemapEditorState());
            game.Run();
        }
    }
}
