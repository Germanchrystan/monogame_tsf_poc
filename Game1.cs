using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Renderer;

namespace POVMidiPlayer;

public class Game1 : Game
{
  private GraphicsDeviceManager _graphics;
  private SpriteBatch _spriteBatch;
  private AVRenderer renderer;
  
  private float deltaTime = 0f;
  public Game1()
  {
    _graphics = new GraphicsDeviceManager(this);
    Content.RootDirectory = "Content";
    _graphics.PreferredBackBufferWidth = Constants.WINDOW_WIDTH;
    _graphics.PreferredBackBufferHeight = Constants.WINDOW_HEIGHT;
    _graphics.ApplyChanges();
    IsMouseVisible = true;
  }

  protected override void Initialize()
  {
    try
    {
      base.Initialize();
    }
    catch (Exception ex)
    {
      Console.WriteLine("Error during initialization: " + ex.Message);
    }
  }
  protected override void LoadContent()
  {
    _spriteBatch = new SpriteBatch(GraphicsDevice);
    renderer = new AVRenderer("fluid.sf2", Midi.midiExample, GraphicsDevice);
  }
  protected override void Update(GameTime gameTime)
  {
    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
      Exit();
    deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
    try
    {
      renderer.Update(deltaTime);
    } catch (Exception ex)
    {
      Console.WriteLine("Error during update: " + ex.Message);
    }  
    
    base.Update(gameTime);
  }

  protected override void Draw(GameTime gameTime)
  {
    GraphicsDevice.Clear(Color.CornflowerBlue);
    
    _spriteBatch.Begin();
    renderer.Render(_spriteBatch);
    _spriteBatch.End();

    base.Draw(gameTime);
  }
}