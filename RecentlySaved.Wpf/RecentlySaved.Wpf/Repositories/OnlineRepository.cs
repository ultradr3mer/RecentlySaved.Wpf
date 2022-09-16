using Prism.Events;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.Services;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace RecentlySaved.Wpf.Repositories
{
  public class OnlineRepository
  {
    private readonly CredentialsService credentialsService;
    private readonly ClipboardOnlineItemsRetrivedEvent itemsRetrivedEvent;
    private Client client;
    private CookieContainer cookieContainer;
    private HttpClientHandler handler;
    private HttpClient httpClient;

    public OnlineRepository(IEventAggregator eventAggregator, CredentialsService credentialsService)
    {
      this.credentialsService = credentialsService;
      this.itemsRetrivedEvent = eventAggregator.GetEvent<ClipboardOnlineItemsRetrivedEvent>();
    }

    internal async Task Initialize()
    {
      this.cookieContainer = new CookieContainer();
      this.handler = new HttpClientHandler() { CookieContainer = cookieContainer };
      this.httpClient = new HttpClient(this.handler);
      this.client = new Client(this.httpClient);

      AuthenticationPostData data = new AuthenticationPostData() { Email = this.credentialsService.UserName, Password = this.credentialsService.UserPassword };
      await this.client.ApiAuthenticationAsync(data);

      await this.Refresh();
    }

    internal async Task<ClipboardGetData> PostfileAsync(string fullPath, Guid? laneId)
    {
      string extension = Path.GetExtension(fullPath);

      using (var stream = File.OpenRead(fullPath))
      {
        return await this.client.ApiClipboardPostfileAsync(extension, laneId, new FileParameter(stream));
      }
    }

    internal Task PostPlainTextAsync(ClipboardPostPlainTextData body)
    {
      return this.client.ApiClipboardPostplaintextAsync(body);
    }

    internal Task<LaneGetData> PostlaneAsync(LanePostData data)
    {
      return this.client.ApiLanePostlaneAsync(data);
    }

    public async Task Refresh()
    {
      ClipboardContainerGetData content = await this.client.ApiClipboardGetwithcontextAsync();
      this.itemsRetrivedEvent.Publish(new ClipboardOnlineItemsRetrivedData()
      {
        Entries = content.Entries.ToList(),
        Lanes = content.Lanes.ToList()
      });
    }
  }
}


