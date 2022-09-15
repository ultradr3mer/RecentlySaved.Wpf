using Prism.Events;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecentlySaved.Wpf.Events
{
  public class ClipboardOnlineItemsRetrivedData
  {
    public List<ClipboardGetData> Entries { get; set; }
    public List<LaneGetData> Lanes { get; set; }
  }

  public class ClipboardOnlineItemsRetrivedEvent : PubSubEvent<ClipboardOnlineItemsRetrivedData>
  {

  }
}
