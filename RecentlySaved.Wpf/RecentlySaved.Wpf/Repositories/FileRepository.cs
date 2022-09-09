using Newtonsoft.Json;
using Prism.Events;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.Events;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace RecentlySaved.Wpf.Repositories
{
  public class FileRepository
  {
    private readonly string fileName = "fileRepo.json";
    private readonly Dictionary<string, FileData> recentFiles = new Dictionary<string, FileData>();
    private readonly object lockObj = new object();

    public FileRepository(IEventAggregator eventAggregator)
    {
      if (File.Exists(this.fileName))
      {
        string json = File.ReadAllText(this.fileName);
        this.recentFiles = JsonConvert.DeserializeObject<FileRepoData>(json)?.RecentFiles ?? recentFiles;
      }

      eventAggregator.GetEvent<FileCreatedChangedEvent>().Subscribe(this.OnFileCreatedChanged);
      eventAggregator.GetEvent<FileDeletedEvent>().Subscribe(this.OnFileDeleted);
      eventAggregator.GetEvent<FileRenamedEvent>().Subscribe(this.OnFileRenamed);
    }

    private void OnFileRenamed(FileRenamedData data)
    {
      lock (lockObj)
      {
        this.recentFiles.Remove(data.OldData.FullPath);
        this.recentFiles.Add(data.NewData.FullPath, data.NewData);

        this.OnFilesChanged();
      }
    }

    private void OnFileDeleted(FileDeletedEventData data)
    {
      lock (lockObj)
      {
        if (this.recentFiles.Remove(data.DeletedFile.FullPath))
        {
          this.OnFilesChanged();
        }
      }
    }

    private void OnFileCreatedChanged(FileCreatedChangedData data)
    {
      lock (lockObj)
      {
        if (this.recentFiles.TryGetValue(data.CreatedChangedData.FullPath, out FileData existingData))
        {
          existingData.Date = DateTime.Now;
        }
        else
        {
          this.recentFiles.Add(data.CreatedChangedData.FullPath, data.CreatedChangedData);
        }

        this.OnFilesChanged();
      }
    }

    internal List<FileData> GetRecentFiles()
    {
      lock (lockObj)
      {
        return this.recentFiles.Values.OrderByDescending(f => f.Date).ToList();
      }
    }

    private void OnFilesChanged()
    {
      string json = JsonConvert.SerializeObject(new FileRepoData { RecentFiles = recentFiles });

      File.WriteAllText(this.fileName, json);
    }
  }
}
