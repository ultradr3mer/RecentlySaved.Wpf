using RecentlySaved.Wpf.Composite;
using RecentlySaved.Wpf.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecentlySaved.Wpf.ViewModels.Controls
{
  public class FileCardViewModelBase : BaseViewModel<FileData>
  {
    public string FileName { get; set; }

    public string FilePath { get; set; }

    public DateTime Date { get; set; }
  }

  public class FileCardViewModel : FileCardViewModelBase
  {
    protected override void OnReadingDataModel(FileData data)
    {
      //this.Folder = Path.GetFileName(data.FilePath);
    }
  }
}
