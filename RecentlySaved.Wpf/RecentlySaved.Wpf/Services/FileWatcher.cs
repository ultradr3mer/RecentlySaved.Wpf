using Prism.Events;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.Events;
using RecentlySaved.Wpf.Repositories;
using System;
using System.Collections.Generic;
using System.IO;

namespace RecentlySaved.Wpf
{
  public class FileWatcher : IDisposable
  {
    private List<FileSystemWatcher> watcherList;
    private FileCreatedChangedEvent fileCreatedChangedEvent;
    private FileDeletedEvent fileDeletedEvent;
    private FileRenamedEvent fileRenamedEvent;

    public FileWatcher(SettingsRepository settingsRepository, IEventAggregator eventAggregator)
    {

      this.fileCreatedChangedEvent = eventAggregator.GetEvent<FileCreatedChangedEvent>();
      this.fileDeletedEvent = eventAggregator.GetEvent<FileDeletedEvent>();
      this.fileRenamedEvent = eventAggregator.GetEvent<FileRenamedEvent>();

      this.watcherList = new List<FileSystemWatcher>();

      foreach (string path in settingsRepository.PathsToWatch)
      {
        string actualPath = path.Replace("%HOMEPATH%", System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile));

        var watcher = new FileSystemWatcher(actualPath);

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

      var data = new FileData() { FullPath= e.FullPath, Date = DateTime.Now };
      fileCreatedChangedEvent.Publish(new FileCreatedChangedData { CreatedChangedData = data });
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
      string value = $"Created: {e.FullPath}";
      System.Console.WriteLine(value);

      var data = new FileData() { FullPath = e.FullPath, Date = DateTime.Now };
      fileCreatedChangedEvent.Publish(new FileCreatedChangedData { CreatedChangedData = data });
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
      System.Console.WriteLine($"Deleted: {e.FullPath}");

      var data = new FileData() { FullPath = e.FullPath, Date = DateTime.Now };
      fileDeletedEvent.Publish(new FileDeletedEventData { DeletedFile = data });
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
      System.Console.WriteLine($"Renamed:");
      System.Console.WriteLine($"    Old: {e.OldFullPath}");
      System.Console.WriteLine($"    New: {e.FullPath}");

      var oldData = new FileData() { FullPath = e.OldFullPath };
      var newData = new FileData() { FullPath = e.FullPath, Date = DateTime.Now };
      fileRenamedEvent.Publish(new FileRenamedData { OldData = oldData, NewData = newData });
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
