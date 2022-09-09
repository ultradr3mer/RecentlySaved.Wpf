using RecentlySaved.Wpf.Repositories;
using System;
using System.Collections.Generic;
using System.IO;

namespace RecentlySaved.Wpf
{
  public class FileWatcher : IDisposable
  {
    private List<FileSystemWatcher> watcherList;
    private readonly FileRepository fileRepository;

    public FileWatcher(SettingsRepository settingsRepository, FileRepository fileRepository)
    {
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

      this.fileRepository = fileRepository;
    }


    private void OnChanged(object sender, FileSystemEventArgs e)
    {
      if (e.ChangeType != WatcherChangeTypes.Changed)
      {
        return;
      }
      System.Console.WriteLine($"Changed: {e.FullPath}");
      this.fileRepository.CreateOrUpdate(e.FullPath);
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
      string value = $"Created: {e.FullPath}";
      System.Console.WriteLine(value);
      this.fileRepository.CreateOrUpdate(e.FullPath);
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
      System.Console.WriteLine($"Deleted: {e.FullPath}");
      this.fileRepository.DeleteFile(e.FullPath);
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
      System.Console.WriteLine($"Renamed:");
      System.Console.WriteLine($"    Old: {e.OldFullPath}");
      System.Console.WriteLine($"    New: {e.FullPath}");
      this.fileRepository.RenameFile(e.OldFullPath, e.FullPath);
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
