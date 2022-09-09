using Prism.Events;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.Extensions;
using RecentlySaved.Wpf.Repositories;
using RecentlySaved.Wpf.ViewModels.Controls;
using RecentlySaved.Wpf.ViewModels.Fragments.DesignTime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace RecentlySaved.Wpf.ViewModels.Fragments
{
  public class RecentFilesFragementViewModelBase
  {
    public BindingList<FileCardViewModel> Items { get; set; } = new BindingList<FileCardViewModel>();
  }

  public class RecentFilesFragmentViewModel : RecentFilesFragementViewModelBase
  {

#pragma warning disable IDE0044 // Add readonly modifier
    private FileWatcher watcher;
    private readonly IUnityContainer container;
#pragma warning restore IDE0044 // Add readonly modifier

    public RecentFilesFragmentViewModel(IEventAggregator eventAggregator, FileRepository fileRepository, FileWatcher watcher, IUnityContainer container)
    {
      this.watcher = watcher;
      this.container = container;
      this.UpdateRecentFiles(fileRepository.GetRecentFiles());

      eventAggregator.GetEvent<RecentFilesChangedEvent>().Subscribe(this.OnRecentFilesChanged, ThreadOption.UIThread);
    }

    private void UpdateRecentFiles(IEnumerable<FileData> files)
    {
      //int currentIndex = 0;

      //List<FileCardViewModel> deadViewModels = this.Items.ToList();

      //foreach (var singleFile in files)
      //{
      //  var vm = this.Items.FirstOrDefault(o => o.FilePath == singleFile.FilePath && o.FileName == singleFile.FileName);
      //  if (vm != null)
      //  {
      //    vm.SetDataModel(o);
      //    deadViewModels.Remove(vm);
      //  }

      //  vm = this.container.Resolve<FileCardViewModel>().GetWithDataModel(singleFile);
      //  Items.Add(vm);
      //  deadViewModels.Remove(vm);
      //}

      //foreach (var vm in deadViewModels)
      //{
      //  this.Items.Remove(vm);
      //}

      //this.Items.(o => o.Date);

      this.Items.Clear();
      foreach (FileData singleFile in files)
      {
        this.Items.Add(this.container.Resolve<FileCardViewModel>().GetWithDataModel(singleFile));
      }
      
    }

    private void OnRecentFilesChanged(RecentFilesChangedData data)
    {
      this.UpdateRecentFiles(data.repository.GetRecentFiles());
    }
  }
}
