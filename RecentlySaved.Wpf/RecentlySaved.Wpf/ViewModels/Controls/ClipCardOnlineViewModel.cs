using Prism.Events;
using RecentlySaved.Wpf.Composite;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using RecentlySaved.Wpf.Extensions;

namespace RecentlySaved.Wpf.ViewModels.Controls
{
  public class ClipCardOnlineViewModelBase : BaseViewModel<ClipboardGetData>
  {
    public LaneGetData Lane { get; set; }
    public string StringPreview { get; set; }
    public string TextContent { get; set; }
    public SolidColorBrush LaneBackgroundBrush { get; set; }
    public SolidColorBrush ForegroundBrush { get; set; }
    public string LaneName { get; internal set; }

    protected string GeneratePreview(string content)
    {
      IEnumerable<string> lines = content.Replace(Environment.NewLine, "\n").Replace("\t", "  ").Split('\n');
      int minSpaces = lines.Min(l => l.Length - l.TrimStart().Length);
      lines = lines.Select(l => ">" + l.Substring(minSpaces).TrimEnd());

      return string.Join(Environment.NewLine, lines);
    }
  }

  public class ClipCardOnlineViewModel : ClipCardOnlineViewModelBase
  {

    public ClipCardOnlineViewModel()
    {
      this.PropertyChanged += this.ClipCardViewModel_PropertyChanged;
    }

    private void ClipCardViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(TextContent))
      {
        this.StringPreview = this.GeneratePreview(this.TextContent);
      }
      if(e.PropertyName == nameof(Lane))
      {
        var laneColor = (Color)ColorConverter.ConvertFromString(this.Lane.Color);
        this.LaneBackgroundBrush = new SolidColorBrush(laneColor);

        var foregroundColor = laneColor.CalculateLuminance() > 0.6 ? Colors.Black : Colors.White;
        this.ForegroundBrush = new SolidColorBrush(foregroundColor);
      }
    }

    protected override void OnReadingDataModel(ClipboardGetData data)
    {
      this.StringPreview = this.GeneratePreview(data.TextContent);
    }
  }
}
