using Microsoft.Xna.Framework;
using tsybulya.GameObjects;
using notation.scale;
using notation.music_file;
using notation.note;
using POVMidiPlayer;
using System.Collections.Generic;

namespace entities.score
{
  public class Score: GameObject
  {
    private Scale scale = Scale.CMajor();
    private MusicFile musicFile;
    private Staff[] staffs;
    public Score(Staff[] staffs, TimeSignature timeSignature, int bpm) : base(new Vector2(0, 0), new Vector2(0, 0), 0)
    {
      this.musicFile = new MusicFile()
      {
        bpm = bpm,
        timeSignature = timeSignature,
        voices = [],
      };
      this.staffs = staffs;
      foreach (Staff staff in staffs)
      {
        AddChildren(staff);
      }
    }
    public Score AddScale(Scale scale)
    {
      this.scale = scale;
      return this;
    }
    public MidiEvent[] GetMidiEvents()
    {
      List<MidiEvent> allMidiEvents = new List<MidiEvent>();
      foreach (Staff staff in staffs)
      {
        allMidiEvents.AddRange(staff.GetMidiEvents(musicFile.bpm));
      }
      return allMidiEvents.ToArray();
    }
    public MusicFile GetMusicFile()
    {
      Voice[] newVoices = new Voice[staffs.Length];
      
      for (int index = 0; index < staffs.Length; index++)
      {
        Staff staff = staffs[index];
        RealNote[] realNotes = staff.GetNotes();
        newVoices[index] = new Voice()
        {
          id = staff.RegisterId,
          register = staff.Register,
          notes = realNotes,
        };
      }
      musicFile.voices = newVoices;

      return musicFile;
    }
  }
} 