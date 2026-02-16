
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using POVMidiPlayer;

namespace Renderer;

public class VideoVisualizer
{
  private MidiEvent[] midiEvents = Array.Empty<MidiEvent>();
  private NoteVisualizer[] notes = Array.Empty<NoteVisualizer>();
  private Rectangle mainRect = new Rectangle(400, 300, 100, 50);
  private Texture2D blankTexture;
  public bool On { get; set; } = true;
  private int centerNote = 71;

  public VideoVisualizer(MidiEvent[] midiEvents, GraphicsDevice graphicsDevice)
  {
    blankTexture = new Texture2D(graphicsDevice, 1, 1);
    Color[] data = { Color.White };
    blankTexture.SetData(data);

    this.midiEvents = midiEvents;
    InitializeNotes();
  }
  private void InitializeNotes()
  {
    List<NoteVisualizer> noteList = new List<NoteVisualizer>();
    foreach (MidiEvent midiEvent in midiEvents)
    {
      if (midiEvent.IsNoteOn)
      {
        float startTime = midiEvent.Time;
        float endTime = FindNoteOffTime(midiEvent);
        Rectangle noteRect = SetNotePosition(midiEvent.Key, startTime);
        noteList.Add(new NoteVisualizer(midiEvent.Key, startTime, endTime, noteRect, ref blankTexture));
      }
    }
    notes = noteList.ToArray();
  }
  private float FindNoteOffTime(MidiEvent midiEvent)
  {
    int i = Array.IndexOf(midiEvents, midiEvent) + 1;
    while (i < midiEvents.Length)
    {
      if (!midiEvents[i].IsNoteOn && midiEvents[i].Key == midiEvent.Key && midiEvents[i].Channel == midiEvent.Channel)
      {
        return midiEvents[i].Time;
      }
      i++;
    }
    return midiEvent.Time + 1f; // Default to 1 second after note-on if no note-off is found
  }
  private Rectangle SetNotePosition(int key, float startTime)
  {
    int noteOffset = centerNote - key;
    int y = mainRect.Y + mainRect.Height / 2 + noteOffset * 10;
    int x = mainRect.X + (int)(startTime * 100); // Scale time to pixels
    return new Rectangle(x, y, 10, 10);
  }
  public void Update(float currentTime)
  {
    foreach (NoteVisualizer note in notes)
    {
      note.Update(currentTime);
    }
  }
  public void Render(SpriteBatch spriteBatch)
  {
    spriteBatch.Draw(blankTexture, mainRect, Color.Wheat);
    foreach (NoteVisualizer note in notes)
    {
      note.Render(spriteBatch);
    }
  }
}

public class NoteVisualizer
{
  private int note;
  public int Note { get { return note; } }
  public float startTime;
  public float endTime;
  public Rectangle rectangle;
  public bool IsPlaying { get; set; } = false;
  private Texture2D blankTexture;
  public NoteVisualizer(int note, float startTime, float endTime, Rectangle rectangle, ref Texture2D blankTexture)
  {
    this.note = note;
    this.startTime = startTime;
    this.endTime = endTime;
    this.rectangle = rectangle;
    this.blankTexture = blankTexture;
  }
  public void Update(float deltaTime)
  {
    if (deltaTime >= startTime && deltaTime < endTime)
    {
      Console.WriteLine($"Note {note} is playing at time {deltaTime}");
      IsPlaying = true;
      return;
    }

    Console.WriteLine($"Note {note} is not playing at time {deltaTime}");
    IsPlaying = false;
  }
  public void Render(SpriteBatch spriteBatch)
  {
    Color color = IsPlaying ? Color.Green : Color.Red;
    spriteBatch.Draw(blankTexture, rectangle, color);
  }
}