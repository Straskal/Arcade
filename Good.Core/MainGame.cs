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

        public GraphicsDeviceManager GraphicsManager { get; }
        public Renderer Renderer { get; }

        private readonly List<MainGameState> states;
        private readonly Queue<Action> stackOperations;

        public MainGame()
        {
            Instance = this;

            states = new List<MainGameState>();
            stackOperations = new Queue<Action>();

            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1f / 60f);

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Window.IsBorderless = false;

            GraphicsManager = new GraphicsDeviceManager(this)
            {
                IsFullScreen = false,
                PreferredBackBufferWidth = View.ResolutionWidth * 3,
                PreferredBackBufferHeight = View.ResolutionHeight * 3,
                HardwareModeSwitch = false,
                SynchronizeWithVerticalRetrace = true,
                PreferMultiSampling = false,
                GraphicsProfile = GraphicsProfile.HiDef
            };

            GraphicsManager.ApplyChanges();

            Renderer = new Renderer(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
            Content.Unload();
        }

        public void Push(MainGameState state) 
        {
            stackOperations.Enqueue(() => 
            {
                states.Add(state);
                state.Enter();
            });
        }

        public void Pop() 
        {
            stackOperations.Enqueue(() =>
            {
                var state = states.Last();
                state.Exit();
                states.Remove(state);
            });
        }

        public void PopAbove(MainGameState state) 
        {
            int i = states.Count - 1;

            while (states.ElementAt(i--) != state) 
                Pop();
        }

        public T GetState<T>() where T : MainGameState 
        {
            return states.FirstOrDefault(state => state is T) as T;
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
                GraphicsManager.PreferredBackBufferWidth = View.ResolutionWidth * 3;
                GraphicsManager.PreferredBackBufferHeight = View.ResolutionHeight * 3;
            }

            GraphicsManager.ApplyChanges();
        }

        protected override void Update(GameTime gameTime)
        {
            Time = gameTime;
            InputManager.Poll();

            for (int i = states.Count - 1; i >= 0; i--) 
            {
                var state = states.ElementAt(i);
                View.UpdateTransformation(state.Transformation);
                state.Update();
                if (!state.IsTranscendent) break;
            }

            while (stackOperations.Count > 0) 
                stackOperations.Dequeue().Invoke();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            View.AdjustResolution();

            for (int i = 0; i < states.Count; i++)
            {
                if (i == states.Count - 1 || states.ElementAt(i + 1).IsTransparent) 
                {
                    var state = states.ElementAt(i);
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
