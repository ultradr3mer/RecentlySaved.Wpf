using Prism.Events;
using RecentlySaved.Wpf.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecentlySaved.Wpf.Events
{
  public class RecentFilesChangedData
  {
    public FileRepository repository { get; set; }
  }

  internal class RecentFilesChangedEvent : PubSubEvent<RecentFilesChangedData>
  {
  }
}
