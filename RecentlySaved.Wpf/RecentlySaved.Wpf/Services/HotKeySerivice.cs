using RecentlySaved.Wpf.Views;
using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace RecentlySaved.Wpf.Services
{
  public class HotKeySerivice
  {
    [DllImport("User32.dll")]
    private static extern bool RegisterHotKey(
    [In] IntPtr hWnd,
    [In] int id,
    [In] uint fsModifiers,
    [In] uint vk);

    [DllImport("User32.dll")]
    private static extern bool UnregisterHotKey(
        [In] IntPtr hWnd,
        [In] int id);

    private HwndSource _source;
    private MainWindow hotKeyWindow;
    private const int HOTKEY_ID = 9000;

    internal void Register(MainWindow hotKeyWindow)
    {
      this.hotKeyWindow = hotKeyWindow;
      var helper = new WindowInteropHelper(hotKeyWindow);
      _source = HwndSource.FromHwnd(helper.Handle);
      _source.AddHook(HwndHook);
      RegisterHotKey();
    }

    internal void Dispose()
    {
      _source.RemoveHook(HwndHook);
      _source = null;
      UnregisterHotKey();
    }

    private void RegisterHotKey()
    {
      var helper = new WindowInteropHelper(this.hotKeyWindow);
      const uint VK_F10 = 0x79;
      const uint Q_KEY = 0x51;
      const uint S_KEY = 0x53;
      const uint MOD_CTRL = 0x0002;
      const uint MOD_ALT = 0x0001;
      if (!RegisterHotKey(helper.Handle, HOTKEY_ID, MOD_ALT, S_KEY))
      {
        // handle error
      }
    }

    private void UnregisterHotKey()
    {
      var helper = new WindowInteropHelper(this.hotKeyWindow);
      UnregisterHotKey(helper.Handle, HOTKEY_ID);
    }

    private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
      const int WM_HOTKEY = 0x0312;
      switch (msg)
      {
        case WM_HOTKEY:
          switch (wParam.ToInt32())
          {
            case HOTKEY_ID:
              OnHotKeyPressed();
              handled = true;
              break;
          }
          break;
      }
      return IntPtr.Zero;
    }

    private void OnHotKeyPressed()
    {
      hotKeyWindow.OnHotKeyPressed();
    }

  }
}
