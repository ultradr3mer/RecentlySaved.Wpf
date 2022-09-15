using Prism.Regions;
using RecentlySaved.Wpf.Repositories;
using RecentlySaved.Wpf.Services;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unity;

namespace RecentlySaved.Wpf.Views.Fragments
{
  /// <summary>
  /// Interaction logic for LoginFragment.xaml
  /// </summary>
  public partial class LoginFragment : UserControl
  {
    private readonly CredentialsService credentialsService;
    private readonly IRegionManager regionManager;
    private readonly OnlineRepository onlineRepository;

    public LoginFragment(CredentialsService credentialsService, IRegionManager regionManager, OnlineRepository onlineRepository)
    {
      InitializeComponent();

      this.credentialsService = credentialsService;
      this.regionManager = regionManager;
      this.onlineRepository = onlineRepository;
      this.username.Text = credentialsService.UserName;
      this.password.Password = credentialsService.UserPassword;

      if (!string.IsNullOrEmpty(this.username.Text) && !string.IsNullOrEmpty(this.password.Password))
      {
        Login();
      }
    }

    private async void Login()
    {
      try
      {
        this.ShowSingInScreen(true);
        this.message.Text = "Signing in...";
        this.message.Visibility = Visibility.Visible;
        this.credentialsService.SetCredentials(this.username.Text, this.password.Password);
        await onlineRepository.Initialize();
        this.regionManager.RequestNavigate(MainWindow.ClipbboardOnlineRegion, new Uri(nameof(ClipboardOnlineFragment), UriKind.Relative));

        this.ShowSingInScreen(false);
        this.message.Text = string.Empty;
        this.message.Visibility = Visibility.Collapsed;
      }
      catch (Exception ex)
      {
        this.ShowSingInScreen(false);
        this.message.Text = ex.Message;
        this.message.Visibility = Visibility.Visible;
      }
    }

    private void LoginClick(object sender, RoutedEventArgs e)
    {
      Login();
    }

    private void ShowSingInScreen(bool showSingInScreen)
    {
      this.LoadingStackPanel.Visibility = showSingInScreen ? Visibility.Visible : Visibility.Collapsed;
      this.LoginStackPanel.Visibility = showSingInScreen ? Visibility.Collapsed : Visibility.Visible;
    }
  }
}
