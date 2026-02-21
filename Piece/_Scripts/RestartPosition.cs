
using Microsoft.Xna.Framework;
using tsybulya.Components.States;
using tsybulya.Scripts;
using tsybulya.structures;

namespace entities.piece
{
  public class RestartPosition : Script
  {
    Vector2 initialPosition;
    public override void Initialize()
    {
      initialPosition = GetReference<Vector2>("InitialPosition");
    }
    public override void HandleMessage(object sender, Message message)
    {
      if (message.Name == "RESTART_POSITION")
      {
        this.Transform.SetPosition(initialPosition);
        SetState(new StateRequest() { state = PIECE_CONSTANTS.IDLE_STATE });
      }
    }
  }
}