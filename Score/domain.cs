using entities.piece;

namespace entities.score;
public struct NoteSlotRef
{
  public NoteSlot NoteSlot;
  public float TimePosition;
  public bool NoteEvent;
}

public class Constants
{
  public const string IDLE = "Idle";
  public const string PLAYING = "Playing";
}