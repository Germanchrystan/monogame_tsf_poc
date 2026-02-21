using Microsoft.Xna.Framework;

using tsybulya.Scenes;
using tsybulya.Components.GraphicComponents;
using tsybulya.Singletons.Graphics;
using entities.score;
using notation.music_file;

namespace fugue.scenes.level
{
  public class Level : Scene
  {
    private Score score;
    FrameData frameData = new FrameData()
    {
      flipX = false,
      rotation = 0,
      rect = new Rectangle(0, 0, 8, 8),
      texture = GraphicManager.BlackTexture
    };
    public Level(Score score)
    {
      this.score = score;
      AddChildren(score);
    }
    public MusicFile GetFile()
    {
      return this.score.GetMusicFile();
    }
  }
}