using RecentlySaved.Wpf.ViewModels.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media;

namespace RecentlySaved.Wpf.ViewModels.Fragments.DesignTime
{
  public class ClipboardOnlineFragmentViewModelDesingTime : ClipboardOnlineFragmentViewModelBase
  {
    public ClipboardOnlineFragmentViewModelDesingTime()
    {
      this.Items = new BindingList<ClipCardOnlineViewModel>(new List<ClipCardOnlineViewModel>()
      {
        new ClipCardOnlineViewModel { StringPreview = @">using System;
>using System.Runtime.InteropServices;
>using System.Windows;
>using System.Windows.Interop;", Content = @"using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;", LaneColorBrush = new System.Windows.Media.SolidColorBrush(Colors.Yellow) },
        new ClipCardOnlineViewModel { StringPreview = @">QMessageBox msgBox;
>msgBox.setText(""No Row Selected."");
>msgBox.setInformativeText(""Please select a row that you want to generate an equation for."");
>      msgBox.setStandardButtons(QMessageBox::Ok);
>      msgBox.setDefaultButton(QMessageBox::Ok);
>      msgBox.exec();
      ", LaneColorBrush = new System.Windows.Media.SolidColorBrush(Colors.Red)  },
        new ClipCardOnlineViewModel { StringPreview = @">QList<map<string, double>> variablesList;
>  for (int i = 0; i <= resolution; i++)
>  {
>    double r = this->ui->fromX->value() + stepSize * i;
>    variablesList.append(map<string, double>{{BASE_LETTER, r}});
>  }", LaneColorBrush = new System.Windows.Media.SolidColorBrush(Colors.Yellow)  }
      });

      this.SelectedItem = this.Items.First();
    }
  }
}
