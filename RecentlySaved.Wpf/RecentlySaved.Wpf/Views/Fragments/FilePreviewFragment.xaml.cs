using RecentlySaved.Wpf.ViewModels.Fragments;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace RecentlySaved.Wpf.Views.Fragments
{
  /// <summary>
  /// Interaction logic for FilePreviewFragment.xaml
  /// </summary>
  public partial class FilePreviewFragment : UserControl
  {
    public FilePreviewFragment()
    {
      InitializeComponent();
    }

    private FilePreviewFragmentViewModel viewModel { get => DataContext as FilePreviewFragmentViewModel; }

    private async void UploadClickAsync(object sender, RoutedEventArgs e)
    {
      var tmpCursor = this.Cursor;
      this.Cursor = Cursors.Wait;
      await this.viewModel.UploadCommandExecute();
      this.Cursor = tmpCursor;
    }
  }
}
