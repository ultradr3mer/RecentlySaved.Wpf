namespace RecentlySaved.Console
{
  class FileWatcher : IDisposable
  {
    private List<FileSystemWatcher> watcherList;
    private readonly LinkService linkService;

    public FileWatcher(LinkService linkService, IEnumerable<string> pathList)
    {
      this.linkService = linkService;
      this.watcherList = new List<FileSystemWatcher>();

      foreach (string path in pathList)
      {
        var watcher = new FileSystemWatcher(path);

        watcher.NotifyFilter = NotifyFilters.Attributes
                             | NotifyFilters.CreationTime
                             | NotifyFilters.DirectoryName
                             | NotifyFilters.FileName
                             | NotifyFilters.LastAccess
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Security
                             | NotifyFilters.Size;

        watcher.Changed += OnChanged;
        watcher.Created += OnCreated;
        watcher.Deleted += OnDeleted;
        watcher.Renamed += OnRenamed;
        watcher.Error += OnError;

        watcher.Filter = "*.*";
        watcher.EnableRaisingEvents = true;

        this.watcherList.Add(watcher);
      }
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
      if (e.ChangeType != WatcherChangeTypes.Changed)
      {
        return;
      }
      System.Console.WriteLine($"Changed: {e.FullPath}");
      this.linkService.CreateUpdateLink(e.FullPath);
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
      string value = $"Created: {e.FullPath}";
      System.Console.WriteLine(value);
      this.linkService.CreateUpdateLink(e.FullPath);
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
      System.Console.WriteLine($"Deleted: {e.FullPath}");
      this.linkService.DeleteLink(e.FullPath);
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
      System.Console.WriteLine($"Renamed:");
      System.Console.WriteLine($"    Old: {e.OldFullPath}");
      System.Console.WriteLine($"    New: {e.FullPath}");
      //this.linkService.DeleteLink(e.OldFullPath);
      //this.linkService.CreateLink(e.FullPath);
    }

    private void OnError(object sender, ErrorEventArgs e) =>
        PrintException(e.GetException());

    private void PrintException(Exception ex)
    {
      if (ex != null)
      {
        System.Console.WriteLine($"Message: {ex.Message}");
        System.Console.WriteLine("Stacktrace:");
        System.Console.WriteLine(ex.StackTrace);
        System.Console.WriteLine();
        if (ex.InnerException != null)
        {
          PrintException(ex.InnerException);
        }
      }
    }

    public void Dispose()
    {
      foreach (var item in watcherList)
      {
        item.Dispose();
      }
    }
  }
}
