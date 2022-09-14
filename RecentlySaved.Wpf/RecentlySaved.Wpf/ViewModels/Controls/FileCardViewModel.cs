using Prism.Commands;
using RecentlySaved.Wpf.Composite;
using RecentlySaved.Wpf.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.MobileControls;
using System.Windows.Input;

namespace RecentlySaved.Wpf.ViewModels.Controls
{
  public class FileCardViewModelBase : BaseViewModel<FileData>
  {
    public string FileName { get; set; }
    public string FilePath { get; set; }
    public string FullPath { get; set; }
    public DateTime Date { get; set; }
    public string DateString { get; set; }

    public ICommand OpenCommand { get; set; } 
  }

  public class FileCardViewModel : FileCardViewModelBase
  {
    protected override void OnReadingDataModel(FileData data)
    {
      this.FileName = Path.GetFileName(data.FullPath);
      this.FilePath = Path.GetDirectoryName(data.FullPath);
      this.DateString = data.Date.ToString("d");

      this.OpenCommand = new DelegateCommand(this.OpenCommandExecute);
    }

    private void OpenCommandExecute()
    {
      string argument = "/select, \"" + this.FullPath + "\"";

      System.Diagnostics.Process.Start("explorer.exe", argument);
    }
  }


}
