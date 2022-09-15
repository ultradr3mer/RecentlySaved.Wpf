using Prism.Events;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecentlySaved.Wpf.Events
{
  public class ClipboardPinnedChangedData
  {
    public ClipData Data { get; internal set; }

    internal void Subscribe(object onClipboardPinnedChanged)
    {
      throw new NotImplementedException();
    }
  }

  public class ClipboardPinnedChangedEvent : PubSubEvent<ClipboardPinnedChangedData>
  {

  }
}
