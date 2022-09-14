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
  public class ClipboardSelectionChangedData
  {
    public ClipCardViewModelBase Item { get; set; }
  }

  public class ClipboardSelectionChangedEvent : PubSubEvent<ClipboardSelectionChangedData>
  {

  }
}
