using System.Runtime.InteropServices;
using notation.note;

namespace notation.music_file
{
  [StructLayout(LayoutKind.Sequential)]
  public struct Register
  {
    public string name;
    public Pitch higher;
    public Pitch lower;
  }
  public struct Voice
  {
    public int id;
    public Register register;
    public RealNote[] notes;
  }
  [StructLayout(LayoutKind.Sequential)]
  public struct TimeSignature
  {
    public int numerator;
    public int denominator;
  }
  public struct MusicFile
  {
    public Voice[] voices;
    public TimeSignature timeSignature;
    public int bpm;
  }
}