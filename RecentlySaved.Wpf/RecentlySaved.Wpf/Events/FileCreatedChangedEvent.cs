using Prism.Events;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecentlySaved.Wpf.Events
{
  public class FileCreatedChangedData
  {
    public FileData CreatedChangedData { get; set; }
  }

  public class FileCreatedChangedEvent : PubSubEvent<FileCreatedChangedData>
  {
  }
}
