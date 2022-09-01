using Prism.Ioc;
using Prism.Unity;
using RecentlySaved.Wpf.Views;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RecentlySaved.Wpf
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App 
  {
    #region Fields

    public const string RegionName = "MainRegion";

    #endregion Fields

    #region Methods

    protected override Window CreateShell()
    {
      var mainWindow = this.Container.Resolve<MainWindow>();
      return mainWindow;
    }

    protected override void RegisterTypes(IContainerRegistry containerRegistry)
    {
    }

    #endregion
  }
}
