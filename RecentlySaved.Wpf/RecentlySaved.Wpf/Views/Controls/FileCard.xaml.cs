using RecentlySaved.Wpf.ViewModels.Controls;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;

namespace RecentlySaved.Wpf.Views.Controls
{
  /// <summary>
  /// Interaction logic for FileCard.xaml
  /// </summary>
  public partial class FileCard : UserControl
  {
    public FileCard()
    {
      InitializeComponent();

      if (DesignerProperties.GetIsInDesignMode(this))
      {
        return;
      }

      this.DataContextChanged += this.FileCard_DataContextChanged;
      this.MainGrid.SizeChanged += this.Path_SizeChanged;
    }

    private void Path_SizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
    {
      this.UpdatePath();
    }

    private void UpdatePath()
    {
      if (this.ViewModel?.FilePath != null)
      {
        if (this.MainGrid.ActualWidth > 0)
        {
          this.Path.Text = this.FitPath(this.ViewModel.FilePath, this.MainGrid.ActualWidth);
          return;
        }

        this.Path.Text = this.ViewModel.FilePath;
      }
    }

    private FileCardViewModelBase ViewModel
    {
      get { return (FileCardViewModelBase)this.DataContext; }
    }

    private void FileCard_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
    {
      this.UpdatePath();
    }

    private string FitPath(string filePath, double width)
    {
      if (this.MeasureString(filePath) < width)
      {
        return filePath;
      }

      for (int i = 0; i < this.ViewModel.FilePath.Length; i++)
      {
        string canidate = "..." + this.ViewModel.FilePath.Substring(i);

        if (this.MeasureString(canidate) < width)
        {
          return canidate;
        }
      }

      throw new Exception("Unable to fit path.");
    }

    private double MeasureString(string candidate)
    {
      var formattedText = new FormattedText(
             candidate,
             CultureInfo.CurrentCulture,
             System.Windows.FlowDirection.LeftToRight,
             new Typeface(this.Path.FontFamily, this.Path.FontStyle, this.Path.FontWeight, this.Path.FontStretch),
             this.Path.FontSize,
             Brushes.Black,
             new NumberSubstitution(),
             1);

      return formattedText.Width;
    }
  }
}
