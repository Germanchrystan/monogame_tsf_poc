using System;
using tsybulya.Scenes;
using entities.score;
using Renderer;
using midi;
using Microsoft.Xna.Framework.Input;
using tsybulya.Components.States;

namespace fugue.scenes.level
{
  public class Level : Scene
  {
    private Score score;

    
    // private int bpm = 120;
    // private TimeSignature timeSignature = new TimeSignature() { numerator = 2, denominator = 2 };
    public Level(Score score)
    {
      this.score = score;
      // this.timeSignature = timeSignature;
      // this.bpm = bpm;


      AddChildren(score);
      AddState(new State("IDLE", ["PLAY"], ["Idle"]));
      AddScripts([
        new Idle(),
        new PlayBack()
      ]);
      this.LoadMidi();
    }
    public void LoadMidi()
    {
      MidiEvent[] midiEvents = score.GetMusicFile();
      AudioRenderer.LoadMidi(midiEvents);
    }
    public void ToggleAudio()
    {
      if (!AudioRenderer.On)
      {
        LoadMidi();
      }
      AudioRenderer.ToggleAudio();
    }
  }
}