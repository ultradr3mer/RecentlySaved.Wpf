using AdvancedClipboard.Wpf.Services;
using Prism.Ioc;
using Prism.Mvvm;
using RecentlySaved.Wpf.Repositories;
using RecentlySaved.Wpf.Services;
using RecentlySaved.Wpf.Views;
using System.Windows;

namespace RecentlySaved.Wpf
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App 
  {
    #region Fields


    #endregion Fields

    #region Methods

    protected override Window CreateShell()
    {
      var mainWindow = this.Container.Resolve<MainWindow>();
      return mainWindow;
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
      containerRegistry.RegisterManySingleton<SettingsRepository>();
      containerRegistry.RegisterManySingleton<PersistantRepository>();
      containerRegistry.RegisterManySingleton<FileWatcher>();
      containerRegistry.RegisterManySingleton<ClipboardWatcher>();
      containerRegistry.RegisterManySingleton<CredentialsService>();
      containerRegistry.RegisterManySingleton<OnlineRepository>();
      containerRegistry.RegisterManySingleton<UploadService>();

      ViewModelLocationProvider.SetDefaultViewModelFactory((type) => this.Container.Resolve(type));
    }

    #endregion
  }
}
