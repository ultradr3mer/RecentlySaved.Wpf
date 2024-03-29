﻿using AdvancedClipboard.Wpf.Services;
using Prism.Events;
using RecentlySaved.Wpf.Composite;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RecentlySaved.Wpf.ViewModels.Controls
{
  public class ClipCardViewModelBase : BaseViewModel<ClipData>
  {
    public string StringPreview { get; set; }
    public string Content { get; set; }
    public string MetaString { get; set; }
    public bool IsPinned { get; set; }
    public string ImageFileName { get; set; }
    public ImageSource ImageSource { get; set; }
    public ImageSource ImageSourceThumbnail { get; set; }

    protected string GeneratePreview(string content)
    {
      if (string.IsNullOrEmpty(content))
      {
        return string.Empty;
      }

      IEnumerable<string> lines = content.Replace(Environment.NewLine, "\n").Replace("\t", "  ").Split('\n');
      int minSpaces = lines.Min(l => l.Length - l.TrimStart().Length);
      lines = lines.Select(l => ">" + l.Substring(minSpaces).TrimEnd());
      return string.Join(Environment.NewLine, lines);
    }
  }

  public class ClipCardViewModel : ClipCardViewModelBase
  {
    private ClipboardPinnedChangedEvent pinnedEvent;

    public ClipCardViewModel(IEventAggregator eventAggregator)
    {
      this.PropertyChanged += this.ClipCardViewModel_PropertyChanged;
      pinnedEvent = eventAggregator.GetEvent<ClipboardPinnedChangedEvent>();
    }

    private void ClipCardViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(Content))
      {
        this.StringPreview = this.GeneratePreview(this.Content);
      }
      if (e.PropertyName == nameof(IsPinned) && !IsReadingDataModel)
      {
        pinnedEvent.Publish(new ClipboardPinnedChangedData() { Data = this.WriteToDataModel() });
      }
    }

    protected override void OnReadingDataModel(ClipData data)
    {
      this.StringPreview = this.GeneratePreview(data.Content);
      this.MetaString = data.Datum.ToString("d") + " " + data.ProcessName;
      if (!string.IsNullOrEmpty(data.ImageFileName))
      {
        try
        {
          this.ImageSource = BitmapFrame.Create(ClipboardWatcher.GetUriForImage(data.ImageFileName));
        }
        catch (FileNotFoundException)
        {
          // this.ImageSource = ImageSource.err;
        }
      }
    }

    public override bool Equals(object obj)
    {
      if (obj is ClipCardViewModel otherVm)
      {
        return this.Content == otherVm.Content &&
          this.ImageFileName == otherVm.ImageFileName;
      }

      if (obj is ClipData otherData)
      {
        return this.Content == otherData.Content &&
          this.ImageFileName == otherData.ImageFileName;
      }

      return false;
    }
  }
}
