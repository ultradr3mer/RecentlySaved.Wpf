using Prism.Events;
using RecentlySaved.Wpf.Composite;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.ViewModels.Controls;

namespace RecentlySaved.Wpf.ViewModels.Fragments
{
  internal class ClipPreviewOnlineFragmentViewModel : ObservableBase
  {
    public ClipCardOnlineViewModel Item { get; set; }

    public ClipPreviewOnlineFragmentViewModel(IEventAggregator eventAggregator)
    {
      eventAggregator.GetEvent<SelectionChangedEvent>().Subscribe(this.OnSelectionChanged);
    }

    private void OnSelectionChanged(SelectionChangedData data)
    {
      if(data.Item is ClipCardOnlineViewModel newItem)
      {
        this.Item = newItem;
      }
    }
  }
}
