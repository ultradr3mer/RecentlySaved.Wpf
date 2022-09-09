using Prism.Events;
using RecentlySaved.Wpf.Events;
using System.Windows;

namespace AdvancedClipboard.Wpf.Services
{
  public class ClipboardWatcher
  {
    private ClipboardChangedEvent clipboardChanged;
    private readonly IEventAggregator eventAggregator;
    string lastText = string.Empty;

    public ClipboardWatcher(IEventAggregator eventAggregator)
    {
      this.clipboardChanged = eventAggregator.GetEvent<ClipboardChangedEvent>();
      this.eventAggregator = eventAggregator;
    }

    internal void Notify()
    {
      if (Clipboard.ContainsText())
      {
        string text = Clipboard.GetText(TextDataFormat.Text);
        if (text != lastText)
        {
          this.clipboardChanged.Publish(new ClipboardChangedData() { TextContent = text });
        }
      }
    }
  }
}
