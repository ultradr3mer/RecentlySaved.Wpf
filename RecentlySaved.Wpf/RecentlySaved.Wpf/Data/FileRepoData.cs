using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecentlySaved.Wpf.Data
{
  public class FileRepoData
  {
    public Dictionary<string, FileData> RecentFiles { get; set; } = new Dictionary<string, FileData>();
  }
}
