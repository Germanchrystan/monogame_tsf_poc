using System;
using Microsoft.Xna.Framework;

using tsybulya.GameObjects;
using tsybulya.Components.States;
using tsybulya.Singletons.Graphics;
using tsybulya.Components.Collisions;
using tsybulya.Components.GraphicComponents;

using entities.piece;
using notation.note;
using notation.clef;
using notation.music_file;
using POVMidiPlayer;

namespace entities.score
{
  public class StaffLine : GameObject
  {
    Rectangle newRect = new Rectangle(0, 0, 8, 8);

    public StaffLine(Vector2 position, Vector2 size) : base(position, size, 0)
    {
      AddComponent(
        new SingleFrameRenderer(
          new FrameData()
          {
            flipX = false,
            rotation = 0,
            rect = newRect,
            texture = GraphicManager.WhiteTexture
          },
          Color.Black, 90, Transform
        ));
    }
  } // END StaffLine class
  public class Staff : GameObject
  {
    private Register register;
    public Register Register { get { return register; } }
    private int registerId;
    public int RegisterId { get { return registerId;  } }
    private StaffPieceList placedPieces;
    private Clef clef;
    private float spaceBetweenLines;
    private float marginsBetweenLines;
    // TODO: Stop using size. Get size.x from totalTickDuration and spaceBetweenLines. Get size.y from spaceBetweenLines and/or marginsBetweenLines.
    public Staff(Vector2 position, Vector2 size, float totalTickDuration) : base(position, size, 0)
    {
      this.spaceBetweenLines = size.Y / 5;
      this.marginsBetweenLines = spaceBetweenLines / 2;
      this.placedPieces = new StaffPieceList(totalTickDuration);
      createLines();
      AddScripts([new PiecePlacement()]);
      AddState(new State("IDLE", [], ["PiecePlacement"]));
      AddComponent(new CollisionBox("STAFF", ScriptList, Transform, ReceiveMessage));
      AddChildren(new StaffConnector(new Vector2(Transform.Position.X, Transform.Position.Y + Transform.Size.Y / 2 - 4), size.Y));
      AddComponent(
         new SingleFrameRenderer(
          new FrameData()
          {
            flipX = false,
            rotation = 0,
            rect = new Rectangle(0, 0, 8, 8),
            texture = GraphicManager.WhiteTexture
          },
          Color.Wheat, 80, Transform
        ));
      SetReference("placedPieces", placedPieces);
    }
    private void createLines()
    {
      float linePositionX = this.Transform.Position.X;
      float linePositionY = Transform.Position.Y + marginsBetweenLines;
      for (int i = 0; i < 5; i++)
      {
        GameObject line = new StaffLine(new Vector2(linePositionX, linePositionY), new Vector2(Transform.Size.X, 1));
        AddChildren(line);
        linePositionY += this.spaceBetweenLines;
      }
    }
    public MidiEvent[] GetMidiEvents(float bpm)
    {
      return placedPieces.GetMidiEvents(clef.CentralNote, bpm);
    }
    public RealNote[] GetNotes()
    {
      return placedPieces.GetNotes(clef.CentralNote);
    }
    public Staff SetClef(Clef clef)
    {
      this.clef = clef;
      return this;
    }
    public Staff AddFixedPieces(BasePiece[] fixedPieces)
    {
      StaffConnector staffConnector = (StaffConnector)GetChildren<StaffConnector>()[0];
      try
      {
        placedPieces.Initialize(fixedPieces, staffConnector);
        foreach (BasePiece piece in fixedPieces)
        {
          AddChildren(piece);
        }
      }
      catch (Exception e)
      {
        Console.WriteLine("Error adding fixed pieces to staff: " + e.Message);
      }

      return this;
    }
    public Staff SetRegister(int registerId, Register register)
    {
      this.registerId = registerId;
      this.register = register;

      return this;
    }
    // Static factory methods
    public static Staff GetTrebleStaff(Vector2 position, Vector2 size, float tickDuration)
    {
      return new Staff(position, size, tickDuration).SetClef(Clef.TrebleClef());
    }
    public static Staff GetBassStaff(Vector2 position, Vector2 size, float tickDuration)
    {
      return new Staff(position, size, tickDuration).SetClef(Clef.BassClef());
    }
  }
}