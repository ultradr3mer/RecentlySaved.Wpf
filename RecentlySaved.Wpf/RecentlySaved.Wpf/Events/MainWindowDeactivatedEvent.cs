using Prism.Events;
using RecentlySaved.Wpf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecentlySaved.Wpf.Events
{
  public class MainWindowDeactivatedData
  {
  }

  public class MainWindowDeactivatedEvent : PubSubEvent<MainWindowDeactivatedData>
  {

  }
}
