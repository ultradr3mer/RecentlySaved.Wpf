using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecentlySaved.Wpf.ViewModels.Controls.DesignTime
{
  internal class ClipCardViewModelDesignTime : ClipCardViewModelBase
  {
    public ClipCardViewModelDesignTime()
    {
      this.StringPreview = @">QMessageBox msgBox;
>msgBox.setText(""No Row Selected."");
>msgBox.setInformativeText(""Please select a row that you want to generate an equation for."");
>  msgBox.setStandardButtons(QMessageBox::Ok);
>  msgBox.setDefaultButton(QMessageBox::Ok);
>  msgBox.exec();";
      this.MetaInfo = @"Notepad";
    }

  }
}
