using System;
using System.Linq;
using System.Collections.Generic;

using entities.piece;
using notation.note;
using midi;

namespace entities.score
{
#nullable enable
  public class StaffPieceList
  {
    private float totalTickDuration;
    private List<BasePiece> placedPieces = new List<BasePiece>();
    public StaffPieceList(float totalTickDuration)
    {
      this.totalTickDuration = totalTickDuration;
    }
    public void Initialize(BasePiece[] fixedPieces, StaffConnector staffConnector)
    {
      float tickPosition = 0;
      placedPieces.Clear();

      fixedPieces = fixedPieces.OrderBy(p => p.TickPosition).ToArray();
      foreach (BasePiece piece in fixedPieces)
      {
        if (piece.TickPosition < tickPosition) throw new Exception("Pieces overlap at tick position " + piece.TickPosition);
        if (piece.TickPosition > tickPosition) tickPosition = piece.TickPosition;
      }
      foreach (BasePiece piece in fixedPieces)
      {
        try
        {
          AddPiece(piece, staffConnector);
        }
        catch (Exception e)
        {
          Console.WriteLine($"Error adding fixed piece to staff: {e.Message}");
          throw;
        }
      }
    }
    public void AddPiece(BasePiece newPiece, StaffConnector staffConnector)
    {
      newPiece.TickPosition = staffConnector.TickPosition;
      newPiece.PlacePieceFromConnector(staffConnector.Transform.Position);
      placedPieces.Add(newPiece);
      placedPieces.Sort((a, b) => a.TickPosition.CompareTo(b.TickPosition));
      try
      {
        int newPieceInt = placedPieces.FindIndex(x => x == newPiece);
        if (newPieceInt > 0)
        {
          BasePiece previousPiece = placedPieces[newPieceInt - 1];
          if (previousPiece.TickPosition + previousPiece.GetTickDuration() > newPiece.TickPosition)
          {
            throw new Exception($"Cannot insert piece at position {newPiece.TickPosition} because it would overlap with the previous piece.");
          }
        }
        if (newPieceInt + 1 < placedPieces.Count)
        {
          BasePiece nextPiece = placedPieces[newPieceInt + 1];
          if (newPiece.TickPosition + newPiece.GetTickDuration() > nextPiece.TickPosition)
          {
            throw new Exception($"Cannot insert piece at position {newPiece.TickPosition} because it would overlap with the next piece.");
          }
        }
      }
      catch (Exception e)
      {
        placedPieces.Remove(newPiece);
        Console.WriteLine("Error validating piece placement: " + e.Message);
        throw;
      }
      staffConnector.MoveToRightPieceEnd(newPiece);
    }
    public MidiEvent[] GetMidiEvents(Pitch initialPitch, float bpm) // Working with fixed bpm here
    {
      Pitch lastPitch = initialPitch;
      List<MidiEvent> midiEvents = new List<MidiEvent>();
      int index = 0;
      while (index < placedPieces.Count)
      {
        BasePiece piece = placedPieces[index];
        float tickPosition = piece.TickPosition; // Tick position is number of beats from the start of the staff
        float pieceTimePosition = tickPosition * (60f / bpm); // Convert tick position to time in seconds based on BPM
        float innerPieceTimePosition = pieceTimePosition; // This will track the time position within the piece as we iterate through its intervals
        foreach (IntervalData data in piece.IntervalData)
        {
          Pitch newPitch = PitchUtils.GetPitchFromInterval(lastPitch, data.Interval);
          float noteTimePosition = innerPieceTimePosition;
          midiEvents.Add(new MidiEvent()
          {
            Key = newPitch.AbsPitch,
            Time = noteTimePosition,
            Velocity = 1f,
            IsNoteOn = true,
          });
          midiEvents.Add(new MidiEvent()
          {
            Key = newPitch.AbsPitch,
            Time = noteTimePosition + (data.Duration * (60f / bpm)), // Note off event after the duration of the note
            Velocity = 0f,
            IsNoteOn = false,
          });
          lastPitch = newPitch;
          innerPieceTimePosition += data.Duration * (60f / bpm);
        }

        index++;
      }

      return midiEvents.ToArray();
    }
    public RealNote[] GetNotes(Pitch initialPitch)
    {
      Pitch lastPitch = initialPitch;
      List<RealNote> notes = new List<RealNote>();
      int index = 0;

      while (index < placedPieces.Count)
      {
        BasePiece piece = placedPieces[index];
        float tickPosition = piece.TickPosition;
        foreach (IntervalData data in piece.IntervalData)
        {
          Pitch newPitch = PitchUtils.GetPitchFromInterval(lastPitch, data.Interval);
          notes.Add(new RealNote(
            newPitch,
            tickPosition,
            data.Duration
          ));

          lastPitch = newPitch;
          tickPosition += data.Duration;
        }
        index++;
      }

      return notes.ToArray();
    }
    public bool IsComplete()
    {
      float tickPosition = 0;
      foreach (BasePiece piece in placedPieces)
      {
        if (tickPosition < piece.TickPosition) return false;
        tickPosition += piece.GetTickDuration();
        if (tickPosition == totalTickDuration) return true;
      }
      return false;
    }
    public int FindIndex(BasePiece piece)
    {
      return placedPieces.FindIndex(x => x == piece);
    }
    public void Remove(BasePiece piece)
    {
      placedPieces.Remove(piece);
    }
    public int Count
    {
      get { return placedPieces.Count; }
    }
    public BasePiece GetAtIndex(int index)
    {
      return placedPieces[index];
    }
  }
}