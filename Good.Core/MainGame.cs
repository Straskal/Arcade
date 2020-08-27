using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Good.Core
{
    public class MainGame : Game
    {
        public static MainGame Instance { get; private set; }
        public static GameTime Time { get; private set; }

        private readonly List<MainGameState> stack;
        private readonly Queue<Action> stackOps;

        public MainGame()
        {
            stack = new List<MainGameState>();
            stackOps = new Queue<Action>();

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1f / 60f);

            Instance = this;
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.IsBorderless = false;

            GraphicsManager = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferWidth = Renderer.ResolutionWidth * 3,
                PreferredBackBufferHeight = Renderer.ResolutionHeight * 3,
                HardwareModeSwitch = false,
                SynchronizeWithVerticalRetrace = true,
                PreferMultiSampling = false,
                GraphicsProfile = GraphicsProfile.HiDef
            };

            GraphicsManager.ApplyChanges();
            Renderer = new Renderer(GraphicsDevice);
        }

        public GraphicsDeviceManager GraphicsManager { get; }
        public Renderer Renderer { get; }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        public void Push(MainGameState state) 
        {
            stackOps.Enqueue(() => 
            {
                stack.Add(state);
                state.Enter();
            });
        }

        public void Pop() 
        {
            stackOps.Enqueue(() =>
            {
                var state = stack.Last();
                state.Exit();
                stack.Remove(state);
            });
        }

        public void PopAbove(MainGameState state) 
        {
            MainGameState top;
            while ((top = stack.Last()) != state)
            {
                top.Exit();
                stack.Remove(top);
            }
        }

        public T GetState<T>() where T : MainGameState 
        {
            return stack.FirstOrDefault(state => state is T) as T;
        }

        public void ToggleFullscreen()
        {
            GraphicsManager.ToggleFullScreen();

            if (GraphicsManager.IsFullScreen)
            {
                GraphicsManager.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                GraphicsManager.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                GraphicsManager.PreferredBackBufferWidth = Renderer.ResolutionWidth * 3;
                GraphicsManager.PreferredBackBufferHeight = Renderer.ResolutionHeight * 3;
            }

            GraphicsManager.ApplyChanges();
        }

        protected override void Update(GameTime gameTime)
        {
            View.AdjustResolution();
            InputManager.Poll();

            Time = gameTime;

            for (int i = stack.Count - 1; i >= 0; i--) 
            {
                var state = stack.ElementAt(i);
                View.UpdateTransformation(state.Transformation);
                state.Update();
                if (!state.IsTranscendent) break;
            }

            while (stackOps.Count > 0) 
                stackOps.Dequeue().Invoke();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            for (int i = 0; i < stack.Count; i++)
            {
                if (i == stack.Count - 1 || stack.ElementAt(i + 1).IsTransparent) 
                {
                    var state = stack.ElementAt(i);
                    View.UpdateTransformation(state.Transformation);
                    Renderer.BeginDraw(View.CurrentTransform);
                    state.Draw();
                    Renderer.EndDraw();
                }
            }

            base.Draw(gameTime);
        }
    }
}
