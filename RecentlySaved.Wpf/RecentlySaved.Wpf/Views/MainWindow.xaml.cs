
using AdvancedClipboard.Wpf.Services;
using MahApps.Metro.Controls;
using Prism.Events;
using Prism.Regions;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.Repositories;
using RecentlySaved.Wpf.Services;
using RecentlySaved.Wpf.Views.Controls;
using RecentlySaved.Wpf.Views.Fragments;
using System;
using System.Web.UI;
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
    public const string Preview = "Preview";

    private readonly IEventAggregator eventAggregator;
    private readonly ClipboardWatcher clipboardWatcher;
    private readonly HotKeySerivice hotKeySerivice;
    private readonly PersistantRepository persistantRepository;

    private IRegionManager regionManager;
    private MainWindowDeactivatedEvent deactivatedEvent;

    public MainWindow(IRegionManager regionManager,
                      IUnityContainer container,
                      IEventAggregator eventAggregator,
                      ClipboardWatcher clipboardWatcher,
                      HotKeySerivice hotKeySerivice,
                      PersistantRepository persistantRepository)
    {
      this.hotKeySerivice = hotKeySerivice;
      this.persistantRepository = persistantRepository;
      this.InitializeComponent();

      this.regionManager = regionManager;
      regionManager.RegisterViewWithRegion(MainWindow.RecentFilesRegion, () => container.Resolve<RecentFilesFragment>());
      regionManager.RegisterViewWithRegion(MainWindow.ClipboardHistRegion, () => container.Resolve<ClipboardHistFragment>());
      regionManager.RegisterViewWithRegion(MainWindow.Preview, () => container.Resolve<UserControl>());
      regionManager.RegisterViewWithRegion(MainWindow.Preview, () => container.Resolve<ClipPreviewWindow>());

      WpfClipboardMonitor.ClipboardMonitor clip = new WpfClipboardMonitor.ClipboardMonitor(this, true);
      clip.ClipboardUpdate += this.Clip_ClipboardUpdate;
      this.eventAggregator = eventAggregator;
      this.clipboardWatcher = clipboardWatcher;
      eventAggregator.GetEvent<ClipboardSelectionChangedEvent>().Subscribe(this.OnSelectedClipboardItemChanged);
      this.deactivatedEvent = eventAggregator.GetEvent<MainWindowDeactivatedEvent>();
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
      base.OnSourceInitialized(e);
      this.hotKeySerivice.Register(this);
    }

    internal void OnHotKeyPressed()
    {
      this.Activate();
    }

    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);
      this.hotKeySerivice.Dispose();
      this.persistantRepository.Save(true);
    }

    protected override void OnActivated(EventArgs e)
    {
      base.OnActivated(e);
      this.persistantRepository.Save(false);
    }

    protected override void OnDeactivated(EventArgs e)
    {
      base.OnDeactivated(e);
      deactivatedEvent.Publish(new MainWindowDeactivatedData());
      this.persistantRepository.Save(true);
    }

    private void OnSelectedClipboardItemChanged(ClipboardSelectionChangedData data)
    {
      regionManager.RequestNavigate(MainWindow.Preview, new Uri(nameof(ClipPreviewWindow), UriKind.Relative));
    }

    private void Clip_ClipboardUpdate(object sender, EventArgs e)
    {
      this.clipboardWatcher.Notify();
    }
  }
}
