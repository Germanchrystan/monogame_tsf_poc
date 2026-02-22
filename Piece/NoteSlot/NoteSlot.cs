using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using tsybulya.GameObjects;
using tsybulya.Components.GraphicComponents;
using tsybulya.Singletons.Graphics;

namespace entities.piece
{
  public class NoteSlot: GameObject
  {
    // TODO: This framedata could be a constant
    FrameData frameData = new FrameData() { flipX = false, rotation = 0, rect = new Rectangle(0, 0, 16, 32), texture = GraphicManager.WhiteTexture };
    public NoteSlot(Vector2 position, float scale): base(position, new Vector2(4, 4), 0)
    {
      this.Transform = new Transform(position, new Vector2(scale, scale), 0);
      AddComponent(new SingleFrameRenderer(frameData, Color.Black, 200, Transform));
    }
    public void TurnOn()
    {
      GetComponent<SingleFrameRenderer>().Color = Color.Red;
    }
    public void TurnOff()   
    {
      GetComponent<SingleFrameRenderer>().Color = Color.Black;
    } 
  }
}