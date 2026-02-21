using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using tsybulya.Scripts;
using tsybulya.Components.States;
using tsybulya.Singletons;
using entities.piece;


namespace entities.piece_end
{
  public class Attached : Script
  {
    Vector2 mousePosition;
    Vector2 offset;
    float mouseXOffset;
    float mouseYOffset;

    public override void Start()
    {
      mouseXOffset = Transform.Size.X / 2;
      mouseYOffset = Transform.Size.Y / 2;
    }
    public override void Update(float dt)
    {
      mousePosition = MouseInput.MousePosition;
      Vector2 center = new Vector2(Transform.Position.X + Transform.Size.X / 2, Transform.Position.Y + Transform.Size.Y / 2);
      offset = new Vector2(center.X - mousePosition.X, center.Y - mousePosition.Y);

      if (Math.Abs(offset.X) > 16 || Math.Abs(offset.Y) > 16)
      {
        SetState(new StateRequest() { state = PIECE_CONSTANTS.SELECTED_STATE, substate = "" });
        this.Transform.SetPosition(new Vector2(mousePosition.X - mouseXOffset, mousePosition.Y - mouseYOffset));
      }

      if (Mouse.GetState().LeftButton == ButtonState.Released)
      {
        SetState(new StateRequest() { state = PIECE_CONSTANTS.PLACED_STATE, substate = "" });
      }
    }
  }
}