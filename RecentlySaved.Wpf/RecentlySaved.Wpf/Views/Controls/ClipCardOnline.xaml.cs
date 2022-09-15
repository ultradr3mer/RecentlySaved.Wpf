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
  public partial class ClipCardOnline : UserControl
  {
    public ClipCardOnline()
    {
      InitializeComponent();

      if (DesignerProperties.GetIsInDesignMode(this))
      {
        return;
      }
    }
  }
}
