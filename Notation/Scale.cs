using System;
using System.Linq;
using notation.note;

namespace notation.scale
{

  public struct Scale
  {
    public Alteration[] alterations = new Alteration[7];
    public Scale(Alteration C, Alteration D, Alteration E, Alteration F, Alteration G, Alteration A, Alteration B)
    {
      alterations = [C,D,E,F,G,A,B];
    }
    public Pitch GetDiatonicPitch(Pitch pitch)
    {
      if (NotationConstants.ALTERED_PITCHS.Contains(pitch.pitch))
      {
        Console.WriteLine($"Pitch is already altered");
        return pitch;
      }

      pitch.pitch = (pitch.pitch + (int)alterations[(int)pitch.name]) % 12;
      return pitch;
    }
    public static Scale CMajor() { return new Scale(Alteration.NATURAL, Alteration.NATURAL, Alteration.NATURAL, Alteration.NATURAL, Alteration.NATURAL, Alteration.NATURAL, Alteration.NATURAL); }

  }
  public class NotationConstants
  {
    public static int[] ALTERED_PITCHS = [1, 3, 6, 8, 10];
  }
}