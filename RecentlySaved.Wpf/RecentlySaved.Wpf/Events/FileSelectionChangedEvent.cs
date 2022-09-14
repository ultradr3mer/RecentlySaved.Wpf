﻿using Prism.Events;
using RecentlySaved.Wpf.Data;
using RecentlySaved.Wpf.ViewModels.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecentlySaved.Wpf.Events
{
  public class FileSelectionChangedData
  {
    public FileCardViewModel Item { get; set; }
  }

  public class FileSelectionChangedEvent : PubSubEvent<FileSelectionChangedData>
  {

  }
}
