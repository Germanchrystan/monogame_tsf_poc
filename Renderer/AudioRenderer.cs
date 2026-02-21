using System;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Audio;
using midi;

namespace Renderer;

public class AudioRenderer
{
  private DynamicSoundEffectInstance dynSound;
  private short[] synthBuffer;
  private byte[] byteBuffer;
  private int channelCount = 2; // Stereo
  private Player player;
  private string sf2Path;
  private MidiEvent[] midiEvents = Array.Empty<MidiEvent>();
  private int currentIndex = 0;
  public bool On = true;
  float currentTime = 0f;
  public float duration = 0f; // Will be set based on the MIDI events
  public AudioRenderer(string sf2Path)
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
  public void LoadPlayer()
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
  public void LoadMidi(MidiEvent[] midiEvents)
  {
    this.midiEvents = midiEvents;
    this.midiEvents = midiEvents.OrderBy(e => e.Time).ToArray();
    if (midiEvents.Length > 0)
    {
      this.duration = midiEvents[midiEvents.Length - 1].Time + 0.5f; // Add a little extra time at the end
    }
  }
  public void Update(float deltaTime)
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
  public void ToggleAudio()
  {
    On = !On;
  }
}
