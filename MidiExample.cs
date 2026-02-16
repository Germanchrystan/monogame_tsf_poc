namespace POVMidiPlayer
{
  public struct MidiEvent
  {
    public float Time;
    public int Channel;
    public int Key;
    public float Velocity;
    public bool IsNoteOn;
  }
  public class Midi
  {
    public static MidiEvent[] midiExample =
    [
      new MidiEvent { Time = 0f, Channel = 0, Key = 60, Velocity = 1f, IsNoteOn = true },
      new MidiEvent { Time = 0.5f, Channel = 0, Key = 60, Velocity = 0f, IsNoteOn = false },
      new MidiEvent { Time = 1f, Channel = 0, Key = 62, Velocity = 1f, IsNoteOn = true },
      new MidiEvent { Time = 1.5f, Channel = 0, Key = 62, Velocity = 0f, IsNoteOn = false },
      new MidiEvent { Time = 2f, Channel = 0, Key = 64, Velocity = 1f, IsNoteOn = true },
      new MidiEvent { Time = 2.5f, Channel = 0, Key = 64, Velocity = 0f, IsNoteOn = false },
    ];
  }
}
