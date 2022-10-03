using System;

namespace RecentlySaved.Wpf.Data
{
  public class ClipData
  {
    public string Content { get; set; }
    public string ProcessName { get; set; }
    public DateTime Datum { get; set; }
    public bool IsPinned { get; set; }
    public string ImageFileName { get; set; }

    public override bool Equals(object obj)
    {
      if (obj is ClipData other)
      {
        return this.Content == other.Content && ImageFileName == other.ImageFileName;
      }

      return false;
    }
  }
}
