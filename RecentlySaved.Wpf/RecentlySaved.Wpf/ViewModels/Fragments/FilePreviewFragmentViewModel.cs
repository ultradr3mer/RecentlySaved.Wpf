using MvvmHelpers.Commands;
using Prism.Commands;
using Prism.Events;
using RecentlySaved.Wpf.Composite;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.Services;
using RecentlySaved.Wpf.ViewModels.Controls;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RecentlySaved.Wpf.ViewModels.Fragments
{
  internal class FilePreviewFragmentViewModelBase : BaseViewModel
  {
    public FileCardViewModel Item { get; set; }
  }

  internal class FilePreviewFragmentViewModel : FilePreviewFragmentViewModelBase
  {
    private readonly UploadService uploadService;

    public ICommand OpenCommand { get; set; }

    public FilePreviewFragmentViewModel(IEventAggregator eventAggregator, UploadService uploadService)
    {
      eventAggregator.GetEvent<SelectionChangedEvent>().Subscribe(this.OnSelectionChanged);
      this.OpenCommand = new DelegateCommand(this.OpenCommandExecute);
      this.uploadService = uploadService;
    }

    private void OpenCommandExecute()
    {
      string argument = "/select, \"" + this.Item.FullPath + "\"";

      System.Diagnostics.Process.Start("explorer.exe", argument);
    }

    private void OnSelectionChanged(SelectionChangedData data)
    {
      if (data.Item is FileCardViewModel newItem)
      {
        this.Item = newItem;
      }
    }

    public async Task UploadCommandExecute()
    {
      await this.uploadService.PostFile(Item.FullPath);
    }
  }
}
