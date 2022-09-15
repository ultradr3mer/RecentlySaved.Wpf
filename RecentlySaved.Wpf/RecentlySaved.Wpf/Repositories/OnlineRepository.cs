using Prism.Events;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Unity;

namespace RecentlySaved.Wpf.Repositories
{
  public class OnlineRepository
  {
    private readonly CredentialsService credentialsService;
    private ClipboardOnlineItemsRetrivedEvent itemsRetrivedEvent;
    private Client client;
    private HttpClientHandler handler;
    private HttpClient httpClient;

    public OnlineRepository(IEventAggregator eventAggregator, CredentialsService credentialsService)
    {
      this.credentialsService = credentialsService;
      this.itemsRetrivedEvent = eventAggregator.GetEvent<ClipboardOnlineItemsRetrivedEvent>();
    }

    internal async Task Initialize()
    {
      var cookieContainer = new CookieContainer();
      this.handler = new HttpClientHandler() { CookieContainer = cookieContainer };
      this.httpClient = new HttpClient(handler);
      this.client = new Client(this.httpClient);

      var data = new AuthenticationPostData() { Email = this.credentialsService.UserName, Password = this.credentialsService.UserPassword };
      await this.client.ApiAuthenticationAsync(data);

      await Refresh();
    }

    private async Task Refresh()
    {
      var content = await this.client.ApiClipboardGetwithcontextAsync();
      this.itemsRetrivedEvent.Publish(new ClipboardOnlineItemsRetrivedData()
      {
        Entries = content.Entries.ToList(),
        Lanes = content.Lanes.ToList()
      });
    }
  }

}
