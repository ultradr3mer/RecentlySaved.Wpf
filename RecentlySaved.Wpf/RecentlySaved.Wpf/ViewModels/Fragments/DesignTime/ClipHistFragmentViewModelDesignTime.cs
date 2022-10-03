using RecentlySaved.Wpf.ViewModels.Controls;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace RecentlySaved.Wpf.ViewModels.Fragments.DesignTime
{
  public class ClipHistFragmentViewModelDesignTime : ClipboardHistFragmentViewModelBase
  {
    public ClipHistFragmentViewModelDesignTime()
    {
      this.Items = new BindingList<ClipCardViewModelBase>(new List<ClipCardViewModelBase>()
      {
        new ClipCardViewModelBase { StringPreview = @">using System;
>using System.Runtime.InteropServices;
>using System.Windows;
>using System.Windows.Interop;", Content = @"using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;", MetaString = @"Notepad 12.04.2022" },
        new ClipCardViewModelBase { StringPreview = @">QMessageBox msgBox;
>msgBox.setText(""No Row Selected."");
>msgBox.setInformativeText(""Please select a row that you want to generate an equation for."");
>      msgBox.setStandardButtons(QMessageBox::Ok);
>      msgBox.setDefaultButton(QMessageBox::Ok);
>      msgBox.exec();
      ", MetaString = @"Notepad 12.04.2022" },
        new ClipCardViewModelBase { StringPreview = @">QList<map<string, double>> variablesList;
>  for (int i = 0; i <= resolution; i++)
>  {
>    double r = this->ui->fromX->value() + stepSize * i;
>    variablesList.append(map<string, double>{{BASE_LETTER, r}});
>  }", MetaString = @"Notepad 12.04.2022" },
        new ClipCardViewModelBase {
          MetaString = @"Notepad 12.04.2022", 
          ImageSource = new System.Uri(@"https://advancedclipboard2.azurewebsites.net/api/file/thumb/D55A6AC0786D48F9/clip_20211125_175008.png")
        },
        new ClipCardViewModelBase
        {
          MetaString = @"Notepad 12.04.2022",
          ImageSource = new System.Uri(@"https://advancedclipboard2.azurewebsites.net/api/file/thumb/5F1AA915DFAE986B/clip_20210925_165928.png")
        }
      });

      this.SelectedItem = this.Items.First();
    }
  }
}
