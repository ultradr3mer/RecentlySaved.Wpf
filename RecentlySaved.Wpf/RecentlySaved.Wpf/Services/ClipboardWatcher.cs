using Prism.Events;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.Events;
using System.Windows;

namespace AdvancedClipboard.Wpf.Services
{
  public class ClipboardWatcher
  {
    private ClipboardChangedEvent clipboardChanged;
    string lastText = string.Empty;

    public ClipboardWatcher(IEventAggregator eventAggregator)
    {
      this.clipboardChanged = eventAggregator.GetEvent<ClipboardChangedEvent>();
    }

    internal void Notify()
    {
      if (Clipboard.ContainsText())
      {
        string text = Clipboard.GetText(TextDataFormat.Text);
        if (text != lastText)
        {
          this.clipboardChanged.Publish(new ClipboardChangedData() { Data = new ClipData() { Content = text } });
        }
      }
    }
  }
}
