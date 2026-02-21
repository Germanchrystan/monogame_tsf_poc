using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using tsybulya.Components.Collisions;
using tsybulya.Scripts;
using tsybulya.structures;
using tsybulya.Singletons.CollisionChecker;
using tsybulya.Components.States;
using tsybulya.Components.GraphicComponents;

namespace entities.piece
{
public struct PiecePlacementMetadata
{
    [JsonPropertyName("noteData")]
    public string NoteData { get; set; }

    [JsonPropertyName("x")]
    public float X { get; set; }

    [JsonPropertyName("y")]
    public float Y { get; set; }
}
  public class PlacedNotifier : Script
  {
    RightPieceEnd rightPieceEnd;
    LeftPieceEnd leftPieceEnd;
    CollisionBox leftPieceCollision;
    SingleFrameRenderer singleFrameRenderer;
    CollisionBox connectorCollisionBox;
    public override void Initialize()
    {
      singleFrameRenderer = GetComponent<SingleFrameRenderer>();
      rightPieceEnd = GetChildren<RightPieceEnd>()[0];
      leftPieceEnd = GetChildren<LeftPieceEnd>()[0];
    }
    public override void Start()
    {
      singleFrameRenderer.Alpha = 0f;
      try
      {
        leftPieceCollision = leftPieceEnd.GetComponent<CollisionBox>();
        connectorCollisionBox = CollisionChecker.CheckCollisionOnDemand(leftPieceCollision, "STAFF_CONNECTOR")[0];
        string metadata = JsonSerializer.Serialize(new PiecePlacementMetadata() { NoteData = "note", X = rightPieceEnd.Transform.Position.X, Y = rightPieceEnd.Transform.Position.Y });
        SendMessage(connectorCollisionBox, this, 1, "PIECE_PLACED", metadata);
      }
      catch (Exception e)
      {
        Console.WriteLine($"No staff connected {e.Message}");
        Console.WriteLine($"{e.StackTrace}");
      }
    }
    public override void HandleMessage(object sender, Message message)
    {
      if (message.Name == "SELECTED")
      {
        try
        {
        string metadata = JsonSerializer.Serialize(new PiecePlacementMetadata() { NoteData = "note", X = leftPieceEnd.Transform.Position.X, Y = leftPieceEnd.Transform.Position.Y });
        SendMessage(connectorCollisionBox, this, 1, "PIECE_REMOVED", metadata); // FIX THISSS
        this.SetState(new StateRequest() { state = PIECE_CONSTANTS.SELECTED_STATE });
        }
        catch (Exception e)
        {
          Console.WriteLine(e.Message);
          Console.WriteLine(e.StackTrace);
        }
      }
    }
  }
}