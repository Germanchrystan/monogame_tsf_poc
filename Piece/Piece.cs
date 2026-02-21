using System;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using tsybulya.Components.Collisions;
using tsybulya.Components.GraphicComponents;
using tsybulya.Components.States;
using tsybulya.Singletons.Graphics;

using entities.piece_end;

namespace entities.piece
{
  public class Piece : BasePiece
  {
    private const float INITIAL_MOVING_PIECE_POSITION = -1f;
    State idle = new State(PIECE_CONSTANTS.IDLE_STATE, [PIECE_CONSTANTS.SELECTED_STATE], ["CheckClicked"]);
    State selected = new State(PIECE_CONSTANTS.SELECTED_STATE, [PIECE_CONSTANTS.IDLE_STATE, PIECE_CONSTANTS.ATTACHED_STATE], ["FollowMouse"]);
    State attached = new State(PIECE_CONSTANTS.ATTACHED_STATE, [PIECE_CONSTANTS.SELECTED_STATE, PIECE_CONSTANTS.PLACED_STATE], ["Attached"]);
    State placed = new State(PIECE_CONSTANTS.PLACED_STATE, [PIECE_CONSTANTS.IDLE_STATE, PIECE_CONSTANTS.SELECTED_STATE], ["PlacedNotifier", "RestartPosition"]);
    FollowMouse followMouse = new FollowMouse();
    CheckClicked checkClicked = new CheckClicked();
    Attached attachedScript = new Attached();
    PlacedNotifier placedNotifier = new PlacedNotifier();
    RestartPosition restartPosition = new RestartPosition();
    FrameData frameData = new FrameData() { flipX = false, rotation = 0, rect = new Rectangle(0, 0, 16, 32), texture = GraphicManager.WhiteTexture };
    public Piece(Vector2 position, float scale, IntervalData[] intervalData) : base(scale, intervalData, INITIAL_MOVING_PIECE_POSITION)
    {
      this.Transform.SetPosition(position);
      SetReference("InitialPosition", position);
      SetReference("PlacePieceFromConnector", new Action<Vector2>(PlacePieceFromConnector));
      AddComponent(new CollisionBox("PIECE", ScriptList, Transform, ReceiveMessage));
      AddScripts([followMouse, checkClicked, attachedScript, placedNotifier, restartPosition]);
      // AddComponent(new SingleFrameRenderer(frameData, new Color(Color.Brown, 0.5f), 100, Transform));
      AddState(idle);
      AddState(selected);
      AddState(attached);
      AddState(placed);
    }
  }
}