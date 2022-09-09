using RecentlySaved.Wpf.Composite;
using RecentlySaved.Wpf.Data;
using System;
using System.Linq;

namespace RecentlySaved.Wpf.ViewModels.Controls
{
  public class ClipCardViewModelBase : BaseViewModel<ClipData>
  {
    public string StringPreview { get; set; }
    public string Content { get; set; }
    public string MetaString { get; set; }

    protected string GeneratePreview(string content)
    {
      var lines = content.Trim().Replace(Environment.NewLine, "\n").Split('\n');
      return string.Join(Environment.NewLine, lines.Select(l => ">" + l));
    }
  }

  public class ClipCardViewModel : ClipCardViewModelBase
  {
    protected override void OnReadingDataModel(ClipData data)
    {
      this.StringPreview = this.GeneratePreview(data.Content);
      this.MetaString = data.Datum.ToString("d") + " " + data.ProcessName;
    }
  }
}
