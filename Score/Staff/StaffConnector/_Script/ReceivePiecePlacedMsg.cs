using tsybulya.Scripts;
using tsybulya.structures;

namespace entities.score
{
  public class ReceivePiecePlacedMsg : Script
  {
    public override void Initialize()
    {
      
    }
    public override void HandleMessage(object sender, Message message)
    {
      if (message.Name == "PIECE_PLACED" || message.Name == "PIECE_REMOVED")
      {
        SendMessageToParent(message);
      }
    }
  }
}