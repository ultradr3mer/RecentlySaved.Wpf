﻿using AdvancedClipboard.Wpf.Services;
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
  public class ClipboardPinnedFragmentViewModelBase : ObservableBase
  {
    public BindingList<ClipCardViewModelBase> Items { get; set; } = new BindingList<ClipCardViewModelBase>();
    public ClipCardViewModelBase SelectedItem { get; set; }
  }

  public class ClipboardPinnedFragmentViewModel : ClipboardPinnedFragmentViewModelBase
  {
    private readonly IUnityContainer container;
    private readonly PersistantRepository persistantRepository;
    private readonly ClipboardWatcher watcher;
    private readonly SelectionChangedEvent selectionChangedEvent;

    private readonly ConcurrentQueue<ClipData> changedDatas = new ConcurrentQueue<ClipData>();

    public ClipboardPinnedFragmentViewModel(IEventAggregator eventAggregator, IUnityContainer container, PersistantRepository persistantRepository, ClipboardWatcher watcher)
    {
      this.container = container;
      this.persistantRepository = persistantRepository;
      this.watcher = watcher;
      this.ReadItems();

      eventAggregator.GetEvent<ClipboardPinnedChangedEvent>().Subscribe(this.OnClipPinned);
      eventAggregator.GetEvent<SelectionChangedEvent>().Subscribe(this.OnSelectionChanged);
      eventAggregator.GetEvent<MainWindowDeactivatedEvent>().Subscribe(this.OnDeactivated);
      this.selectionChangedEvent = eventAggregator.GetEvent<SelectionChangedEvent>();

      this.PropertyChanged += this.ClipboardHistFragmentViewModel_PropertyChanged;
    }

    private void OnDeactivated(MainWindowDeactivatedData obj)
    {
      ClipboardHistFragmentViewModel.IsAlteringClipboard = false;
      while (this.changedDatas.TryDequeue(out ClipData data))
      {
        this.MoveItemToTop(data);
      }
    }

    private void OnSelectionChanged(SelectionChangedData data)
    {
      if (this.SelectedItem != data.Item)
      {
        this.SelectedItem = null;
      }
    }

    private void OnClipPinned(ClipboardPinnedChangedData data)
    {
      this.MoveItemToTop(data.Data);
    }

    private void MoveItemToTop(ClipData data)
    {
      foreach (var singleItem in this.Items.ToList())
      {
        if (singleItem.Equals(data))
        {
          this.Items.Remove(singleItem);
        }
      }

      if (data.IsPinned)
      {
        ClipCardViewModel vm = this.container.Resolve<ClipCardViewModel>().GetWithDataModel(data);
        this.Items.Insert(0, vm);
      }
    }

    private void ReadItems()
    {
      this.Items.Clear();
      foreach (ClipData item in persistantRepository.GetPinnedClipboardData())
      {
        ClipCardViewModel vm = this.container.Resolve<ClipCardViewModel>().GetWithDataModel(item);
        this.Items.Add(vm);
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
        ClipData data = this.SelectedItem.WriteToDataModel();
        this.watcher.PutOntoClipboard(data);
        this.changedDatas.Enqueue(data);
      }
    }
  }
}
