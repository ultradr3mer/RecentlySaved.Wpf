﻿using Prism.Events;
using RecentlySaved.Wpf.Events;
using System.Windows.Controls;

namespace RecentlySaved.Wpf.Views.Fragments
{
  /// <summary>
  /// Interaction logic for ClipboardHistFragment.xaml
  /// </summary>
  public partial class ClipboardHistFragment : UserControl
  {
    public ClipboardHistFragment(IEventAggregator eventAggregator)
    {
      InitializeComponent();

      eventAggregator.GetEvent<MainWindowDeactivatedEvent>().Subscribe(this.OnMainDeactivated, ThreadOption.UIThread);
    }

    private void OnMainDeactivated(MainWindowDeactivatedData obj)
    {
      if (this.List.Items.Count > 0)
      {
        var first = this.List.Items[0];
        this.List.ScrollIntoView(first);
      }
    }
  }
}
