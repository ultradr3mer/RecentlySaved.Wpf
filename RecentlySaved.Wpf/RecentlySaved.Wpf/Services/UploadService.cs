using Prism.Events;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RecentlySaved.Wpf.Services
{
  public class UploadService
  {
    private const string UploadLaneName = "StartPlus";
    private readonly OnlineRepository onlineRepository;
    private LaneGetData uploadLane;

    public UploadService(OnlineRepository onlineRepository, IEventAggregator eventAggregator)
    {
      eventAggregator.GetEvent<ClipboardOnlineItemsRetrivedEvent>().Subscribe(this.OnDataRetrived);

      this.onlineRepository = onlineRepository;
    }

    public async Task PostFile(string fullPath)
    {
      var laneId = this.uploadLane?.Id ?? await this.CreateUploadLane();

      await onlineRepository.PostfileAsync(fullPath, laneId);
      await onlineRepository.Refresh();
    }

    public async Task PostPlainTextAsync(string content)
    {
      var body = new ClipboardPostPlainTextData
      {
        Content = content,
        LaneGuid = this.uploadLane?.Id ?? await this.CreateUploadLane()
      };

      await onlineRepository.PostPlainTextAsync(body);
      await onlineRepository.Refresh();
    }

    private async Task<Guid?> CreateUploadLane()
    {
      var data = new LanePostData()
      {
        Color = "#0e6eba", // Accent Color,
        Name = UploadLaneName
      };
      var result = await onlineRepository.PostlaneAsync(data);
      return result.Id;
    }

    private void OnDataRetrived(ClipboardOnlineItemsRetrivedData data)
    {
      this.uploadLane = data.Lanes.FirstOrDefault(l =>
        string.Compare(l.Name, UploadLaneName, StringComparison.OrdinalIgnoreCase) == 0);
    }

  }
}
