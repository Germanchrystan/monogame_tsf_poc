using System;
using tsybulya.Scripts;

namespace entities.score
{
  public class PlayingAudio : Script
  {
    StaffPieceList placedPieces;
    private float timeSinceStart = 0;
    private int currentIntervalIndex = 0;
    private int currentPieceIndex = 0;
    public override void Initialize()
    {
      placedPieces = (StaffPieceList)GetReferenceByName("placedPieces");
      foreach(NotePlacementRef notePlacementRef in placedPieces.NotePlacementRefs)
      {
        Console.WriteLine($"Note placement ref: {notePlacementRef.PieceIndex}, {notePlacementRef.IntervalIndex}, {notePlacementRef.TimePosition}, {notePlacementRef.Duration}");
      }
    }
    public override void Start()
    {
      timeSinceStart = 0;
    }
    public override void Update(float dt)
    {
      timeSinceStart += dt;
      if (timeSinceStart >= placedPieces.NotePlacementRefs[placedPieces.NotePlacementRefs.Count - 1].TimePosition + placedPieces.NotePlacementRefs[placedPieces.NotePlacementRefs.Count - 1].Duration)
      {
        timeSinceStart = 0;
        currentIntervalIndex = 0;
        currentPieceIndex = 0;
      }
      foreach (NotePlacementRef notePlacementRef in placedPieces.NotePlacementRefs)
      {
        if (timeSinceStart >= notePlacementRef.TimePosition && timeSinceStart < notePlacementRef.TimePosition + notePlacementRef.Duration)
        {
          // Console.WriteLine($"Playing note at time {notePlacementRef.PieceIndex}, {notePlacementRef.IntervalIndex}");
          placedPieces.PlayNote(notePlacementRef.PieceIndex, notePlacementRef.IntervalIndex);
        }
      }
    }
  }
}