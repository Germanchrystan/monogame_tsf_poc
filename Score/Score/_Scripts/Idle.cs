using System;
using tsybulya.Components.States;
using tsybulya.Scripts;
using tsybulya.structures;

namespace entities.score;
public class Idle: Script
{
  override public void Start()
  {
  }
  override public void HandleMessage(object sender, Message message)
  {
    if (message.Name == "PLAY")
    {
      SetState(new StateRequest() { state = Constants.PLAYING, substate = "" });
    }
  }
}