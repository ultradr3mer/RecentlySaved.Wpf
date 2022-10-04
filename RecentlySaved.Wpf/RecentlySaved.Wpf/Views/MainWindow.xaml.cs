
using AdvancedClipboard.Wpf.Services;
using MahApps.Metro.Controls;
using Prism.Events;
using Prism.Regions;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.Interop;
using RecentlySaved.Wpf.Repositories;
using RecentlySaved.Wpf.Services;
using RecentlySaved.Wpf.ViewModels.Controls;
using RecentlySaved.Wpf.Views.Controls;
using RecentlySaved.Wpf.Views.Fragments;
using System;
using System.Runtime.InteropServices;
using System.Web.UI;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Unity;

namespace RecentlySaved.Wpf.Views
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private const int GWL_STYLE = -16;

    private const int WS_SYSMENU = 0x80000;

    public const string RecentFilesRegion = "RecentFilesRegion";
    public const string ClipboardHistRegion = "ClipboardHistRegion";
    public const string Preview = "Preview";
    public const string ClipboardPinnedRegion = "ClipboardPinnedRegion";
    public const string ClipbboardOnlineRegion = "ClipbboardOnlineRegion";

    private readonly IEventAggregator eventAggregator;
    private readonly ClipboardWatcher clipboardWatcher;
    private readonly HotKeySerivice hotKeySerivice;
    private readonly PersistantRepository persistantRepository;

    private IRegionManager regionManager;
    private MainWindowDeactivatedEvent deactivatedEvent;
    private MainWindowActivatedEvent activatedEvent;
    private double aspect = 8.0;
    private ClipboardUpdateEvent updateEvent;

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

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

      WindowBlur.SetIsEnabled(this, true);

      this.regionManager = regionManager;
      regionManager.RegisterViewWithRegion(MainWindow.RecentFilesRegion, () => container.Resolve<RecentFilesFragment>());
      regionManager.RegisterViewWithRegion(MainWindow.ClipboardHistRegion, () => container.Resolve<ClipboardHistFragment>());
      regionManager.RegisterViewWithRegion(MainWindow.Preview, () => container.Resolve<UserControl>());
      regionManager.RegisterViewWithRegion(MainWindow.Preview, () => container.Resolve<ClipPreviewFragment>());
      regionManager.RegisterViewWithRegion(MainWindow.Preview, () => container.Resolve<FilePreviewFragment>());
      regionManager.RegisterViewWithRegion(MainWindow.Preview, () => container.Resolve<ClipPreviewOnlineFragment>());
      regionManager.RegisterViewWithRegion(MainWindow.ClipboardPinnedRegion, () => container.Resolve<ClipboardPinnedFragment>());
      regionManager.RegisterViewWithRegion(MainWindow.ClipbboardOnlineRegion, () => container.Resolve<LoginFragment>());
      regionManager.RegisterViewWithRegion(MainWindow.ClipbboardOnlineRegion, () => container.Resolve<ClipboardOnlineFragment>());

      eventAggregator.GetEvent<SelectionChangedEvent>();

      WpfClipboardMonitor.ClipboardMonitor clip = new WpfClipboardMonitor.ClipboardMonitor(this, true);
      this.updateEvent = eventAggregator.GetEvent<ClipboardUpdateEvent>();
      clip.ClipboardUpdate += this.Clip_ClipboardUpdate;
      this.eventAggregator = eventAggregator;

      this.clipboardWatcher = clipboardWatcher;
      eventAggregator.GetEvent<SelectionChangedEvent>().Subscribe(this.OnSelectedionChanged);
      this.deactivatedEvent = eventAggregator.GetEvent<MainWindowDeactivatedEvent>();
      this.activatedEvent = eventAggregator.GetEvent<MainWindowActivatedEvent>();

      this.MoveToBottomPosition();
    }

    private void Clip_ClipboardUpdate(object sender, EventArgs e)
    {
      this.updateEvent.Publish(new ClipboardUpdateData());
    }

    private void MoveToBottomPosition()
    {
      this.Height = SystemParameters.WorkArea.Height / aspect;
      this.Width = Math.Min(SystemParameters.WorkArea.Width * 0.9, 1800);

      this.Left = (SystemParameters.WorkArea.Width - this.Width) / 2;
      this.Top = (SystemParameters.WorkArea.Height - this.Height);
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
      base.OnSourceInitialized(e);
      this.hotKeySerivice.Register(this);
    }

    internal void OnHotKeyPressed()
    {
      this.WindowState = WindowState.Normal;
      this.aspect = 8;
      MoveToBottomPosition();
      this.Activate();
      this.WakeBlur();
    }

    private void WakeBlur()
    {
      this.Left--;
      this.Left++;
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
      activatedEvent.Publish(new MainWindowActivatedData());
      this.persistantRepository.Save(false);
    }

    protected override void OnDeactivated(EventArgs e)
    {
      base.OnDeactivated(e);
      deactivatedEvent.Publish(new MainWindowDeactivatedData());
      this.persistantRepository.Save(true);
      this.WindowState = WindowState.Minimized;
    }

    private void OnSelectedionChanged(SelectionChangedData data)
    {
      if (data.Item.GetType() == typeof(ClipCardViewModel))
      {
        regionManager.RequestNavigate(MainWindow.Preview, new Uri(nameof(ClipPreviewFragment), UriKind.Relative));
      }
      else if (data.Item.GetType() == typeof(FileCardViewModel))
      {
        regionManager.RequestNavigate(MainWindow.Preview, new Uri(nameof(FilePreviewFragment), UriKind.Relative));
      }
      else if (data.Item.GetType() == typeof(ClipCardOnlineViewModel))
      {
        regionManager.RequestNavigate(MainWindow.Preview, new Uri(nameof(ClipPreviewOnlineFragment), UriKind.Relative));
      }
    }

    private void MinimizeClick(object sender, RoutedEventArgs e)
    {
      this.WindowState = WindowState.Minimized;
    }

    private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
    {
      this.ContentGrid.Margin = new Thickness((this.WindowState == WindowState.Maximized) ? 5 : 0);
    }

    private void MaximizeClick(object sender, RoutedEventArgs e)
    {
      if (this.WindowState == System.Windows.WindowState.Normal)
      {
        this.WindowState = System.Windows.WindowState.Maximized;
      }
      else
      {
        this.WindowState = System.Windows.WindowState.Normal;
      }
    }

    private void CloseClick(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    private void MetroWindowLoaded(object sender, RoutedEventArgs e)
    {
      var hwnd = new WindowInteropHelper(this).Handle;
      SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
    }

    private void DragMouseDown(object sender, MouseButtonEventArgs e)
    {
      if (e.ChangedButton == MouseButton.Left)
      {
        this.DragMove();
      }
    }

    private void PopUpClick(object sender, RoutedEventArgs e)
    {
      aspect = Math.Max(aspect / 2.0, 1.0);

      MoveToBottomPosition();
    }

    private void PopDownClick(object sender, RoutedEventArgs e)
    {
      aspect = Math.Max(aspect * 2.0, 8.0);

      MoveToBottomPosition();
    }
  }
}
