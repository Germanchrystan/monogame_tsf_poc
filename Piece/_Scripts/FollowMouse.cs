using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using tsybulya.Scripts;
using tsybulya.Components.States;
using tsybulya.Components.GraphicComponents;
using tsybulya.structures;
using tsybulya.GameObjects;

using tsybulya.Singletons;

namespace entities.piece
{
  public class FollowMouse : Script
  {
    ButtonState currentState;
    Action<Vector2> PlacePieceFromConnector;
    private SingleFrameRenderer singleFrameRenderer = null;
    float mouseXOffset;
    float mouseYOffset;
    public override void Initialize()
    {
      mouseXOffset = Transform.Size.X / 2;
      mouseYOffset = Transform.Size.Y / 2;
      singleFrameRenderer = GetComponent<SingleFrameRenderer>();
      PlacePieceFromConnector = GetReference<Action<Vector2>>("PlacePieceFromConnector");
    }
    public override void Start()
    {
      singleFrameRenderer.Alpha = 0.25f;
    }
    public override void Update(float dt)
    {
      followMouse();
    }
    private void followMouse()
    {
      MouseState mouseState = Mouse.GetState();
      currentState = mouseState.LeftButton;
      if (currentState == ButtonState.Released)
      {
        SetState(new StateRequest() { state = PIECE_CONSTANTS.IDLE_STATE });

      }
      this.Transform.SetPosition(new Vector2(MouseInput.MousePosition.X - mouseXOffset, MouseInput.MousePosition.Y - mouseYOffset));
    }
    public override void HandleMessage(object sender, Message message)
    {
      if (message.Name == "COLLIDED_WITH_STAFF_CONNECTOR") // Get message from PieceEnd
      {
        float[] parts = Array.ConvertAll(message.Metadata.Split(','), float.Parse);
        PlacePieceFromConnector(new Vector2(parts[0], parts[1]));
        this.SetState(new StateRequest() { state = PIECE_CONSTANTS.ATTACHED_STATE });
      }
    }
  }
}