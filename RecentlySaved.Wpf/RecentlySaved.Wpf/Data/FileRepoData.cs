using System.Collections.Generic;

namespace RecentlySaved.Wpf.Data
{
  public class FileRepoData
  {
    public Dictionary<string, FileData> RecentFiles { get; set; } = new Dictionary<string, FileData>();
    public List<ClipData> ClipboardData { get; set; } = new List<ClipData>();
  }
}
