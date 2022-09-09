using Prism.Events;
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
  public class ClipboardHistFragmentViewModelBase
  {
    public BindingList<ClipCardViewModelBase> Items { get; set; } = new BindingList<ClipCardViewModelBase>();
  }

  public class ClipboardHistFragmentViewModel : ClipboardHistFragmentViewModelBase
  {

    private readonly IUnityContainer container;

    public ClipboardHistFragmentViewModel(IEventAggregator eventAggregator, IUnityContainer container)
    {
      eventAggregator.GetEvent<ClipboardChangedEvent>().Subscribe(this.OnClipCreatedChanged, ThreadOption.UIThread);
      this.container = container;
    }

    private void OnClipCreatedChanged(ClipboardChangedData data)
    {
      var vm = this.container.Resolve<ClipCardViewModel>().GetWithDataModel(data.Data);
      Items.Add(vm);
    }
  }
}
