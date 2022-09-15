using Prism.Events;
using RecentlySaved.Wpf.Composite;
using RecentlySaved.Wpf.Constants;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.Extensions;
using RecentlySaved.Wpf.ViewModels.Controls;
using System.ComponentModel;
using Unity;

namespace RecentlySaved.Wpf.ViewModels.Fragments
{
  public class ClipboardOnlineFragmentViewModel : BaseViewModel
  {
    private readonly IUnityContainer container;

    public BindingList<ClipCardViewModelBase> Items { get; set; } = new BindingList<ClipCardViewModelBase>();
    public ClipCardViewModelBase SelectedItem { get; set; }

    public ClipboardOnlineFragmentViewModel(IEventAggregator eventAggregator, IUnityContainer container)
    {
      eventAggregator.GetEvent<ClipboardOnlineItemsRetrivedEvent>().Subscribe(this.OnItemsRetrived, ThreadOption.UIThread);
      this.container = container;
    }

    private void OnItemsRetrived(ClipboardOnlineItemsRetrivedData data)
    {
      foreach (var item in data.Entries)
      {
        if (item.ContentTypeId == ContentTypes.PlainText)
        {
          var dataEntry = new ClipData() { Content = item.TextContent };
          ClipCardViewModel vm = this.container.Resolve<ClipCardViewModel>().GetWithDataModel(dataEntry);
          this.Items.Add(vm);
        }
      }
    }
  }
}
