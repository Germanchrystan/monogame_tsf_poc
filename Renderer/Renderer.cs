using Microsoft.Xna.Framework.Graphics;
using midi;

namespace Renderer;

public class AVRenderer
{
  private const float EXTRA_TIME = 0.5f;
  AudioRenderer audio;
  VideoVisualizer visualizer;
  public float currentTime = 0f;
  public float duration = 0f;  
  public AVRenderer(string sf2Path, MidiEvent[] midiEvents, GraphicsDevice graphicsDevice)
  {
    audio = new AudioRenderer(sf2Path);
    audio.LoadPlayer();
    audio.LoadMidi(midiEvents);
    visualizer = new VideoVisualizer(midiEvents, graphicsDevice);
    this.duration = midiEvents[midiEvents.Length - 1].Time + EXTRA_TIME;
  }
  public void Update(float deltaTime)
  {
    currentTime += deltaTime;
    if (currentTime >= duration) currentTime = 0f;
    audio.Update(currentTime);
    visualizer.Update(currentTime);
  }
  public void Render(SpriteBatch spriteBatch)
  {
    visualizer.Render(spriteBatch);
  }
}