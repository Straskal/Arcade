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
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                        { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                    }
                }
            };

            Sprite createPlayer(int x, int y) => new Sprite
            {
                Behaviors = new List<Behavior>
                {
                    new PlayerBehavior()
                },
                BodyInfo = new SpriteBodyInfo
                {
                    Position = new Vector2(x, y),
                    Width = 16,
                    Height = 24,
                },
                DrawInfo = new SpriteDrawInfo
                {
                    Texture = game.Content.Load<Texture2D>("Art/peasant"),
                    Source = new Rectangle(0, 0, 16, 24),
                },
                AnimationInfo = new SpriteAnimationInfo
                {
                    CurrentAnimationName = "idle",
                    Animations = new Dictionary<string, SpriteAnimationInfo.Animation>
                    {
                        {
                            "idle",
                            new SpriteAnimationInfo.Animation
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
                            new SpriteAnimationInfo.Animation
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
                            new SpriteAnimationInfo.Animation
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
                            new SpriteAnimationInfo.Animation
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
                }
            };

            level.Add(createPlayer(10, 10));

            game.Push(level);
            //game.Push(new TilemapEditorState());
            game.Run();
        }
    }
}
