using System.Windows.Media;

namespace RecentlySaved.Wpf.ViewModels.Controls.DesignTime
{
  public class ClipCardOnlineViewModelDesignTime : ClipCardOnlineViewModelBase
  {
    public ClipCardOnlineViewModelDesignTime()
    {
      this.StringPreview = @">QMessageBox msgBox;
>msgBox.setText(""No Row Selected."");
>msgBox.setInformativeText(""Please select a row that you want to generate an equation for."");
>  msgBox.setStandardButtons(QMessageBox::Ok);
>  msgBox.setDefaultButton(QMessageBox::Ok);
>  msgBox.exec();";
      this.LaneBackgroundBrush = new System.Windows.Media.SolidColorBrush(Colors.Yellow);
    }
  }
}
