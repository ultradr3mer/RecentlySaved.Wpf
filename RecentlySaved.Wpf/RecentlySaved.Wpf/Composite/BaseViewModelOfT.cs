using Mapster;

namespace RecentlySaved.Wpf.Composite
{
  public class BaseViewModel<T> : ObservableBase where T : class
  {
    #region Fields

    private T attachedDataModel;
    protected bool IsReadingDataModel { get; private set; }

    #endregion Fields

    #region Methods

    public void SetDataModel(T value)
    {
      if (this.attachedDataModel is ObservableBase oldObservable)
      {
        oldObservable.PropertyChanged -= this.Model_PropertyChanged;
      }

      this.IsReadingDataModel = true;
      this.attachedDataModel = value;
      
      if(this.attachedDataModel is ObservableBase newObservable)
      {
        newObservable.PropertyChanged += this.Model_PropertyChanged;
      }

      value.Adapt(this, value.GetType(), this.GetType());
      this.OnReadingDataModel(value);
      this.IsReadingDataModel = false;
    }

    private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
      var thisProp = this.GetType().GetProperty(e.PropertyName);
      if(thisProp == null)
      {
        return;
      }

      var modelProp = typeof(T).GetProperty(e.PropertyName);
      thisProp.SetValue(obj: this, value: modelProp.GetValue(this.attachedDataModel), index: null);
    }

    protected virtual void OnReadingDataModel(T data)
    {

    }

    public T WriteToDataModel()
    {
      return (T)this.Adapt(this.attachedDataModel, this.GetType(), this.attachedDataModel.GetType());
    }

    #endregion Methods
  }
}
