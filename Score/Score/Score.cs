using System.Collections.Generic;
using Microsoft.Xna.Framework;

using tsybulya.GameObjects;
using notation.scale;
using notation.music_file;
using midi;
using tsybulya.Components.States;

namespace entities.score;
public class Score : GameObject
{
  private Scale scale = Scale.CMajor();
  private MusicFile musicFile;
  private List<MidiEvent> midiEvents = new List<MidiEvent>();
  private List<NoteSlotRef> noteSlotRefs = new List<NoteSlotRef>();
  private Staff[] staffs;
  private State idle = new State(Constants.IDLE, [Constants.PLAYING], [Constants.IDLE]);
  private State playing = new State(Constants.PLAYING, [Constants.IDLE], [Constants.PLAYING]);
  public Score(Staff[] staffs, TimeSignature timeSignature, int bpm) : base(new Vector2(0, 0), new Vector2(0, 0), 0)
  {
    this.musicFile = new MusicFile()
    {
      bpm = bpm,
      timeSignature = timeSignature,
    };
    this.staffs = staffs;
    foreach (Staff staff in staffs)
    {
      AddChildren(staff);
    }
    AddState(idle);
    AddState(playing);
    AddScripts([new Idle(), new Playing()]);
    SetReference("midiEvents", midiEvents);
    SetReference("noteSlotRefs", noteSlotRefs);
  }
  public Score AddScale(Scale scale)
  {
    this.scale = scale;
    return this;
  }
  private void getMidiEvents()
  {
    foreach (Staff staff in staffs)
    {
      staff.GetMidiEvents(midiEvents, noteSlotRefs);
      noteSlotRefs.Sort((a, b) => a.TimePosition.CompareTo(b.TimePosition));
      midiEvents.Sort((a, b) => a.Time.CompareTo(b.Time));
    }
  }
  public MidiEvent[] GetMusicFile()
  {
    getMidiEvents();
    return midiEvents.ToArray();
  }
}