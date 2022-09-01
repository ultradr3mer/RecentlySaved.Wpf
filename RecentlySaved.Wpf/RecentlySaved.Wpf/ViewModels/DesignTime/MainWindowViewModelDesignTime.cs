﻿using RecentlySaved.Wpf.Composite;
using System;
using System.Collections.Generic;

namespace RecentlySaved.Wpf.ViewModels.DesignTime
{
  public class MainWindowViewModelDesignTime : BaseViewModel
  {
    public List<EntryItemViewModel> Items { get; set; }

    public MainWindowViewModelDesignTime()
    {
      this.Initialize();
    }

    protected virtual void Initialize()
    {
      this.Items = new List<EntryItemViewModel>()
      {
        new EntryItemViewModel { Title = "Datei 1", Description = "Geändert am 01.09.2022" }
      };
    }
  }
}