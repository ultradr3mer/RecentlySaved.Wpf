using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecentlySaved.Wpf.Events
{
  public class ClipboardChangedData
  {
    public string TextContent { get; set; }
  }

  public class ClipboardChangedEvent : PubSubEvent<ClipboardChangedData>
  {

  }
}
