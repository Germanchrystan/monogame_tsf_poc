using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace POVMidiPlayer;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private bool play = true;
    private Player player;
    private DynamicSoundEffectInstance dynSound;
    private float[] synthBuffer;
    private byte[] byteBuffer;
    private int channelCount = 2; // Stereo
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
            dynSound = new DynamicSoundEffectInstance(44100, AudioChannels.Stereo);
            dynSound.Play();

        } catch (Exception ex)
        {
            Console.WriteLine("Error during initialization: " + ex.Message);
        }
        synthBuffer = new float[2048 * channelCount];
        byteBuffer = new byte[synthBuffer.Length * sizeof(float)];
    }

    protected override void LoadContent()
    {
        string sf2Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fluid.sf2");
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        player = new Player(sf2Path);
    }

    private bool notePlaying = false;
    private double noteOnTime = 0;
    private double duration = 0.5;
    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

       if (!notePlaying && play)
        {
            player.NoteOn(0, 60, 0.8f); // Middle C
            noteOnTime = gameTime.TotalGameTime.TotalSeconds;
            notePlaying = true;
            play = false;
        }

        if (notePlaying)
        {
            double now = gameTime.TotalGameTime.TotalSeconds;
            if(now - noteOnTime >= duration)
            {
                player.NoteOff(0, 60);
                notePlaying = false;
            }
        }

        while(dynSound.PendingBufferCount < 2)
        {
            player.Render(synthBuffer);
            Console.WriteLine("Rendering audio buffer...");
            Console.WriteLine($"synthBuffer length: {synthBuffer.Length}");
            Console.WriteLine($"byteBuffer length: {byteBuffer.Length}");
            Buffer.BlockCopy(synthBuffer, 0, byteBuffer, 0, synthBuffer.Length * sizeof(float));
            Console.WriteLine("Submitting audio buffer...");
            dynSound.SubmitBuffer(byteBuffer);
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
