using RecentlySaved.Wpf.Composite;
using RecentlySaved.Wpf.Data;
using System;
using System.IO;
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
    public ICommand UploadCommand { get; set; }
  }

  public class FileCardViewModel : FileCardViewModelBase
  {

    protected override void OnReadingDataModel(FileData data)
    {
      this.FileName = Path.GetFileName(data.FullPath);
      this.FilePath = Path.GetDirectoryName(data.FullPath);
      this.DateString = data.Date.ToString("d");
    }
  }


}
