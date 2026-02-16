using System;
using System.Runtime.InteropServices;

public static class Tsf
{
  public struct TSF{}

  private const string DllName = "libtsf.dylib";
  [DllImport("tsf", CallingConvention = CallingConvention.Cdecl)]
  public static extern int tsf_test_function();
  [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
  public static extern void tsf_channel_set_bank(IntPtr tsf, int channel, int bank);
  [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
  public static extern void tsf_channel_set_presetnumber(IntPtr tsf, int channel, int preset, int mididrums);
  [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
  public static extern IntPtr tsf_load_filename(string filename);
  [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
  public static extern void tsf_close(IntPtr tsf);
  [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
  public static extern void tsf_note_on(IntPtr tsf, int channel, int key, float vel);
  [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
  public static extern void tsf_note_off(IntPtr tsf, int channel, int key);
  [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
  public static extern void tsf_render_float(IntPtr synth, float[] buffer, int numSamples, int flag);
  [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
  public static extern void tsf_render_short(IntPtr synth, short[] buffer, int numSamples, int flag);
  [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
  public static extern string tsf_bank_get_presetname(IntPtr tsf, int bank, int preset_number);
  [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
  public static extern int tsf_get_presetcount(IntPtr tsf, int bank);
  [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
  public static extern void tsf_set_volume(IntPtr tsf, float global_gain);
  [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
  public static extern void tsf_set_output(IntPtr tsf, int outputmode, int samplerate, float global_gain_db);

}

public class Player : IDisposable
{
  private IntPtr synth;
  private int channelCount = 2; // Stereo

  public Player(string soundFontPath)
  {
    
    this.synth = Tsf.tsf_load_filename(soundFontPath);
    if (this.synth == IntPtr.Zero)
    {
      throw new Exception("Failed to load sound font.");
    }
  }

  public void NoteOn(int channel, int key, float velocity)
  {
    Tsf.tsf_note_on(this.synth, channel, key, velocity);
  }
  public void NoteOff(int channel, int key)
  {
    Tsf.tsf_note_off(this.synth, channel, key);
  }
  public void Render(short[] synthBuffer)
  {
    Tsf.tsf_render_short(this.synth, synthBuffer, synthBuffer.Length / channelCount, 0);
  }
  public void ProgramSelect(int channel, int bank, int preset, bool isDrum = false)
  {
    Tsf.tsf_channel_set_bank(this.synth, channel, bank);
    Tsf.tsf_channel_set_presetnumber(this.synth, channel, preset, isDrum ? 1 : 0);
  }
  public string GetPresetName(int bank, int preset_number)
  {
    return Tsf.tsf_bank_get_presetname(this.synth, bank, preset_number);
  }
  public int GetPresetCount(int bank)
  {
    return Tsf.tsf_get_presetcount(this.synth, bank);
  }
  public void SetVolume(float global_gain)
  {
    Tsf.tsf_set_volume(this.synth, global_gain);
  }
  public void SetOutput(int outputmode, int samplerate, float global_gain_db)
  {
    Tsf.tsf_set_output(this.synth, outputmode, samplerate, global_gain_db);
  }
  public void Dispose()
  {
    Tsf.tsf_close(this.synth);
  }
}