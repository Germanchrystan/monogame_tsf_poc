using System;
using audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace POVMidiPlayer;

public class Game1 : Game
{
  private GraphicsDeviceManager _graphics;
  private SpriteBatch _spriteBatch;
  private AudioRenderer audio;
  private MidiEvent[] midiEvents;
  public Game1()
  {
    _graphics = new GraphicsDeviceManager(this);
    Content.RootDirectory = "Content";
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
    audio = new AudioRenderer("fluid.sf2");
      midiEvents =
      [
        new MidiEvent { Time = 0f, Channel = 0, Key = 60, Velocity = 1f, IsNoteOn = true },
        new MidiEvent { Time = 0.5f, Channel = 0, Key = 60, Velocity = 0f, IsNoteOn = false },
        new MidiEvent { Time = 1f, Channel = 0, Key = 62, Velocity = 1f, IsNoteOn = true },
        new MidiEvent { Time = 1.5f, Channel = 0, Key = 62, Velocity = 0f, IsNoteOn = false },
        new MidiEvent { Time = 2f, Channel = 0, Key = 64, Velocity = 1f, IsNoteOn = true },
        new MidiEvent { Time = 2.5f, Channel = 0, Key = 64, Velocity = 0f, IsNoteOn = false },
      ];
    audio.LoadPlayer();
    audio.LoadMidi(midiEvents);
  }
  protected override void Update(GameTime gameTime)
  {
    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
      Exit();
    try
    {
      audio.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
    } catch (Exception ex)
    {
      Console.WriteLine("Error during update: " + ex.Message);
      Console.WriteLine($"Is audio working? {(audio != null ? "Yes" : "No")}");
    }  
    base.Update(gameTime);
  }

  protected override void Draw(GameTime gameTime)
  {
    GraphicsDevice.Clear(Color.CornflowerBlue);

    // TODO: Add your drawing code here

    base.Draw(gameTime);
  }
}