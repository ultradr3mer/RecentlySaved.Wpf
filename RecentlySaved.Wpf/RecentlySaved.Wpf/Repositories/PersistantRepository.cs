using Newtonsoft.Json;
using Prism.Events;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.Events;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using RecentlySaved.Wpf.Extensions;

namespace RecentlySaved.Wpf.Repositories
{
  public class PersistantRepository
  {
    private readonly string fileName = "fileRepo.json";
    private readonly Dictionary<string, FileData> recentFiles = new Dictionary<string, FileData>();
    private readonly List<ClipData> clipboardData = new List<ClipData>();
    private readonly object lockObj = new object();
    private DateTime nextSaveInterval = DateTime.MinValue;
    private bool isDirty = false;
    private string processName;

    public PersistantRepository(IEventAggregator eventAggregator)
    {
      this.processName = Process.GetCurrentProcess().ProcessName;

      if (File.Exists(this.fileName))
      {
        string json = File.ReadAllText(this.fileName);
        var data = JsonConvert.DeserializeObject<FileRepoData>(json);
        this.recentFiles = data?.RecentFiles ?? recentFiles;
        this.clipboardData = data?.ClipboardData ?? clipboardData;
      }

      eventAggregator.GetEvent<FileCreatedChangedEvent>().Subscribe(this.OnFileCreatedChanged);
      eventAggregator.GetEvent<FileDeletedEvent>().Subscribe(this.OnFileDeleted);
      eventAggregator.GetEvent<FileRenamedEvent>().Subscribe(this.OnFileRenamed);
      eventAggregator.GetEvent<ClipboardChangedEvent>().Subscribe(this.OnClipboardChanged);
      eventAggregator.GetEvent<ClipboardPinnedChangedEvent>().Subscribe(this.OnClipboardPinnedChanged);

      nextSaveInterval = DateTime.Now.AddMinutes(30);
    }

    private void OnClipboardPinnedChanged(ClipboardPinnedChangedData data)
    {
      lock (lockObj)
      {
        this.OnFilesChanged();
      }
    }

    private void OnClipboardChanged(ClipboardChangedData data)
    {
      lock (lockObj)
      {
        var existing = clipboardData.FirstOrDefault(c => c.Equals(data.Data));
        if (existing != null)
        {
          existing.Datum = DateTime.Now;
          if(data.Data.ProcessName != this.processName)
          {
            existing.ProcessName = data.Data.ProcessName;
          }
        }
        else
        {
          clipboardData.Add(data.Data);
        }

        this.OnFilesChanged();
      }
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

    internal List<ClipData> GetRecentClipboardData()
    {
      lock (lockObj)
      {
        return this.clipboardData.OrderByDescending(o => o.Datum).ToList();
      }
    }

    internal List<ClipData> GetPinnedClipboardData()
    {
      lock (lockObj)
      {
        return this.clipboardData.Where(c => c.IsPinned).OrderByDescending(o => o.Datum).ToList();
      }
    }

    private void OnFilesChanged()
    {
      nextSaveInterval = nextSaveInterval.AddMinutes(-3);
      isDirty = true;
      Save(false);
    }

    public void Save(bool force)
    {
      lock (lockObj)
      {
        if (!isDirty)
        {
          return;
        }

        if (!force && nextSaveInterval > DateTime.Now)
        {
          return;
        }

        string json = JsonConvert.SerializeObject(new FileRepoData { RecentFiles = recentFiles, ClipboardData = clipboardData });

        File.WriteAllText(this.fileName, json);

        nextSaveInterval = DateTime.Now.AddMinutes(30);
        isDirty = true;
      }
    }
  }
}
