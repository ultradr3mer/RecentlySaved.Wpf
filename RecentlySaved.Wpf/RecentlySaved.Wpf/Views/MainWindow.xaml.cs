
using AdvancedClipboard.Wpf.Services;
using MahApps.Metro.Controls;
using Prism.Events;
using Prism.Regions;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.Interop;
using RecentlySaved.Wpf.Repositories;
using RecentlySaved.Wpf.Services;
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

    private readonly IEventAggregator eventAggregator;
    private readonly ClipboardWatcher clipboardWatcher;
    private readonly HotKeySerivice hotKeySerivice;
    private readonly PersistantRepository persistantRepository;

    private IRegionManager regionManager;
    private MainWindowDeactivatedEvent deactivatedEvent;

    private int aspect = 8;

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

      WpfClipboardMonitor.ClipboardMonitor clip = new WpfClipboardMonitor.ClipboardMonitor(this, true);
      clip.ClipboardUpdate += this.Clip_ClipboardUpdate;
      this.eventAggregator = eventAggregator;
      this.clipboardWatcher = clipboardWatcher;
      eventAggregator.GetEvent<ClipboardSelectionChangedEvent>().Subscribe(this.OnSelectedClipboardItemChanged);
      eventAggregator.GetEvent<FileSelectionChangedEvent>().Subscribe(this.OnSelectedFileChanged);
      this.deactivatedEvent = eventAggregator.GetEvent<MainWindowDeactivatedEvent>();

      this.MoveToBottomPosition();
    }

    private void MoveToBottomPosition()
    {
      this.Height = SystemParameters.WorkArea.Height / aspect;
      this.Width = SystemParameters.WorkArea.Width * 0.8;

      this.Left = (SystemParameters.WorkArea.Width - this.Width) / 2;
      this.Top = (SystemParameters.WorkArea.Height - this.Height);
    }

    private void OnSelectedFileChanged(FileSelectionChangedData obj)
    {
      regionManager.RequestNavigate(MainWindow.Preview, new Uri(nameof(FilePreviewFragment), UriKind.Relative));
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
      this.persistantRepository.Save(false);
    }

    protected override void OnDeactivated(EventArgs e)
    {
      base.OnDeactivated(e);
      deactivatedEvent.Publish(new MainWindowDeactivatedData());
      this.persistantRepository.Save(true);
      this.WindowState = WindowState.Minimized;
    }

    private void OnSelectedClipboardItemChanged(ClipboardSelectionChangedData data)
    {
      regionManager.RequestNavigate(MainWindow.Preview, new Uri(nameof(ClipPreviewFragment), UriKind.Relative));
    }

    private void Clip_ClipboardUpdate(object sender, EventArgs e)
    {
      this.clipboardWatcher.Notify();
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
      aspect /= 2;

      MoveToBottomPosition();
    }
  }
}
