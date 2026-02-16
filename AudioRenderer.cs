using System;
using System.IO;
using Microsoft.Xna.Framework.Audio;

namespace audio
{
  public struct MidiEvent
  {
    public float Time;
    public int Channel;
    public int Key;
    public float Velocity;
    public bool IsNoteOn;
  }
  public class AudioRenderer
  {
    private DynamicSoundEffectInstance dynSound;
    private short[] synthBuffer;
    private byte[] byteBuffer;
    private int channelCount = 2; // Stereo
    private Player player;
    private string sf2Path;
    private MidiEvent[] midiEvents = Array.Empty<MidiEvent>();
    private float currentTime = 0;
    private int currentIndex = 0;
    private bool On = false;
    public AudioRenderer(string sf2Path)
    {
      this.sf2Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, sf2Path);
      if(!File.Exists(this.sf2Path))
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
        // Console.WriteLine("Failed to initialize player.");
        return;
      }
      Console.WriteLine("Player loaded with SoundFont: " + sf2Path);
      player.ProgramSelect(0, 0, 0);
      player.SetOutput(0, 44100, -6f);
      player.SetVolume(1f);
    }
    public void Reset()
    {
      currentTime = 0f;
      currentIndex = 0;
    }
    public void LoadMidi(MidiEvent[] midiEvents)
    {
      this.midiEvents = midiEvents;
      // Sort events by time just in case
    }
    public void Update(float deltaTime)
    {
      try
      {
        if (player == null)
        {
          Console.WriteLine("Player not initialized.");
          return;
        }
        if (!On) return;
        float previousTime = currentTime;
        currentTime += deltaTime;
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

        while (dynSound.PendingBufferCount < 2)
        {
          player.Render(synthBuffer);

          Buffer.BlockCopy(synthBuffer, 0, byteBuffer, 0, synthBuffer.Length * sizeof(short));
          dynSound.SubmitBuffer(byteBuffer);
        }
        if (currentIndex >= midiEvents.Length) On = false;
      }
      catch (Exception ex)
      {
        Console.WriteLine("Error during audio update: " + ex.Message);
      }
    }
  }
}