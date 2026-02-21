using System;
using Microsoft.Xna.Framework;
using tsybulya.Scripts;
using tsybulya.structures;
using entities.piece;

namespace entities.score
{
  public class PiecePlacement : Script
  {
    StaffPieceList placedPieces;
    StaffConnector staffConnector;
    Vector2 initialConnectorPosition;
    public override void Initialize()
    {
      placedPieces = (StaffPieceList)GetReferenceByName("placedPieces");
      staffConnector = GetChildren<StaffConnector>()[0];
      initialConnectorPosition = staffConnector.Transform.Position;
    }
    public override void Update(float dt) { }
    public override void HandleMessage(object sender, Message message)
    {
      Piece piece = (Piece)message.Sender;
      if (message.Name == "PIECE_PLACED")
      {
        try
        {
          placedPieces.AddPiece(piece, staffConnector);
        }
        catch (Exception e)
        {
          SendMessage(piece, this, 1, "RESTART_POSITION", new String(""));
          Console.WriteLine("Error placing piece on staff: " + e.Message);
          return;
        }
        staffConnector.MoveToRightPieceEnd(piece);
      }
      if (message.Name == "PIECE_REMOVED")
      {
        int pieceIndex = placedPieces.FindIndex(piece);
        for (int i = placedPieces.Count - 1; i >= pieceIndex; i--)
        {
          BasePiece p = placedPieces.GetAtIndex(i);
          if (!(p is Piece)) continue;
          SendMessage(p, this, 1, "RESTART_POSITION", new String(""));
          placedPieces.Remove(p);
        }
        placedPieces.Remove(piece);

        if (placedPieces.Count > 0)
        {
          BasePiece lastPiece = placedPieces.GetAtIndex(placedPieces.Count - 1);
          // if (lastPiece is Piece == false) return; // TODO: Revise this
          RightPieceEnd rightPieceEnd = (RightPieceEnd)lastPiece.GetChildren<RightPieceEnd>()[0];
          staffConnector.Transform.Position = new Vector2(rightPieceEnd.Transform.Position.X, rightPieceEnd.Transform.Position.Y);
          staffConnector.TickPosition -= lastPiece.GetTickDuration();
        }
        else
        {
          staffConnector.Transform.Position = initialConnectorPosition;
        }
      }
    }
  }
}