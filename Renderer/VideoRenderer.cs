
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using POVMidiPlayer;

namespace Renderer;

public class VideoVisualizer
{
  private MidiEvent[] midiEvents = Array.Empty<MidiEvent>();
  private NoteVisualizer[] notes = Array.Empty<NoteVisualizer>();
  private Rectangle mainRect;
  private Texture2D blankTexture;
  public bool On { get; set; } = true;
  private int centerNote = 71;
  private int[] alteredNoteIndex = [1, 3, 6, 8, 10]; // C#, D#, F#, G#, A#
  public VideoVisualizer(MidiEvent[] midiEvents, GraphicsDevice graphicsDevice)
  {
    blankTexture = new Texture2D(graphicsDevice, 1, 1);
    Color[] data = { Color.White };
    blankTexture.SetData(data);

    this.midiEvents = midiEvents;
    InitializeMainRect();
    InitializeNotes();
  }
  private void InitializeMainRect()
  {
    int totalWidth = (int)(midiEvents[midiEvents.Length - 1].Time * 100) + 100; // Scale time to pixels and add extra space
    int totalHeight = 100;
    mainRect = new Rectangle(
      (int)(Constants.WINDOW_WIDTH / 2 - totalWidth / 2),
      (int)(Constants.WINDOW_HEIGHT / 2 - totalHeight / 2),
      totalWidth,
      totalHeight
    );
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
    int keyInt = alteredNoteIndex.Contains(key % 12) ? key - 1 : key; // Adjust for altered notes
    int noteOffset = (centerNote - keyInt) * (mainRect.Height / 12);
    int y = mainRect.Y + mainRect.Height / 2 + noteOffset;
    int x = mainRect.X + (int)(startTime * 100); // Scale time to pixels
    return new Rectangle(x, y - 5, 10, 10);
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
// ====================================================== //
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
      IsPlaying = true;
      return;
    }

    IsPlaying = false;
  }
  public void Render(SpriteBatch spriteBatch)
  {
    Color color = IsPlaying ? Color.Green : Color.Red;
    spriteBatch.Draw(blankTexture, rectangle, color);
  }
}