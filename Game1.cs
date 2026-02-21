using System;
using System.Collections.Generic;
using fugue.level;
using fugue.scenes.level;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using tsybulya.Components.GraphicComponents;
using tsybulya.Components.States;
using tsybulya.Managers;
using tsybulya.Singletons;
using tsybulya.Singletons.CollisionChecker;
using tsybulya.Singletons.Commands;
using tsybulya.Singletons.Graphics;

namespace POVMidiPlayer
{
  public class Game1 : Game
  {
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SceneManager sceneManager = null;
    private RenderTarget2D renderTarget;
    private Rectangle renderDestination;
    private bool isResizing;
    private const int WINDOW_WIDTH = 1280;
    private const int WINDOW_HEIGHT = 720;
    public Game1()
    {
      _graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
      _graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
      _graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
      _graphics.ApplyChanges();
      Window.AllowUserResizing = true;
      Window.ClientSizeChanged += handleClientSizeChanged;
      IsMouseVisible = true;
    }
    private void handleClientSizeChanged(object sender, EventArgs e)
    {
      if (!isResizing && Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0)
      {
        isResizing = true;
        calculateRenderDestionation();
        isResizing = false;
      }
    }
    private void calculateRenderDestionation()
    {
      Point size = GraphicsDevice.Viewport.Bounds.Size;

      float scaleX = (float)size.X / renderTarget.Width;
      float scaleY = (float)size.Y / renderTarget.Height;
      float scale = Math.Min(scaleX, scaleY);

      renderDestination.Width = (int)(renderTarget.Width * scale);
      renderDestination.Height = (int)(renderTarget.Height * scale);

      renderDestination.X = (size.X - renderDestination.Width) / 2;
      renderDestination.Y = (size.Y - renderDestination.Height) / 2;
    }
    protected override void Initialize()
    {

      base.Initialize();
      _graphics.IsFullScreen = false;
      _graphics.ApplyChanges();
    }
    protected override void LoadContent()
    {
      _spriteBatch = new SpriteBatch(GraphicsDevice);
      renderTarget = new RenderTarget2D(GraphicsDevice, WINDOW_WIDTH, WINDOW_HEIGHT);
      calculateRenderDestionation();

      // Graphic Manager Initialization
      GraphicManager.Initialize();
      GraphicManager.LoadContent(GraphicsDevice, Content);
      Texture2D whiteTexture = new Texture2D(GraphicsDevice, 1, 1);
      whiteTexture.SetData([Color.White]);
      GraphicManager.LoadTexture("white", whiteTexture);

      RendererList.Initialize();

      // Input Initialization
      Input.Initialize(new List<Command>()
      {
        // new Command(Mouse.GetState().LeftButton, Constants.LEFT),
      });

      // Level level = new Level(Score.ScoreFactory("test.mid"), new BasePiece[] { });
      Level level = new Level(LevelScore.CreateSimpleScore());
      sceneManager = new SceneManager(level);
      sceneManager.Build();
      sceneManager.Start();
    }

    protected override void Update(GameTime gameTime)
    {
      if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        Exit();

      float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

      MouseInput.Update(renderDestination, renderTarget);

      AnimatorRenderer.Update(dt);
      Input.Instance.Update();
      sceneManager?.Update(dt);
      CollisionChecker.Update(dt);

      RendererList.Update();
      StateManager.UpdateStates();

      base.Update(gameTime);
    }
    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.SetRenderTarget(renderTarget);
      GraphicsDevice.Clear(Color.White);

      _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
      RendererList.Render(_spriteBatch);
      _spriteBatch.End();

      GraphicsDevice.SetRenderTarget(null);

      _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
      _spriteBatch.Draw(renderTarget, renderDestination, Color.White);
      _spriteBatch.End();

      base.Draw(gameTime);
    }
  }
}
