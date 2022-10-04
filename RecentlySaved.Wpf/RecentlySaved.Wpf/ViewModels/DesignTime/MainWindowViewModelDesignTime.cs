using RecentlySaved.Wpf.Composite;
using System;
using System.Collections.Generic;

namespace RecentlySaved.Wpf.ViewModels.DesignTime
{
  public class MainWindowViewModelDesignTime : ObservableBase
  {
    public List<EntryItemViewModel> Items { get; set; }

    public MainWindowViewModelDesignTime()
    {
      this.Initialize();
    }

    protected virtual void Initialize()
    {
      this.Items = new List<EntryItemViewModel>()
      {
        new EntryItemViewModel { Title = "Datei 1", Description = "Geändert am 01.09.2022" },
        new EntryItemViewModel { Title = "Datei 2", Description = "Geändert am 01.09.2022" },
        new EntryItemViewModel { Title = "Datei 3", Description = "Geändert am 01.09.2022" }
      };
    }
  }
}
