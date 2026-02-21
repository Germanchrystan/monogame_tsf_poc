using System;
using System.Linq;
using Microsoft.Xna.Framework;

using tsybulya.Components.GraphicComponents;
using tsybulya.GameObjects;
using tsybulya.Singletons.Graphics;
using notation.interval;

namespace entities.piece
{
  public struct IntervalData
  {
    public IntervalData(PitchInterval interval, IntervalDirection intervalDirection, int duration)
    {
      Interval = (int)interval * (int)intervalDirection;
      Duration = duration;
    }
    public int Interval;
    public int Duration;
  }
  public class BasePiece : GameObject
  {
    float scale;
    IntervalData[] intervalData;
    private float tickPosition = 0;
    public float TickPosition { get { return tickPosition; } set { tickPosition = value; } }
    public IntervalData[] IntervalData { get { return intervalData; } }
    public BasePiece(float staffSizeY, IntervalData[] intervalData, float tickPosition, bool addLeft = true, bool addRight = true) : base(new Vector2(0,0), new Vector2(8, 8), 0)
    {
      this.scale = staffSizeY / 10;
      Transform = new Transform(new Vector2(0,0), new Vector2(8, 8), 0);
      this.intervalData = intervalData;
      this.tickPosition = tickPosition;
      AddComponent(new SingleFrameRenderer(new FrameData() { flipX = false, rotation = 0, rect = new Rectangle(0, 0, 16, 32), texture = GraphicManager.WhiteTexture }, new Color(Color.Brown, 0.5f), 100, Transform));
      AddNodeSlots();
      AddPieceEnds();
    }
    // public RealNote[] GetNotes(Pitch lastPitch)
    // {
    //   List<RealNote> notes = new List<RealNote>();
    //   int startTime = 0;
    //   foreach (IntervalData data in intervalData)
    //   {
    //     Pitch newPitch = PitchUtils.GetPitchFromInterval(lastPitch, data.Interval);
    //     RealNote note = new RealNote(newPitch, startTime, data.Duration);
    //     notes.Add(note);
    //     startTime += data.Duration;
    //     lastPitch = newPitch;
    
    //   }
    //   return notes.ToArray();
    // }
    public float GetTickDuration()
    {
      float duration = 0;
      foreach (IntervalData data in intervalData)
      {
        duration += data.Duration;
      }
      return duration;
    }
    private void AddNodeSlots()
    {
      float pieceXSize = 0;
      float pieceYSize = this.Transform.Size.Y;
      float yPos = Transform.Position.Y + Transform.Size.Y / 2;
      Vector2 intervalNodePosition = new Vector2(Transform.Position.X - scale, yPos);
      float yUpperOverflow = 0;
      float yLowerOverflow = 0;
      for (int i = 0; i < intervalData.Count(); i++)
      {
        NoteSlot noteSlot = new NoteSlot(intervalNodePosition, scale);
        yPos += intervalData[i].Interval * (scale * (-1));
        if (yPos - noteSlot.Transform.Size.Y / 2 < Transform.Position.Y) yUpperOverflow = Math.Min(yPos - noteSlot.Transform.Size.Y / 2 - Transform.Position.Y, yUpperOverflow);
        if (yPos + noteSlot.Transform.Size.Y / 2 > Transform.Position.Y + Transform.Size.Y) yLowerOverflow = Math.Max(yPos + noteSlot.Transform.Size.Y / 2 - (Transform.Position.Y + Transform.Size.Y), yLowerOverflow);
        intervalNodePosition = new Vector2(intervalNodePosition.X, yPos);
        noteSlot.Transform.Position = new Vector2(intervalNodePosition.X, intervalNodePosition.Y + noteSlot.Transform.Size.Y / 2);
        AddChildren(noteSlot);
        intervalNodePosition = new Vector2(intervalNodePosition.X + (intervalData[i].Duration * scale), intervalNodePosition.Y);
        pieceXSize += intervalData[i].Duration * scale;
      }
      // Correcting overflow
      if (yUpperOverflow < 0)
      {
        foreach (NoteSlot noteSlot in GetChildren<NoteSlot>())
        {
          noteSlot.Transform.SetPosition(new Vector2(
            noteSlot.Transform.Position.X,
            noteSlot.Transform.Position.Y - yUpperOverflow
          ));
          float noteSlotY = noteSlot.Transform.Position.Y;
          float pieceBottom = Transform.Position.Y + Transform.Size.Y;
          if (noteSlotY > pieceBottom)
          {
            float increment = noteSlotY - pieceBottom;
            yLowerOverflow += increment + noteSlot.Transform.Size.Y * 1.5f; // TODO: why 1.5? Overflow padding
          }
        }
      }
      if (yLowerOverflow > 0) pieceYSize += yLowerOverflow / 2;

      Transform.Size = new Vector2(pieceXSize, pieceYSize);
    }
    private void AddPieceEnds(bool addLeftEnd = true, bool addRightEnd = true)
    {
      GameObject[] noteSlots = GetChildren<NoteSlot>();
      NoteSlot firstNoteSlot = (NoteSlot)noteSlots[0];
      NoteSlot lastNoteSlot = (NoteSlot)noteSlots[noteSlots.Count() - 1];
      float leftEndYPos = firstNoteSlot.Transform.Position.Y + intervalData[0].Interval * scale;
      if (addLeftEnd)
      {
        Vector2 leftEndPosition = new Vector2(Transform.Position.X - scale, leftEndYPos);
        AddChildren(new LeftPieceEnd(leftEndPosition), true, true);
      }
      if (addRightEnd)
      {
        Vector2 rightEndPosition = new Vector2(Transform.Position.X + Transform.Size.X - scale, lastNoteSlot.Transform.Position.Y);
        AddChildren(new RightPieceEnd(rightEndPosition), true, true);
      }
    }
    public void PlacePieceFromConnector(Vector2 connectorPosition)
    {
      LeftPieceEnd leftPieceEnd = (LeftPieceEnd)GetChildren<LeftPieceEnd>()[0];
      Transform pieceEndTransform = leftPieceEnd.Transform;
      Vector2 posDifferance = new Vector2(this.Transform.Position.X - pieceEndTransform.Position.X, this.Transform.Position.Y - pieceEndTransform.Position.Y);
      this.Transform.SetPosition(new Vector2(
        connectorPosition.X + posDifferance.X,
        connectorPosition.Y + posDifferance.Y
      ));
    }
  }
}