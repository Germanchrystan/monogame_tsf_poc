using System;
using System.Runtime.InteropServices;

public static class tsf
{
  public struct TSF{}

  private const string DllName = "libtsf.dylib";
  [DllImport("tsf", CallingConvention = CallingConvention.Cdecl)]
  public static extern int tsf_test_function();

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
}

public class Player : IDisposable
{
  private IntPtr synth;
  private int channelCount = 2; // Stereo

  public Player(string soundFontPath)
  {
    
    this.synth = tsf.tsf_load_filename(soundFontPath);
    if (this.synth == IntPtr.Zero)
    {
      throw new Exception("Failed to load sound font.");
    }
  }

  public void NoteOn(int channel, int key, float velocity)
  {
    tsf.tsf_note_on(this.synth, channel, key, velocity);
  }

  public void NoteOff(int channel, int key)
  {
    tsf.tsf_note_off(this.synth, channel, key);
  }

  public void Render(float[] synthBuffer)
  {
    tsf.tsf_render_float(this.synth, synthBuffer, synthBuffer.Length/ channelCount, 0);
  }

  public void Dispose()
  {
    tsf.tsf_close(this.synth);
  }
}