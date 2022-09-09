using Prism.Events;
using RecentlySaved.Wpf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecentlySaved.Wpf.Events
{
  public class ClipboardChangedData
  {
    public ClipData Data { get; set; }

    internal void Subscribe(object onClipboardChanged)
    {
      throw new NotImplementedException();
    }
  }

  public class ClipboardChangedEvent : PubSubEvent<ClipboardChangedData>
  {

  }
}
