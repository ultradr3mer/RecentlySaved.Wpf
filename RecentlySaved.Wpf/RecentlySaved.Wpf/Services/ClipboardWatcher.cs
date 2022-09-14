using Prism.Events;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.Events;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;

namespace AdvancedClipboard.Wpf.Services
{
  public class ClipboardWatcher
  {
    private class NativeMethods
    {
      [DllImport("user32.dll")]
      public static extern IntPtr GetForegroundWindow();

      [DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
      public static extern int GetWindowThreadProcessId(IntPtr windowHandle, out int processId);

      [DllImport("user32.dll")]
      public static extern int GetWindowText(int hWnd, StringBuilder text, int count);
    }

    private readonly ClipboardChangedEvent clipboardChanged;
    private string lastText = string.Empty;

    public ClipboardWatcher(IEventAggregator eventAggregator)
    {
      this.clipboardChanged = eventAggregator.GetEvent<ClipboardChangedEvent>();
    }

    internal void Notify()
    {
      if (Clipboard.ContainsText())
      {
        string text = Clipboard.GetText(TextDataFormat.Text);
        if (text == lastText)
        {
          return;
        }

        string processName = this.GetForegroundProcessName();
        ClipData data = new ClipData() { Content = text, ProcessName = processName, Datum = DateTime.Now };
        this.clipboardChanged.Publish(new ClipboardChangedData() { Data = data });
        lastText = text;
      }
    }

    private string GetForegroundProcessName()
    {
      IntPtr activeWindowId = NativeMethods.GetForegroundWindow();
      if (activeWindowId.Equals(0))
      {
        return string.Empty;
      }

      NativeMethods.GetWindowThreadProcessId(activeWindowId, out int processId);
      if (processId == 0)
      {
        return string.Empty;
      }

      Process process = Process.GetProcessById(processId);
      return process.ProcessName;
    }

    internal void PutOntoClipboard(ClipData clipData)
    {
      Clipboard.SetText(clipData.Content);
    }
  }
}
