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
      eventAggregator.GetEvent<FileSelectionChangedEvent>().Subscribe(this.OnSelectionChanged);
    }

    private void OnSelectionChanged(FileSelectionChangedData data)
    {
      this.Item = data.Item;
    }
  }
}
