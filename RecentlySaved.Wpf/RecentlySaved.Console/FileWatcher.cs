using System;
using System.Collections.Generic;
using System.IO;

namespace RecentlySaved.Console
{
  class FileWatcher : IDisposable
  {
    private List<FileSystemWatcher> watcherList;

    public FileWatcher(IEnumerable<string> pathList)
    {
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
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;

        this.watcherList.Add(watcher);
      }

      Console.WriteLine("Press enter to exit.");
      Console.ReadLine();
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
      if (e.ChangeType != WatcherChangeTypes.Changed)
      {
        return;
      }
      Console.WriteLine($"Changed: {e.FullPath}");
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
      string value = $"Created: {e.FullPath}";
      Console.WriteLine(value);
    }

    private void OnDeleted(object sender, FileSystemEventArgs e) =>
        Console.WriteLine($"Deleted: {e.FullPath}");

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
      Console.WriteLine($"Renamed:");
      Console.WriteLine($"    Old: {e.OldFullPath}");
      Console.WriteLine($"    New: {e.FullPath}");
    }

    private void OnError(object sender, ErrorEventArgs e) =>
        PrintException(e.GetException());

    private void PrintException(Exception ex)
    {
      if (ex != null)
      {
        Console.WriteLine($"Message: {ex.Message}");
        Console.WriteLine("Stacktrace:");
        Console.WriteLine(ex.StackTrace);
        Console.WriteLine();
        PrintException(ex.InnerException);
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
