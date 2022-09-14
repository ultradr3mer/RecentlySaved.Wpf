using RecentlySaved.Wpf.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecentlySaved.Wpf.ViewModels.Fragments.DesignTime
{
  internal class FilePreviewFragmentViewModelBaseDesignTime : FilePreviewFragmentViewModelBase
  {
    public FilePreviewFragmentViewModelBaseDesignTime()
    {
      this.Item = new FileCardViewModel()
      {
        FilePath = @"C:\Users\Clara\Downloads",
        FileName = "image.jpg",
        Date = DateTime.Parse("02.03.2022"),
        DateString = DateTime.Parse("02.03.2022").ToString("d")
      };
    }
  }
}
