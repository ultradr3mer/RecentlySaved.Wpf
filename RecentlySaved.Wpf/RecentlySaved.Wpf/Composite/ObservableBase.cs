﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RecentlySaved.Wpf.Composite
{
  public class ObservableBase : INotifyPropertyChanged
  {
    #region Events

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion Events

    #region Methods

    protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
    {
      PropertyChangedEventHandler changed = PropertyChanged;
      if (changed == null)
      {
        return;
      }

      changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion Methods
  }
}
