using tsybulya.Scenes;
using entities.score;
using notation.music_file;
using Renderer;
using midi;
using System;
using Microsoft.Xna.Framework.Input;

namespace fugue.scenes.level
{
  public class Level : Scene
  {
    MidiEvent[] exampleMidiEvents = new MidiEvent[]
    {
      new MidiEvent { Time = 0f,   Channel = 0, Key = 36, Velocity = 1f, IsNoteOn = true  },
      new MidiEvent { Time = 0.5f, Channel = 0, Key = 36, Velocity = 0f, IsNoteOn = false },
      new MidiEvent { Time = 0.5f, Channel = 0, Key = 38, Velocity = 1f, IsNoteOn = true  },
      new MidiEvent { Time = 1f,   Channel = 0, Key = 38, Velocity = 0f, IsNoteOn = false },
      new MidiEvent { Time = 1f,   Channel = 0, Key = 40, Velocity = 1f, IsNoteOn = true  },
      new MidiEvent { Time = 1.5f, Channel = 0, Key = 40, Velocity = 0f, IsNoteOn = false },
    };
    private Score score;
    private AudioRenderer audio;
    private bool isSpacePressed = false;
    public Level(Score score)
    {
      this.score = score;
      AddChildren(score);

      this.audio = new AudioRenderer("fluid.sf2");
      audio.LoadPlayer();
      this.LoadMidi();
    }
    override public void Update(float deltaTime)
    {
      KeyboardState kState = Keyboard.GetState();
      bool isPreviouslyPressed = isSpacePressed;
      isSpacePressed = kState.IsKeyDown(Keys.Space);
      if (!isPreviouslyPressed && isSpacePressed)
      {
        audio.ToggleAudio();
      }
      
      base.Update(deltaTime);
      audio.Update(deltaTime);
    }
    public void LoadMidi()
    {
      MidiEvent[] midiEvents = score.GetMidiEvents();
      foreach (MidiEvent midiEvent in midiEvents)
      {
        Console.WriteLine($"Time: {midiEvent.Time}, Type: {midiEvent.IsNoteOn}, Note: {midiEvent.Key}, Velocity: {midiEvent.Velocity}");
      }
      audio.LoadMidi(midiEvents);
    }
    public void ToggleAudio()
    {
      if (!audio.On)
      {
        LoadMidi();
      }
      Console.WriteLine("Toggling audio. Current state: " + (audio.On ? "On" : "Off"));
      audio.ToggleAudio();
    }
    public MusicFile GetFile()
    {
      return this.score.GetMusicFile();
    }
  }
}