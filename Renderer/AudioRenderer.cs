using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using midi;

namespace Renderer;
public sealed class AudioRenderer
{
  private DynamicSoundEffectInstance dynSound;
  private short[] synthBuffer;
  private byte[] byteBuffer;
  private readonly int channelCount = 2; // Stereo
  private Player player;
  private string sf2Path = string.Empty;
  private MidiEvent[] midiEvents = Array.Empty<MidiEvent>();
  private int currentIndex = 0;
  private bool on = false;
  private float currentTime = 0f;
  private float duration = 0f; // Will be set based on the MIDI events

  private static AudioRenderer instance;
  public static bool On => instance != null && instance.on;

  private AudioRenderer()
  {
  }
  private void initialize(string sf2Path)
  {
    this.sf2Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sf2Path);
    if (!File.Exists(this.sf2Path))
    {
      Console.WriteLine($"SoundFont file not found: {this.sf2Path}");
    }
    dynSound = new DynamicSoundEffectInstance(44100, AudioChannels.Stereo);
    dynSound.Play();

    synthBuffer = new short[2048 * channelCount];
    byteBuffer = new byte[synthBuffer.Length * sizeof(short)];
  }
  public static void Initialize(string sf2Path)
  {
    if (instance == null)
    {
      instance = new AudioRenderer();
    }
    instance.initialize(sf2Path);
  }
  private void loadPlayer()
  {
    player = new Player(sf2Path);
    if (player == null)
    {
      Console.WriteLine("Failed to initialize player.");
      return;
    }
    Console.WriteLine("Player loaded with SoundFont: " + sf2Path);
    player.ProgramSelect(0, 0, 0);
    player.SetOutput(0, 44100, -6f);
    player.SetVolume(1f);
  }
  public static void LoadPlayer()
  {
    if (instance == null)
    {
      Console.WriteLine("AudioRenderer not initialized.");
      return;
    }
    instance.loadPlayer();
  }
  private void loadMidi(MidiEvent[] midiEvents)
  {
    this.midiEvents = midiEvents;
    this.midiEvents = midiEvents.OrderBy(e => e.Time).ToArray();
    if (midiEvents.Length > 0)
    {
      this.duration = midiEvents[midiEvents.Length - 1].Time + 0.5f; // Add a little extra time at the end
    }
  }
  public static void LoadMidi(MidiEvent[] midiEvents)
  {
    if (instance == null)
    {
      Console.WriteLine("AudioRenderer not initialized.");
      return;
    }
    instance.loadMidi(midiEvents);
  }
  public void update(float deltaTime)
  {
    currentTime += deltaTime;
    if (currentTime >= duration) currentTime = 0f;
    if (player == null)
    {
      Console.WriteLine("Player not initialized.");
      return;
    }
    if (!On) return;
    try
    {
      float previousTime = currentTime;
      while (midiEvents != null &&
             currentIndex < midiEvents.Length &&
             midiEvents[currentIndex].Time <= currentTime)
      {
        var midiEvent = midiEvents[currentIndex];
        if (midiEvent.IsNoteOn)
        {
          player.NoteOn(midiEvent.Channel, midiEvent.Key, midiEvent.Velocity);
        }
        else
        {
          player.NoteOff(midiEvent.Channel, midiEvent.Key);
        }

        currentIndex++;
      }
      if (currentIndex >= midiEvents.Length)
      {
        currentIndex = 0; // Loop back to start
      }

      while (dynSound.PendingBufferCount < 2)
      {
        player.Render(synthBuffer);

        Buffer.BlockCopy(synthBuffer, 0, byteBuffer, 0, synthBuffer.Length * sizeof(short));
        dynSound.SubmitBuffer(byteBuffer);
      }
    }
    catch (Exception ex)
    {
      Console.WriteLine("Error during audio update: " + ex.Message);
    }
  }
  public static void Update(float deltaTime)
  {
    if (instance == null)
    {
      Console.WriteLine("AudioRenderer not initialized.");
      return;
    }
    instance.update(deltaTime);
  }
  private void toggleAudio()
  {
    on = !on;
  }
  public static void ToggleAudio()
  {
    if (instance == null)
    {
      Console.WriteLine("AudioRenderer not initialized.");
      return;
    }
    instance.toggleAudio();
  }
}
