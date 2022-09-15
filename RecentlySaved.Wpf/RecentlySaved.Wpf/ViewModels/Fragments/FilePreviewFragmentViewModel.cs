using Prism.Events;
using RecentlySaved.Wpf.Composite;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecentlySaved.Wpf.ViewModels.Fragments
{
  internal class FilePreviewFragmentViewModelBase : BaseViewModel
  {
    public FileCardViewModel Item { get; set; }
  }

  internal class FilePreviewFragmentViewModel : FilePreviewFragmentViewModelBase
  {
    public FilePreviewFragmentViewModel(IEventAggregator eventAggregator)
    {
      eventAggregator.GetEvent<SelectionChangedEvent>().Subscribe(this.OnSelectionChanged);
    }

    private void OnSelectionChanged(SelectionChangedData data)
    {
      if (data.Item is FileCardViewModel newItem)
      {
        this.Item = newItem;
      }
    }
  }
}
