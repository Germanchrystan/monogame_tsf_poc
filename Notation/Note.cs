using System;
using System.Runtime.InteropServices;
using notation.interval;
using notation.scale;

namespace notation.note
{
  public enum Alteration
  {
    FLAT = -1,
    NATURAL = 0,
    SHARP = 1,
  }
  [StructLayout(LayoutKind.Sequential)]
  public struct Pitch
  {
    public int pitch; // 0-11
    public int octave;
    public readonly PitchName name;
    public Pitch(int pitch, int octave)
    {
      if (pitch < 0 || pitch > 11)
      {
        throw new ArgumentOutOfRangeException("pitch must be between 0 and 11");
      }
      this.pitch = pitch;
      this.octave = octave;
      this.name = PitchUtils.GetPitchName(pitch);
    }
    public Pitch(PitchName name, int octave)
    {
      this.pitch = PitchUtils.GetPitchFromName(name, octave).pitch;
      this.octave = octave;
      this.name = name;
    }
    
    public int AbsPitch { get { return (octave * 12) + pitch; } }
  }
  public struct RealNote // Note with timing and accidentals
  {
    public Pitch pitch;
    public float startTime;
    public float duration;
    public RealNote(int octave, int pitch, float startTime, float duration)
    {
      this.pitch = new Pitch(pitch, octave);
      this.startTime = startTime;
      this.duration = duration;
    }
    public RealNote(Pitch pitch, float startTime, float duration)
    {
      this.pitch = pitch;
      this.startTime = startTime;
      this.duration = duration;
    }
  }
  public class PitchUtils
  {
    private static readonly PitchName[] pitchNameMap = 
    {
      PitchName.C,
      PitchName.C,
      PitchName.D,
      PitchName.D,
      PitchName.E,
      PitchName.F,
      PitchName.F,
      PitchName.G,
      PitchName.G,
      PitchName.A,
      PitchName.A,
      PitchName.B
    };
    public static Pitch GetPitchFromName(PitchName name, int octave, Alteration alteration = Alteration.NATURAL)
    {
      switch (name)
      {
        case PitchName.C:
          return new Pitch(0 + (int)alteration, octave);
        case PitchName.D:
          return new Pitch(2 + (int)alteration, octave);
        case PitchName.E:
          return new Pitch(4 + (int)alteration, octave);
        case PitchName.F:
          return new Pitch(5 + (int)alteration, octave);
        case PitchName.G:
          return new Pitch(7 + (int)alteration, octave);
        case PitchName.A:
          return new Pitch(9 + (int)alteration, octave);
        case PitchName.B:
          return new Pitch(11 + (int)alteration, octave);
        default:
          throw new ArgumentOutOfRangeException("Invalid PitchName");
      }
    }
    public static Pitch GetPitchFromInterval(Pitch basePitch, int interval)
    {
      PitchName newName = (PitchName)(((int)basePitch.name + interval) % 7);
      int octave = basePitch.octave + ((basePitch.pitch + interval) / 12);
      return new Pitch(newName, octave);
    }
    public static PitchName GetPitchName(int pitch)
    {
      // TODO: This is naive, needs to account for accidentals
      return pitchNameMap[pitch];
    }
    public static Pitch GetPitchFromInterval(Pitch basePitch, int interval, Scale scale)
    {
      Pitch pitch = GetPitchFromInterval(basePitch, interval);
      return scale.GetDiatonicPitch(pitch);
    }
  }
}