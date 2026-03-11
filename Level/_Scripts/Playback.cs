
using System.Collections.Generic;
using entities.piece;
using entities.score;
using tsybulya.Scripts;

public class PlayBack: Script
{
  private Score score;
  override public void Initialize()
  {
    score = GetChildren<Score>()[0];
  }
  override public void Start()
  {
    SendMessage(score, null, 1, "PLAY", "");
  }
  override public void Update(float deltaTime)
  {
    
  }
}