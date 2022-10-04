using AdvancedClipboard.Wpf.Services;
using Prism.Events;
using RecentlySaved.Wpf.Composite;
using RecentlySaved.Wpf.Constants;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.Extensions;
using RecentlySaved.Wpf.ViewModels.Controls;
using System;
using System.ComponentModel;
using System.Linq;
using Unity;

namespace RecentlySaved.Wpf.ViewModels.Fragments
{
  public class ClipboardOnlineFragmentViewModelBase : ObservableBase
  {
    public BindingList<ClipCardOnlineViewModel> Items { get; set; } = new BindingList<ClipCardOnlineViewModel>();
    public ClipCardOnlineViewModel SelectedItem { get; set; }
  }

  public class ClipboardOnlineFragmentViewModel : ClipboardOnlineFragmentViewModelBase
  {
    private readonly IUnityContainer container;
    private readonly ClipboardWatcher watcher;
    private SelectionChangedEvent selectionChangedEvent;

    public ClipboardOnlineFragmentViewModel(IEventAggregator eventAggregator, IUnityContainer container, ClipboardWatcher watcher)
    {
      eventAggregator.GetEvent<SelectionChangedEvent>().Subscribe(this.OnSelectionChanged);
      eventAggregator.GetEvent<ClipboardOnlineItemsRetrivedEvent>().Subscribe(this.OnItemsRetrived, ThreadOption.UIThread);
      this.selectionChangedEvent = eventAggregator.GetEvent<SelectionChangedEvent>();
      this.container = container;
      this.watcher = watcher;
      this.PropertyChanged += this.ClipboardOnlineFragmentViewModel_PropertyChanged;
    }

    private void OnSelectionChanged(SelectionChangedData data)
    {
      if (this.SelectedItem != data.Item)
      {
        this.SelectedItem = null;
      }
    }

    private void ClipboardOnlineFragmentViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if(e.PropertyName == nameof(SelectedItem))
      {
        ClipCardOnlineViewModel selectedItem = this.SelectedItem;
        if (selectedItem == null)
        {
          return;
        }

        this.selectionChangedEvent.Publish(new SelectionChangedData() { Item = selectedItem });
        var data = new ClipData() { Content = selectedItem.TextContent, Datum = DateTime.Now, ProcessName = "Advanced Clipboard" };
        ClipboardHistFragmentViewModel.IsAlteringClipboard = true;

        this.watcher.PutOntoClipboard(data);
      }
    }

    private void OnItemsRetrived(ClipboardOnlineItemsRetrivedData data)
    {
      this.Items.Clear();

      var laneDict = data.Lanes.ToDictionary(o => o.Id.Value);

      foreach (var item in data.Entries)
      {
        if (item.ContentTypeId == ContentTypes.PlainText)
        {
          ClipCardOnlineViewModel vm = this.container.Resolve<ClipCardOnlineViewModel>().GetWithDataModel(item);
          if (item.LaneId != null)
          {
            vm.Lane = laneDict[item.LaneId.Value];
            vm.LaneName = vm.Lane.Name;
          }

          this.Items.Add(vm);
        }
      }
    }
  }
}
