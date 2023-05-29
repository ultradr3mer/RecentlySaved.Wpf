using AdvancedClipboard.Wpf.Services;
using Polly.Retry;
using Polly;
using Prism.Events;
using RecentlySaved.Wpf.Composite;
using RecentlySaved.Wpf.Constants;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.Extensions;
using RecentlySaved.Wpf.ViewModels.Controls;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Unity;
using System.Drawing;

namespace RecentlySaved.Wpf.ViewModels.Fragments
{
  public class ClipboardOnlineFragmentViewModelBase : ObservableBase
  {
    public BindingList<ClipCardOnlineViewModel> Items { get; set; } = new BindingList<ClipCardOnlineViewModel>();
    public ClipCardOnlineViewModel SelectedItem { get; set; }
  }

  public class ClipboardOnlineFragmentViewModel : ClipboardOnlineFragmentViewModelBase
  {
    private readonly IUnityContainer container;
    private readonly ClipboardWatcher watcher;
    private SelectionChangedEvent selectionChangedEvent;

    public ClipboardOnlineFragmentViewModel(IEventAggregator eventAggregator, IUnityContainer container, ClipboardWatcher watcher)
    {
      eventAggregator.GetEvent<SelectionChangedEvent>().Subscribe(this.OnSelectionChanged);
      eventAggregator.GetEvent<ClipboardOnlineItemsRetrivedEvent>().Subscribe(this.OnItemsRetrived, ThreadOption.UIThread);
      this.selectionChangedEvent = eventAggregator.GetEvent<SelectionChangedEvent>();
      this.container = container;
      this.watcher = watcher;
      this.PropertyChanged += this.ClipboardOnlineFragmentViewModel_PropertyChanged;
    }

    private void OnSelectionChanged(SelectionChangedData data)
    {
      if (this.SelectedItem != data.Item)
      {
        this.SelectedItem = null;
      }
    }

    private void ClipboardOnlineFragmentViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(SelectedItem))
      {
        ClipCardOnlineViewModel selectedItem = this.SelectedItem;
        if (selectedItem == null)
        {
          return;
        }

        this.selectionChangedEvent.Publish(new SelectionChangedData() { Item = selectedItem });

        this.PutOntoClipboard(selectedItem);
      }
    }

    private async void PutOntoClipboard(ClipCardOnlineViewModel selectedItem)
    {
      var data = new ClipData()
      {
        Content = selectedItem.TextContent,
        Datum = DateTime.Now,
        ProcessName = "Advanced Clipboard"
      };

      if (selectedItem.ImageSource != null)
      {
        var image = await this.GetBitmapImage(selectedItem.FullFileContentUrl);

        BitmapSource bmpSource = null;

        RetryPolicy retryIfException =
          Policy.Handle<Exception>().WaitAndRetry(3, r => TimeSpan.FromMilliseconds(100));

        retryIfException.Execute(() =>
        {
          Clipboard.SetImage(image);
          bmpSource = Clipboard.GetImage();
        });

        if (this.watcher.CheckMd5AndSave(BitmapFrame.Create(bmpSource), out string md5, out string extension))
        {
          data.ImageFileName = md5 + extension;
        }
        else
        {
          // Selected Image is already in clipboard
          return;
        }
      }

      ClipboardHistFragmentViewModel.IsAlteringClipboard = true;
      this.watcher.PutOntoClipboard(data);
    }

    public async Task<BitmapImage> GetBitmapImage(Uri uri)
    {
      byte[] buffer = null;
      using (var httpClient = new System.Net.Http.HttpClient())
      {
        buffer = await httpClient.GetByteArrayAsync(uri);
      }

      using (var stream = new MemoryStream(buffer))
      {
        var image = new BitmapImage();
        image.BeginInit();
        image.CreateOptions = BitmapCreateOptions.None;
        image.CacheOption = BitmapCacheOption.OnLoad;
        image.UriSource = null;
        image.StreamSource = stream;
        image.EndInit();

        return image;
      }
    }

    private void OnItemsRetrived(ClipboardOnlineItemsRetrivedData data)
    {
      this.Items.Clear();

      var laneDict = data.Lanes.ToDictionary(o => o.Id.Value);

      foreach (var item in data.Entries)
      {
        if (item.ContentTypeId == ContentTypes.PlainText || item.ContentTypeId == ContentTypes.Image)
        {
          ClipCardOnlineViewModel vm = this.container.Resolve<ClipCardOnlineViewModel>().GetWithDataModel(item);
          if (item.LaneId != null)
          {
            vm.Lane = laneDict[item.LaneId.Value];
            vm.LaneName = vm.Lane.Name;
          }

          this.Items.Add(vm);
        }
      }
    }
  }
}
