using MvvmHelpers.Commands;
using Prism.Commands;
using Prism.Events;
using RecentlySaved.Wpf.Composite;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.Repositories;
using RecentlySaved.Wpf.Services;
using RecentlySaved.Wpf.ViewModels.Controls;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RecentlySaved.Wpf.ViewModels.Fragments
{
  internal class ClipPreviewFragmentViewModel : BaseViewModel
  {
    private readonly UploadService uploadService;
    public ClipCardViewModelBase Item { get; set; }
    public ICommand UploadCommand { get; set; }

    public ClipPreviewFragmentViewModel(IEventAggregator eventAggregator, UploadService uploadService)
    {
      eventAggregator.GetEvent<SelectionChangedEvent>().Subscribe(this.OnSelectionChanged);
      this.UploadCommand = new AsyncCommand(this.UploadCommandExecute);
      this.uploadService = uploadService;
    }

    private async Task UploadCommandExecute()
    {
      await this.uploadService.PostPlainTextAsync(Item.Content);
    }

    private void OnSelectionChanged(SelectionChangedData data)
    {
      if (data.Item is ClipCardViewModel newItem)
      {
        this.Item = newItem;
      }
    }
  }
}
