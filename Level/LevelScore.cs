using Microsoft.Xna.Framework;
using entities.score;
using entities.piece;
using notation.interval;
using notation.music_file;
using notation.clef;

namespace fugue.level
{
  public class LevelScore
  {
    public static Score CreateSimpleScore()
    {
      IntervalData[] intervalData = new IntervalData[]
      {
        new IntervalData(PitchInterval.SECOND, IntervalDirection.UP, 2),    // C
        new IntervalData(PitchInterval.SECOND, IntervalDirection.UP, 2),    // D
        new IntervalData(PitchInterval.SECOND, IntervalDirection.UP, 2)     // E
      };

      // Create a fixed piece with the three notes
      BasePiece fixedPiece = new BasePiece(
        staffSizeY: 100,
        intervalData: intervalData,
        tickPosition: 0
      );

      // Create a staff with the fixed piece
      Staff staff = new Staff(
        position: new Vector2(100, 200),
        size: new Vector2(500, 100),
        totalTickDuration: 12  // Total duration for all notes
      )
      .SetClef(Clef.TrebleClef())
      .AddFixedPieces(new BasePiece[] { fixedPiece });

      // Create and return a score with the staff
      Score score = new Score(
        staffs: new Staff[] { staff },
        timeSignature: new TimeSignature { numerator = 4, denominator = 4 },
        bpm: 120
      );

      return score;
    }
  }
}
