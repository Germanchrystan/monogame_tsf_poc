using System;
using notation.note;

namespace notation.clef
{
  public struct Clef
  {
    private int symbolId;
    public int SymbolId { get { return symbolId; } }
    private Pitch centralNote;
    public Pitch CentralNote { get { return centralNote; } }
    public Clef(int symbolId, Pitch centralNote)
    {
      this.symbolId = symbolId;
      this.centralNote = centralNote;
    }

    static public Clef TrebleClef() { return new Clef(1, new Pitch(11, 2)); } // B4
    static public Clef BassClef() { return new Clef(2, new Pitch(1, 1)); } // D3
  };
}