using RecentlySaved.Wpf.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RecentlySaved.Wpf.ViewModels.Fragments.DesignTime
{
  public class RecentFilesFragmentViewModelDesignTime : RecentFilesFragementViewModelBase
  {
    public RecentFilesFragmentViewModelDesignTime()
    {
      this.Items = new BindingList<FileCardViewModel>(new List<FileCardViewModel>()
      {
        new FileCardViewModel
        {
          FileName = "Datei 1",
          FilePath = @"C:\Users\Clara\source\repos\RecentlySaved.Wpf\RecentlySaved.Wpf\RecentlySaved.Wpf\bin\Debug"
        },
        new FileCardViewModel
        {
          FileName = "Datei 2",
          FilePath = @"C:\Users\Clara\Downloads"
        },
        new FileCardViewModel
        {
          FileName = "Datei 3",
          FilePath = @"C:\Users\Clara\Downloads"
        }
      });
    }
  }

}
