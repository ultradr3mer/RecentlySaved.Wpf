using RecentlySaved.Wpf.ViewModels.Controls;
using RecentlySaved.Wpf.ViewModels.Controls.DesignTime;
using System.Collections.Generic;
using System.ComponentModel;

namespace RecentlySaved.Wpf.ViewModels.Fragments.DesignTime
{
  public class ClipHistFragmentViewModelDesignTime : ClipboardHistFragmentViewModelBase
  {
    public ClipHistFragmentViewModelDesignTime()
    {
      this.Items = new BindingList<ClipCardViewModelBase>(new List<ClipCardViewModelBase>()
      {
        new ClipCardViewModel { StringPreview = @">using System;
>using System.Runtime.InteropServices;
>using System.Windows;
>using System.Windows.Interop;", MetaInfo = @"Notepad 12.04.2022" },
        new ClipCardViewModel { StringPreview = @">QMessageBox msgBox;
>msgBox.setText(""No Row Selected."");
>msgBox.setInformativeText(""Please select a row that you want to generate an equation for."");
>      msgBox.setStandardButtons(QMessageBox::Ok);
>      msgBox.setDefaultButton(QMessageBox::Ok);
>      msgBox.exec();
      ", MetaInfo = @"Notepad 12.04.2022" },
        new ClipCardViewModel { StringPreview = @">QList<map<string, double>> variablesList;
>  for (int i = 0; i <= resolution; i++)
>  {
>    double r = this->ui->fromX->value() + stepSize * i;
>    variablesList.append(map<string, double>{{BASE_LETTER, r}});
>  }", MetaInfo = @"Notepad 12.04.2022" }
      });
    }
  }
}
