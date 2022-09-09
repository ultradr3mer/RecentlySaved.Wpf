using Prism.Events;
using RecentlySaved.Wpf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecentlySaved.Wpf.Events
{
  public class FileRenamedData
  {
    public FileData OldData { get; set; }
    public FileData NewData { get; set; }
  }

  internal class FileRenamedEvent : PubSubEvent<FileRenamedData>
  {
  }
}
