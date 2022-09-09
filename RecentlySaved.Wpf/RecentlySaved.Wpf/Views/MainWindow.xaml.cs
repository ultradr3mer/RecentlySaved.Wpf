
using AdvancedClipboard.Wpf.Services;
using MahApps.Metro.Controls;
using Prism.Events;
using Prism.Regions;
using RecentlySaved.Wpf.Views.Fragments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Unity;

namespace RecentlySaved.Wpf.Views
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : MetroWindow
  {
    public const string RecentFilesRegion = "RecentFilesRegion";
    public const string ClipboardHistRegion = "ClipboardHistRegion";

    private readonly IEventAggregator eventAggregator;
    private readonly ClipboardWatcher clipboardWatcher;

    public MainWindow(IRegionManager regionManager, IUnityContainer container, IEventAggregator eventAggregator, ClipboardWatcher clipboardWatcher)
    {
      InitializeComponent();

      regionManager.RegisterViewWithRegion(MainWindow.RecentFilesRegion, () => container.Resolve<RecentFilesFragment>());
      regionManager.RegisterViewWithRegion(MainWindow.ClipboardHistRegion, () => container.Resolve<ClipboardHistFragment>());

      var clip = new WpfClipboardMonitor.ClipboardMonitor(this, true);
      clip.ClipboardUpdate += this.Clip_ClipboardUpdate;
      this.eventAggregator = eventAggregator;
      this.clipboardWatcher = clipboardWatcher;
    }

    private void Clip_ClipboardUpdate(object sender, EventArgs e)
    {
      this.clipboardWatcher.Notify();
    }
  }
}
