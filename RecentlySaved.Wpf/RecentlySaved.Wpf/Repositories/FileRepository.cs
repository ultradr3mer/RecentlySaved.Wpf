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
    private readonly RecentFilesChangedEvent fileChangedEvent;
    private readonly Dictionary<string, FileData> recentFiles = new Dictionary<string, FileData>();
    private readonly object lockObj = new object();

    public FileRepository(IEventAggregator eventAggregator)
    {
      if (File.Exists(this.fileName))
      {
        string json = File.ReadAllText(this.fileName);
        this.recentFiles = JsonConvert.DeserializeObject<FileRepoData>(json)?.RecentFiles ?? recentFiles;
      }

      this.fileChangedEvent = eventAggregator.GetEvent<RecentFilesChangedEvent>();

    }

    internal void DeleteFile(string fullPath)
    {
      lock (lockObj)
      {
        if (this.recentFiles.Remove(fullPath))
        {
          this.OnFilesChanged();
        }
      }
    }

    internal List<FileData> GetRecentFiles()
    {
      lock (lockObj)
      {
        return this.recentFiles.Values.OrderByDescending(f => f.Date).ToList();
      }
    }

    internal void RenameFile(string oldFullPath, string fullPath)
    {
      lock (lockObj)
      {
        if (this.recentFiles.TryGetValue(oldFullPath, out FileData data))
        {
          this.recentFiles.Remove(oldFullPath);
          this.recentFiles.Add(fullPath, data);
        }
        else
        {
          this.recentFiles.Add(fullPath, new FileData()
          {
            FilePath = Path.GetDirectoryName(fullPath),
            FileName = Path.GetFileName(fullPath),
            Date = DateTime.Now
          });
        }

        this.OnFilesChanged();
      }
    }

    internal void CreateOrUpdate(string fullPath)
    {
      lock (lockObj)
      {
        if (this.recentFiles.TryGetValue(fullPath, out FileData data))
        {
          data.Date = DateTime.Now;
        }
        else
        {
          this.recentFiles.Add(fullPath, new FileData()
          {
            FilePath = Path.GetDirectoryName(fullPath),
            FileName = Path.GetFileName(fullPath),
            Date = DateTime.Now
          });
        }

        this.OnFilesChanged();
      }
    }

    private void OnFilesChanged()
    {
      string json = JsonConvert.SerializeObject(new FileRepoData { RecentFiles = recentFiles });

      File.WriteAllText(this.fileName, json);

      this.fileChangedEvent.Publish(new RecentFilesChangedData() { repository = this });
    }
  }
}
