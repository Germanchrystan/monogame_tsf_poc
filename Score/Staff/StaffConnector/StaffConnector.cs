using tsybulya.GameObjects;
using Microsoft.Xna.Framework;

using tsybulya.Components.Collisions;
using tsybulya.Components.GraphicComponents;
using tsybulya.Singletons.Graphics;
using tsybulya.Components.States;

using entities.piece;

namespace entities.score
{
  public class StaffConnector : GameObject
  {
    static Vector2 defaultSize = new Vector2(5, 5);
    private float tickPosition = 0;
    public float TickPosition { get { return tickPosition; } set { tickPosition = value; } }
    private ReceivePiecePlacedMsg receivePiecePlacedMsg = new ReceivePiecePlacedMsg();
    private State state = new State("IDLE", [], ["ReceivePiecePlacedMsg"]);
    public StaffConnector(Vector2 position, float staffSizeY) : base(position, defaultSize, 0)
    {
      float scale = staffSizeY / 10;
      Transform = new Transform(new Vector2(position.X, position.Y + scale / 2), new Vector2(scale, scale), 0);
      AddComponent(new CollisionBox("STAFF_CONNECTOR", ScriptList, Transform, ReceiveMessage));
      AddComponent(new SingleFrameRenderer(
        new FrameData() { flipX = false, rotation = 0, rect = new Rectangle(0, 0, 16, 32), texture = GraphicManager.WhiteTexture },
        Color.Green, 1000, Transform
      ));
      AddState(state);
      AddScripts([receivePiecePlacedMsg]);
    }
    public void MoveToRightPieceEnd(BasePiece piece)
    {
      RightPieceEnd rightPieceEnd = (RightPieceEnd)piece.GetChildren<RightPieceEnd>()[0];
      this.Transform.Position = new Vector2(rightPieceEnd.Transform.Position.X, rightPieceEnd.Transform.Position.Y);
      this.tickPosition += piece.GetTickDuration();
    }
  }
}