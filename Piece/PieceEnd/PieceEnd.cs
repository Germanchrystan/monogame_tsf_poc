using entities.piece_end;
using Microsoft.Xna.Framework;
using tsybulya.Components.Collisions;
using tsybulya.Components.GraphicComponents;
using tsybulya.Components.States;
using tsybulya.GameObjects;
using tsybulya.Singletons.Graphics;

namespace entities.piece
{
  public class PieceEnd : GameObject
  {
    FrameData frameData = new FrameData() { flipX = false, rotation = 0, rect = new Rectangle(8, 8, 16, 32), texture = GraphicManager.GetTexture("white") };
    State idle = new State("IDLE", [], ["CheckCollision"]);
    public PieceEnd(Vector2 position, string colliderTag) : base(position, new Vector2(8, 8), 0)
    {
      Transform = new Transform(position, new Vector2(5, 5), 0);
      SingleFrameRenderer renderer = new SingleFrameRenderer(frameData, Color.Yellow, 120, Transform);
      renderer.Alpha = 0.1f;
      AddComponent(new CollisionBox(colliderTag, ScriptList, Transform, ReceiveMessage));
      AddComponent(renderer);
      AddState(idle);
      AddScripts([new CheckCollision()]);
    }
  }
  public class LeftPieceEnd : PieceEnd
  {
    public LeftPieceEnd(Vector2 position) : base(position, "PIECE_END_LEFT") { }
  }
  
  public class RightPieceEnd : PieceEnd
  {
    public RightPieceEnd(Vector2 position) : base(position, "PIECE_END_RIGHT") { }
  }
}