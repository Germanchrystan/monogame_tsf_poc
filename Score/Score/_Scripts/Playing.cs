using System.Collections.Generic;
using entities.score;
using midi;
using Renderer;
using tsybulya.Scripts;
using tsybulya.structures;

public class Playing: Script
{
  private List<MidiEvent> midiEvents;
  private List<NoteSlotRef> noteSlotRefs;
  private float elapsedTime = 0f;
  override public void Start()
  {
    midiEvents = GetReference<List<MidiEvent>>("midiEvents");
    noteSlotRefs = GetReference<List<NoteSlotRef>>("noteSlotRefs");
    elapsedTime = 0f;
    AudioRenderer.LoadMidi(midiEvents.ToArray());
    
    // AudioRenderer.ToggleAudio();
  }
  public override void Update(float dt)
  {
    elapsedTime += dt;
    noteSlotRefs[0].NoteSlot.TurnOn();
  }
  override public void HandleMessage(object sender, Message message)
  {
    if (message.Name == "PLAY")
    {
    }
  }
}