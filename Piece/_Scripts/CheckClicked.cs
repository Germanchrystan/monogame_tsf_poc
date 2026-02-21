using tsybulya.Components.States;
using tsybulya.Scripts;
using tsybulya.structures;
using tsybulya.Components.GraphicComponents;

namespace entities.piece
{
  public class CheckClicked : Script
  {
    private SingleFrameRenderer singleFrameRenderer = null;
    public override void Initialize()
    {
      singleFrameRenderer = GetComponent<SingleFrameRenderer>();
    }
    public override void Start()
    {
      singleFrameRenderer.Alpha = 0.5f;
    }
    public override void Update(float dt)
    {
    }
    public override void HandleMessage(object sender, Message message)
    {
      if (message.Name == "SELECTED")
      {
        this.SetState(new StateRequest() { state = PIECE_CONSTANTS.SELECTED_STATE });
      }
    }
  }
}