using Prism.Events;
using RecentlySaved.Wpf.Composite;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace RecentlySaved.Wpf.ViewModels.Controls
{
  public class ClipCardOnlineViewModelBase : BaseViewModel<ClipboardGetData>
  {
    public LaneGetData Lane { get; set; }
    public string StringPreview { get; set; }
    public string Content { get; set; }
    public SolidColorBrush LaneColorBrush { get; set; }

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
      if (e.PropertyName == nameof(Content))
      {
        this.StringPreview = this.GeneratePreview(this.Content);
      }
      if(e.PropertyName == nameof(Lane))
      {
        var laneColor = (Color)ColorConverter.ConvertFromString(this.Lane.Color);
        this.LaneColorBrush = new SolidColorBrush(laneColor);
      }
    }

    protected override void OnReadingDataModel(ClipboardGetData data)
    {
      this.StringPreview = this.GeneratePreview(data.TextContent);
    }
  }
}
