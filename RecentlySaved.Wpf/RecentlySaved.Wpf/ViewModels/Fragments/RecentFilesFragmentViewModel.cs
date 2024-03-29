﻿using Prism.Events;
using RecentlySaved.Wpf.Composite;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.Extensions;
using RecentlySaved.Wpf.Repositories;
using RecentlySaved.Wpf.ViewModels.Controls;
using System;
using System.ComponentModel;
using System.Linq;
using Unity;

namespace RecentlySaved.Wpf.ViewModels.Fragments
{
  public class RecentFilesFragementViewModelBase : ObservableBase
  {
    public BindingList<FileCardViewModel> Items { get; set; } = new BindingList<FileCardViewModel>();
    public FileCardViewModel SelectedItem { get; set; }
  }

  public class RecentFilesFragmentViewModel : RecentFilesFragementViewModelBase
  {

#pragma warning disable IDE0044 // Add readonly modifier
    private FileWatcher watcher;
    private SelectionChangedEvent selectionChangedEvent;
    private readonly IUnityContainer container;
#pragma warning restore IDE0044 // Add readonly modifier

    public RecentFilesFragmentViewModel(IEventAggregator eventAggregator, PersistantRepository fileRepository, FileWatcher watcher, IUnityContainer container)
    {
      this.watcher = watcher;
      this.container = container;

      eventAggregator.GetEvent<FileCreatedChangedEvent>().Subscribe(this.OnFileCreatedChanged, ThreadOption.UIThread);
      eventAggregator.GetEvent<FileDeletedEvent>().Subscribe(this.OnFileDeleted, ThreadOption.UIThread);
      eventAggregator.GetEvent<FileRenamedEvent>().Subscribe(this.OnFileRenamed, ThreadOption.UIThread);
      eventAggregator.GetEvent<SelectionChangedEvent>().Subscribe(this.OnSelectionChanged, ThreadOption.UIThread);

      this.selectionChangedEvent = eventAggregator.GetEvent<SelectionChangedEvent>();

      foreach (FileData singleFile in fileRepository.GetRecentFiles())
      {
        this.Items.Add(this.container.Resolve<FileCardViewModel>().GetWithDataModel(singleFile));
      }

      this.PropertyChanged += this.RecentFilesFragmentViewModel_PropertyChanged;
    }

    private void OnSelectionChanged(SelectionChangedData data)
    {
      if (this.SelectedItem != data.Item)
      {
        this.SelectedItem = null;
      }
    }

    private void RecentFilesFragmentViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if(e.PropertyName == nameof(SelectedItem))
      {
        if(this.SelectedItem == null)
        {
          return;
        }

        selectionChangedEvent.Publish(new SelectionChangedData() { Item = this.SelectedItem });
      }
    }

    private void OnFileRenamed(FileRenamedData data)
    {
      var vm = this.container.Resolve<FileCardViewModel>().GetWithDataModel(data.NewData);
      this.InsertDistinct(vm, data.OldData.FullPath, data.NewData.FullPath);
    }

    private void OnFileDeleted(FileDeletedEventData data)
    {
      foreach (var item in this.Items.ToList())
      {
        if (item.FullPath == data.DeletedFile.FullPath)
        {
          this.Items.Remove(item);
        }
      }
    }

    private void OnFileCreatedChanged(FileCreatedChangedData data)
    {
      var vm = this.container.Resolve<FileCardViewModel>().GetWithDataModel(data.CreatedChangedData);
      this.InsertDistinct(vm, data.CreatedChangedData.FullPath);
    }

    private void InsertDistinct(FileCardViewModel vm, params string[] replacePaths)
    {
      foreach (var item in this.Items.ToList())
      {
        if (replacePaths.Contains(item.FullPath))
        {
          this.Items.Remove(item);
        }
      }

      this.Items.Insert(0, vm);
    }
  }
}
