using System;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using POVMidiPlayer;

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
  private bool On = true;

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
    // TODO: Sort events by time just in case
  }
  public void Update(float currentTime)
  {
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
}
