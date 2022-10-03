using RecentlySaved.Wpf.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecentlySaved.Wpf.ViewModels
{
  public class EntryItemViewModel 
  {
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public override bool Equals(object obj)
    {
      return base.Equals(obj);
    }
  }
}
