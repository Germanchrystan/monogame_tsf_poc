using System;
using entities.score;
using Microsoft.Xna.Framework.Input;
using Renderer;
using tsybulya.Scripts;

public class Idle: Script
{
  private bool isSpacePressed = false;
  private Score score;
  override public void Initialize()
  {
    score = GetChildren<Score>()[0];
  }
  public override void Update(float dt)
  {
    KeyboardState kState = Keyboard.GetState();
    bool isPreviouslyPressed = isSpacePressed;
    isSpacePressed = kState.IsKeyDown(Keys.Space);
    if (!isPreviouslyPressed && isSpacePressed)
    {
      SendMessage(score, null, 1, "PLAY", "");
      // AudioRenderer.ToggleAudio();
    }
  }
}