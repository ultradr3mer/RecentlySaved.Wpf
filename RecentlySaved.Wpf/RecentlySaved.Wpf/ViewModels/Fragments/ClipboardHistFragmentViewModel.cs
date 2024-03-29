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
  public class ClipboardHistFragmentViewModelBase : ObservableBase
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
    private static bool isActive;
    private readonly ConcurrentQueue<ClipboardChangedData> changedDatas = new ConcurrentQueue<ClipboardChangedData>();

    public ClipboardHistFragmentViewModel(IEventAggregator eventAggregator, IUnityContainer container, PersistantRepository persistantRepository, ClipboardWatcher watcher)
    {
      this.container = container;
      this.persistantRepository = persistantRepository;
      this.watcher = watcher;
      this.ReadItems();

      eventAggregator.GetEvent<ClipboardChangedEvent>().Subscribe(this.OnClipCreatedChanged, ThreadOption.UIThread);
      eventAggregator.GetEvent<SelectionChangedEvent>().Subscribe(this.OnSelectionChanged);
      eventAggregator.GetEvent<MainWindowDeactivatedEvent>().Subscribe(this.OnDeactivated);
      eventAggregator.GetEvent<MainWindowActivatedEvent>().Subscribe(this.OnActivated);
      this.selectionChangedEvent = eventAggregator.GetEvent<SelectionChangedEvent>();

      this.PropertyChanged += this.ClipboardHistFragmentViewModel_PropertyChanged;
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
      foreach (ClipData item in this.persistantRepository.GetRecentClipboardData())
      {
        ClipCardViewModelBase existing = this.Items.FirstOrDefault(c => c.Equals(item));
        if (existing != null)
        {
          this.Items.Remove(existing);
        }

        ClipCardViewModel vm = this.container.Resolve<ClipCardViewModel>().GetWithDataModel(item);
        this.Items.Add(vm);
      }
    }

    private void OnActivated(MainWindowActivatedData obj)
    {
      isActive = true;
    }

    private void OnDeactivated(MainWindowDeactivatedData obj)
    {
      isActive = false;
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
        if (this.SelectedItem == null)
        {
          return;
        }

        this.selectionChangedEvent.Publish(new SelectionChangedData() { Item = this.SelectedItem });

        if(isActive)
        {
          IsAlteringClipboard = true;
          this.watcher.PutOntoClipboard(this.SelectedItem.WriteToDataModel());
        }
      }
    }

    private void OnClipCreatedChanged(ClipboardChangedData data)
    {
      if (ClipboardHistFragmentViewModel.IsAlteringClipboard)
      {
        this.changedDatas.Enqueue(data);
        return;
      }

      bool isSelected = this.SelectedItem != null;

      var existing = this.Items.FirstOrDefault(c => c.Equals(data.Data));
      if (existing != null)
      {
        this.Items.Remove(existing);
      }

      ClipCardViewModel vm = this.container.Resolve<ClipCardViewModel>().GetWithDataModel(data.Data);
      this.Items.Insert(0, vm);

      if (isSelected)
      {
        this.SelectedItem = vm;
      }
    }
  }
}
