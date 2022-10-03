using AdvancedClipboard.Wpf.Services;
using Prism.Events;
using RecentlySaved.Wpf.Composite;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.Extensions;
using RecentlySaved.Wpf.Repositories;
using RecentlySaved.Wpf.ViewModels.Controls;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using Unity;

namespace RecentlySaved.Wpf.ViewModels.Fragments
{
  public class ClipboardHistFragmentViewModelBase : BaseViewModel
  {
    public BindingList<ClipCardViewModelBase> Items { get; set; } = new BindingList<ClipCardViewModelBase>();
    public ClipCardViewModelBase SelectedItem { get; set; }
  }

  public class ClipboardHistFragmentViewModel : ClipboardHistFragmentViewModelBase
  {
    private readonly IUnityContainer container;
    private readonly PersistantRepository persistantRepository;
    private readonly ClipboardWatcher watcher;
    private readonly SelectionChangedEvent selectionChangedEvent;
    public static bool IsAlteringClipboard;
    private readonly ConcurrentQueue<ClipboardChangedData> changedDatas = new ConcurrentQueue<ClipboardChangedData>();

    public ClipboardHistFragmentViewModel(IEventAggregator eventAggregator, IUnityContainer container, PersistantRepository persistantRepository, ClipboardWatcher watcher)
    {
      this.container = container;
      this.persistantRepository = persistantRepository;
      this.watcher = watcher;
      this.ReadItems();

      eventAggregator.GetEvent<ClipboardChangedEvent>().Subscribe(this.OnClipCreatedChanged, ThreadOption.UIThread);
      eventAggregator.GetEvent<SelectionChangedEvent>().Subscribe(this.OnSelectionChanged);
      eventAggregator.GetEvent<ClipboardPinnedChangedEvent>().Subscribe(this.OnPinnedChanged);
      this.selectionChangedEvent = eventAggregator.GetEvent<SelectionChangedEvent>();

      this.PropertyChanged += this.ClipboardHistFragmentViewModel_PropertyChanged;
    }

    private void OnPinnedChanged(ClipboardPinnedChangedData data)
    {
      foreach (var singleItem in this.Items.ToList())
      {
        if (singleItem.DatamodelEquals(data.Data))
        {
          singleItem.SetDataModel(data.Data);
        }
      }
    }

    private void OnSelectionChanged(SelectionChangedData data)
    {
      if (this.SelectedItem != data.Item)
      {
        this.SelectedItem = null;
      }
    }

    private void ReadItems()
    {
      this.Items.Clear();
      foreach (ClipData item in persistantRepository.GetRecentClipboardData())
      {
        ClipCardViewModel vm = this.container.Resolve<ClipCardViewModel>().GetWithDataModel(item);
        this.Items.Add(vm);
      }
    }

    private void OnDeactivated(MainWindowDeactivatedData obj)
    {
      ClipboardHistFragmentViewModel.IsAlteringClipboard = false;
      while (this.changedDatas.TryDequeue(out ClipboardChangedData changedDatas))
      {
        this.OnClipCreatedChanged(changedDatas);
      }
    }

    private void ClipboardHistFragmentViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(this.SelectedItem))
      {
        if (SelectedItem == null)
        {
          return;
        }

        this.selectionChangedEvent.Publish(new SelectionChangedData() { Item = this.SelectedItem });
        ClipboardHistFragmentViewModel.IsAlteringClipboard = true;
        this.watcher.PutOntoClipboard(this.SelectedItem.WriteToDataModel());
      }
    }

    private void OnClipCreatedChanged(ClipboardChangedData data)
    {
      if (ClipboardHistFragmentViewModel.IsAlteringClipboard)
      {
        changedDatas.Enqueue(data);
        return;
      }

      var existing = this.Items.FirstOrDefault(c => c.Content == data.Data.Content);
      if (existing != null)
      {
        this.Items.Remove(existing);
      }

      ClipCardViewModel vm = this.container.Resolve<ClipCardViewModel>().GetWithDataModel(data.Data);
      this.Items.Insert(0, vm);
      this.SelectedItem = vm;
    }
  }
}
