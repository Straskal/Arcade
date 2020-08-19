using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace Good.Core
{
    public class MainGame : Game
    {
        public static MainGame Instance { get; private set; }
        public static GameTime Time { get; private set; }

        public List<GameState> Stack { get; }
        public GraphicsDeviceManager GraphicsManager { get; }
        public Renderer Renderer { get; }

        public MainGame()
        {
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

            Stack = new List<GameState>();
            Renderer = new Renderer(GraphicsDevice);
        }

        public void Push(GameState state) 
        {
            state.Enter();
            Stack.Add(state);
        }

        public void Pop() 
        {
            var state = Stack.Last();
            Stack.Remove(state);
            state.Exit();
        }

        public T GetState<T>() where T : GameState 
        {
            return Stack.FirstOrDefault(state => state.GetType() == typeof(T)) as T;
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
            Time = gameTime;

            // Update stack from top to bottom.
            for (int i = Stack.Count - 1; i >= 0; i--) 
            {
                var state = Stack.ElementAt(i);
                state.Update();

                if (!state.UpdateBelow)
                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Renderer.BeginDraw();

            // Draw the stack bottom to top.
            for (int i = 0; i < Stack.Count; i++)
            {
                if (i < Stack.Count - 1 && !Stack.ElementAt(i + 1).DrawBelow)
                    continue;

                Stack.ElementAt(i).Draw();
            }

            Renderer.EndDraw();

            base.Draw(gameTime);
        }
    }
}
