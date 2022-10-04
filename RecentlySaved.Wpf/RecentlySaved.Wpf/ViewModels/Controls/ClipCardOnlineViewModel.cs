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
using RecentlySaved.Wpf.Constants;
using System.Windows.Media.Imaging;
using System.Net.Cache;

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
    public ImageSource ImageSource { get; set; }
    public ImageSource ImageSourceThumbnail { get; set; }
    public Uri FullFileContentUrl { get; set; }

    protected string GeneratePreview(string content)
    {
      if(string.IsNullOrWhiteSpace(content))
      {
        return string.Empty;
      }

      IEnumerable<string> lines = content.Replace(Environment.NewLine, "\n").Replace("\t", "  ").Split('\n');
      int minSpaces = lines.Min(l => l.Length - l.TrimStart().Length);
      lines = lines.Select(l => ">" + l.Substring(minSpaces).TrimEnd());

      return string.Join(Environment.NewLine, lines);
    }
  }

  public class ClipCardOnlineViewModel : ClipCardOnlineViewModelBase
  {
    private const string BaseUrl = "https://advancedclipboard2.azurewebsites.net/api/file/";
    private const string UrlThumbnails = "thumb/";

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
      if (e.PropertyName == nameof(Lane))
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

      if (data.ContentTypeId == ContentTypes.Image)
      {
        this.ImageSource = BitmapFrame.Create(CreateUrl(data.FileContentUrl), BitmapCreateOptions.None, BitmapCacheOption.OnDemand);
        this.ImageSourceThumbnail = BitmapFrame.Create(CreateUrl(UrlThumbnails + data.FileContentUrl), BitmapCreateOptions.None, BitmapCacheOption.OnDemand);
        this.FullFileContentUrl = CreateUrl(data.FileContentUrl);
      }
    }

    public static Uri CreateUrl(string localUrl)
    {
      return string.IsNullOrEmpty(localUrl) ? null : new Uri(BaseUrl + localUrl);
    }
  }
}
