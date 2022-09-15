using Prism.Events;
using RecentlySaved.Wpf.Constants;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.Extensions;
using RecentlySaved.Wpf.ViewModels.Controls;
using System.ComponentModel;
using System.Linq;
using Unity;

namespace RecentlySaved.Wpf.ViewModels.Fragments
{
  public class ClipboardOnlineFragmentViewModelBase
  {
    public BindingList<ClipCardOnlineViewModel> Items { get; set; } = new BindingList<ClipCardOnlineViewModel>();
    public ClipCardOnlineViewModel SelectedItem { get; set; }
  }

  public class ClipboardOnlineFragmentViewModel : ClipboardOnlineFragmentViewModelBase
  {
    private readonly IUnityContainer container;

    public ClipboardOnlineFragmentViewModel(IEventAggregator eventAggregator, IUnityContainer container)
    {
      eventAggregator.GetEvent<ClipboardOnlineItemsRetrivedEvent>().Subscribe(this.OnItemsRetrived, ThreadOption.UIThread);
      this.container = container;
    }

    private void OnItemsRetrived(ClipboardOnlineItemsRetrivedData data)
    {
      var laneDict = data.Lanes.ToDictionary(o => o.Id.Value);

      foreach (var item in data.Entries)
      {
        if (item.ContentTypeId == ContentTypes.PlainText)
        {
          ClipCardOnlineViewModel vm = this.container.Resolve<ClipCardOnlineViewModel>().GetWithDataModel(item);
          if (item.LaneId != null)
          {
            vm.Lane = laneDict[item.LaneId.Value];
          }

          this.Items.Add(vm);
        }
      }
    }
  }
}
