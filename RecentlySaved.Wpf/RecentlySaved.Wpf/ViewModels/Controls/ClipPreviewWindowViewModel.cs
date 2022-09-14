using Prism.Events;
using RecentlySaved.Wpf.Composite;
using RecentlySaved.Wpf.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecentlySaved.Wpf.ViewModels.Controls
{
  internal class ClipPreviewWindowViewModel : BaseViewModel
  {
    public ClipCardViewModelBase Item { get; set; }

    public ClipPreviewWindowViewModel(IEventAggregator eventAggregator)
    {
      eventAggregator.GetEvent<ClipboardSelectionChangedEvent>().Subscribe(this.OnSelectionChanged);
    }

    private void OnSelectionChanged(ClipboardSelectionChangedData data)
    {
      this.Item = data.Item;
    }
  }
}
