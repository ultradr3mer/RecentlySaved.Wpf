namespace RecentlySaved.Console
{
  internal class LinkService
  {
    private readonly string linkPath;

    public LinkService(string linkPath)
    {
      this.linkPath = linkPath;
    }

    internal void CreateUpdateLink(string fullPath)
    {
      (Mutex mutex, string linkPathFile) = this.CreateMutexAndPath(fullPath);

      var fileInfo = FindFirstFile;
      if (fileInfo != null)
      {
        fileInfo.LastAccessTime = (linkPathFile, DateTime.Now);
      }
      else
      {
        var link = Directory.CreateSymbolicLink(path: linkPathFile, pathToTarget: fullPath);
        string test = link.FullName;
      }

      mutex.ReleaseMutex();
    }

    private FileSystemInfo? SafeResolveLinkTarget(string linkPathFile, bool returnFinalTarget)
    {
      FileSystemInfo? result;
      try
      {
        result = File.ResolveLinkTarget(linkPathFile, returnFinalTarget: false);
      }
      catch
      {
        result = null;
      }

      return result;
    }

    internal void DeleteLink(string fullPath)
    {
      (Mutex mutex, string linkPathFile) = this.CreateMutexAndPath(fullPath);

      if (File.Exists(linkPathFile))
      {
        File.Delete(linkPathFile);
      }

      mutex.ReleaseMutex();
    }

    private string GenerateTargetFileName(string fullPath)
    {
      string file_name = Path.GetFileNameWithoutExtension(fullPath);
      string last_folder = Path.GetFileName(Path.GetDirectoryName(fullPath)!);
      string extension = Path.GetExtension(fullPath);
      string targetFileName = $"{file_name}_{last_folder}{extension}";
      return targetFileName;
    }

    private (Mutex mutex, string linkPathFile) CreateMutexAndPath(string fullPath)
    {
      string linkFileName = GenerateTargetFileName(fullPath);
      Mutex mutex = new Mutex(false, linkFileName);
      mutex.WaitOne();

      string linkPathFile = Path.Combine(linkPath, linkFileName);

      return (mutex, linkPathFile);
    }
  }
}
